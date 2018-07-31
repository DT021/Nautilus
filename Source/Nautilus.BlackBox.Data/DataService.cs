﻿//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Commands;
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Aggregators;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="DataService"/> class. The <see cref="BlackBox"/> service context
    /// which handles all data related operations.
    /// </summary>
    public sealed class DataService : ActorComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IScheduler scheduler;
        private readonly IActorRef marketDataPortRef;
        private readonly IDictionary<Symbol, IActorRef> marketDataProcessorIndex = new Dictionary<Symbol, IActorRef>();

        private IExecutionGateway executionGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public DataService(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IScheduler scheduler)
            : base(
            NautilusService.Data,
            LabelFactory.Component(nameof(DataService)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(scheduler, nameof(scheduler));

            this.storedContainer = container;
            this.scheduler = scheduler;
            this.marketDataPortRef = Context.ActorOf(
                Props.Create(() => new MarketDataPort(container, messagingAdapter)));

            this.SetupCommandMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<InitializeGateway>(msg => this.OnMessage(msg));
            this.Receive<Subscribe<BarType>>(msg => this.OnMessage(msg));
            this.Receive<Unsubscribe<BarType>>(msg => this.OnMessage(msg));
            this.Receive<ShutdownSystem>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Creates a new <see cref="BarAggregationController"/> for the symbol and exchange. Registers
        /// the data type with the processor, updates the index of processors with the
        /// <see cref="MarketDataPort"/>. Then subscribes to the market data for the symbol and exchange.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessage(Subscribe<BarType> message)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(this.executionGateway, nameof(this.executionGateway));

            this.Execute(() =>
            {
                var symbol = message.DataType.Symbol;

//                if (!this.marketDataProcessorIndex.ContainsKey(symbol))
//                {
//                    var barReceivers = new List<Enum>{BlackBoxService.AlphaModel}.ToImmutableList();
//                    var schedulerRef = Context.ActorOf(Props.Create(() => new Scheduler()));
//                    var marketDataProcessorRef = Context.ActorOf(Props.Create(() => new BarAggregationController(
//                        this.storedContainer,
//                        this.GetMessagingAdapter(),
//                        barReceivers,
//                        schedulerRef,
//                        BlackBoxService.Data)));
//
//                    this.marketDataProcessorIndex.Add(symbol, marketDataProcessorRef);
//                    this.marketDataProcessorIndex[symbol].Tell(message, this.Self);
//                    this.marketDataPortRef.Tell(new MarketDataProcessorIndexUpdate(this.marketDataProcessorIndex, this.NewGuid(), this.TimeNow()));
//                    this.brokerageGateway.RequestMarketDataSubscribe(symbol);
//                }
            });
        }

        private void OnMessage(Unsubscribe<BarType> message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                var symbol = message.DataType.Symbol;

                if (this.marketDataProcessorIndex.ContainsKey(symbol))
                this.marketDataProcessorIndex[symbol].Tell(message, this.Self);
            });
        }

        // Brokerage Gateway should be null before receiving this message.
        private void OnMessage(InitializeGateway message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
                {
                    this.executionGateway = message.ExecutionGateway;
                });
        }

        private void OnMessage(ShutdownSystem message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                // TODO
            });
        }
    }
}
