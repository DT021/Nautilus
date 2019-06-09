﻿//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler;
    using NodaTime;

    /// <summary>
    /// Provides a data service.
    /// </summary>
    public sealed class DataService : ComponentBusConnected
    {
        private readonly IScheduler scheduler;
        private readonly IFixGateway fixGateway;
        private readonly IReadOnlyCollection<Symbol> subscribingSymbols;
        private readonly IReadOnlyCollection<BarSpecification> barSpecifications;
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixConnectTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixDisconnectTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) tickDataTrimTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) barDataTrimTime;
        private readonly int tickRollingWindowDays;
        private readonly int barRollingWindowDays;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="fixGateway">The FIX gateway.</param>
        /// <param name="addresses">The data service address dictionary.</param>
        /// <param name="config">The service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        public DataService(
            IComponentryContainer container,
            MessagingAdapter messagingAdapter,
            Dictionary<Address, IEndpoint> addresses,
            IScheduler scheduler,
            IFixGateway fixGateway,
            Configuration config)
            : base(container, messagingAdapter)
        {
            Condition.NotEmpty(addresses, nameof(addresses));

            this.scheduler = scheduler;
            this.fixGateway = fixGateway;
            this.subscribingSymbols = config.SubscribingSymbols;
            this.barSpecifications = config.BarSpecifications;

            this.fixConnectTime = config.FixConfiguration.ConnectTime;
            this.fixDisconnectTime = config.FixConfiguration.DisconnectTime;
            this.tickDataTrimTime = config.TickDataTrimTime;
            this.tickRollingWindowDays = config.TickDataTrimWindowDays;
            this.barDataTrimTime = config.BarDataTrimTime;
            this.barRollingWindowDays = config.BarDataTrimWindowDays;

            addresses.Add(DataServiceAddress.Core, this.Endpoint);
            messagingAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                this.NewGuid(),
                this.TimeNow()));

            this.RegisterHandler<ConnectFix>(this.OnMessage);
            this.RegisterHandler<DisconnectFix>(this.OnMessage);
            this.RegisterHandler<FixSessionConnected>(this.OnMessage);
            this.RegisterHandler<FixSessionDisconnected>(this.OnMessage);
            this.RegisterHandler<MarketOpened>(this.OnMessage);
            this.RegisterHandler<MarketClosed>(this.OnMessage);
            this.RegisterHandler<TrimTickData>(this.OnMessage);
            this.RegisterHandler<TrimBarData>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            if (TimingProvider.IsOutsideWeeklyInterval(
                this.fixDisconnectTime,
                this.fixConnectTime,
                this.InstantNow()))
            {
                this.Send(start, DataServiceAddress.FixGateway);
            }
            else
            {
                this.CreateConnectFixJob();
            }

            this.CreateMarketOpenedJob();
            this.CreateMarketClosedJob();
            this.CreateTrimTickDataJob();
            this.CreateTrimBarDataJob();

            this.Send(start, DataServiceAddress.TickProvider);
            this.Send(start, DataServiceAddress.TickPublisher);
            this.Send(start, DataServiceAddress.BarProvider);
            this.Send(start, DataServiceAddress.BarPublisher);
            this.Send(start, DataServiceAddress.InstrumentProvider);
            this.Send(start, DataServiceAddress.InstrumentPublisher);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.Send(stop, DataServiceAddress.DatabaseTaskManager);
            this.Send(stop, DataServiceAddress.FixGateway);
            this.Send(stop, DataServiceAddress.TickProvider);
            this.Send(stop, DataServiceAddress.TickPublisher);
            this.Send(stop, DataServiceAddress.BarProvider);
            this.Send(stop, DataServiceAddress.BarPublisher);
            this.Send(stop, DataServiceAddress.InstrumentProvider);
            this.Send(stop, DataServiceAddress.InstrumentPublisher);
        }

        private void OnMessage(ConnectFix message)
        {
            // Forward message.
            this.Send(message, DataServiceAddress.FixGateway);
        }

        private void OnMessage(DisconnectFix message)
        {
            // Forward message.
            this.Send(message, DataServiceAddress.FixGateway);
        }

        private void OnMessage(FixSessionConnected message)
        {
            this.Log.Information($"{message.SessionId} session is connected.");

            this.fixGateway.UpdateInstrumentsSubscribeAll();

            foreach (var symbol in this.subscribingSymbols)
            {
                this.fixGateway.MarketDataSubscribe(symbol);

                foreach (var barSpec in this.barSpecifications)
                {
                    var barType = new BarType(symbol, barSpec);
                    var subscribe = new Subscribe<BarType>(
                        barType,
                        this.Endpoint,
                        this.NewGuid(),
                        this.TimeNow());
                    this.Send(subscribe, DataServiceAddress.BarAggregationController);
                }
            }

            this.CreateDisconnectFixJob();
            this.CreateConnectFixJob();
        }

        private void OnMessage(FixSessionDisconnected message)
        {
            this.Log.Warning($"{message.SessionId} session has been disconnected.");
        }

        private void OnMessage(MarketOpened message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message.
            this.Send(message, DataServiceAddress.BarAggregationController);

            this.CreateMarketClosedJob();
        }

        private void OnMessage(MarketClosed message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message.
            this.Send(message, DataServiceAddress.BarAggregationController);

            this.CreateMarketOpenedJob();
        }

        private void OnMessage(TrimTickData message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message.
            this.Send(message, DataServiceAddress.TickStore);

            this.CreateTrimTickDataJob();
        }

        private void OnMessage(TrimBarData message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message.
            this.Send(message, DataServiceAddress.DatabaseTaskManager);

            this.CreateTrimBarDataJob();
        }

        private void CreateConnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.fixConnectTime.Day,
                this.fixConnectTime.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new ConnectFix(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
        }

        private void CreateDisconnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.fixDisconnectTime.Day,
                this.fixDisconnectTime.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new DisconnectFix(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
        }

        private void CreateMarketOpenedJob()
        {
            var jobDay = IsoDayOfWeek.Sunday;
            var jobTime = new LocalTime(21, 00);
            var now = this.InstantNow();

            foreach (var symbol in this.subscribingSymbols)
            {
                var nextTime = TimingProvider.GetNextUtc(jobDay, jobTime, now);
                var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, this.InstantNow());

                var marketOpened = new MarketOpened(
                    symbol,
                    nextTime,
                    this.NewGuid(),
                    this.TimeNow());

                this.scheduler.ScheduleSendOnceCancelable(
                    durationToNext,
                    this.Endpoint,
                    marketOpened,
                    this.Endpoint);

                this.Log.Information($"Created scheduled event {marketOpened}Event[{symbol}] for {nextTime.ToIsoString()}");
            }
        }

        private void CreateMarketClosedJob()
        {
            var jobDay = IsoDayOfWeek.Saturday;
            var jobTime = new LocalTime(20, 00);
            var now = this.InstantNow();

            foreach (var symbol in this.subscribingSymbols)
            {
                var nextTime = TimingProvider.GetNextUtc(jobDay, jobTime, now);
                var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, this.InstantNow());

                var marketClosed = new MarketClosed(
                    symbol,
                    nextTime,
                    this.NewGuid(),
                    this.TimeNow());

                this.scheduler.ScheduleSendOnceCancelable(
                    durationToNext,
                    this.Endpoint,
                    marketClosed,
                    this.Endpoint);

                this.Log.Information($"Created scheduled event {marketClosed}Event[{symbol}] for {nextTime.ToIsoString()}");
            }
        }

        private void CreateTrimTickDataJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.tickDataTrimTime.Day,
                this.tickDataTrimTime.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new TrimTickData(
                nextTime - Duration.FromDays(this.tickRollingWindowDays),
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
        }

        private void CreateTrimBarDataJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.barDataTrimTime.Day,
                this.barDataTrimTime.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new TrimBarData(
                this.barSpecifications,
                this.barRollingWindowDays,
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
        }
    }
}
