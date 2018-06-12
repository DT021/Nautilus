﻿//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Scheduler.Commands;
    using Nautilus.Scheduler.Events;
    using Quartz;

    /// <summary>
    /// This class is responsible for coordinating the creation of closed <see cref="Bar"/> data
    /// events from ingested <see cref="Tick"/>s based on bar jobs created from subscriptions.
    /// </summary>
    public sealed class BarAggregationController : ActorComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IImmutableList<Enum> barDataReceivers;
        private readonly IActorRef schedulerRef;
        private readonly IDictionary<Symbol, IActorRef> barAggregators;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregationController"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="barReceivers">The bar data receivers.</param>
        /// <param name="schedulerRef">The scheduler actor address.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BarAggregationController(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IImmutableList<Enum> barReceivers,
            IActorRef schedulerRef,
            Enum serviceContext)
            : base(
            serviceContext,
            LabelFactory.Component(nameof(BarAggregationController)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(barReceivers, nameof(barReceivers));
            Validate.NotNull(schedulerRef, nameof(schedulerRef));
            Validate.NotNull(serviceContext, nameof(serviceContext));

            this.storedContainer = container;
            this.barDataReceivers = barReceivers.ToImmutableList();
            this.schedulerRef = schedulerRef;
            this.barAggregators = new Dictionary<Symbol, IActorRef>();

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<SubscribeBarData>(msg => this.OnMessage(msg));
            this.Receive<UnsubscribeBarData>(msg => this.OnMessage(msg));
            this.Receive<JobCreated>(msg => this.OnMessage(msg));
            this.Receive<BarJob>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnMessage(msg));
            this.Receive<BarClosed>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Handles the message by creating a <see cref="BarAggregator"/> for the symbol if none
        /// exists, then forwarding the message there. Bar jobs and then registered with the
        /// <see cref="Akka.Actor.IScheduler"/>.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(SubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));

            if (!this.barAggregators.ContainsKey(message.Symbol))
            {
                var barAggregatorRef = Context.ActorOf(Props.Create(() => new BarAggregator(
                    this.storedContainer,
                    this.Service,
                    message.Symbol)));

                this.barAggregators.Add(message.Symbol, barAggregatorRef);
            }

            this.barAggregators[message.Symbol].Tell(message);

            foreach (var barSpec in message.BarSpecifications)
            {
                var job = new BarJob(message.Symbol, barSpec);

                this.schedulerRef.Tell(
                    new CreateJob(
                        this.Self,
                        job,
                        TriggerBuilder
                            .Create()
                            .WithCronSchedule("") // TODO
                            .Build()));
            }
        }

        /// <summary>
        /// Handles the message by cancelling the bar jobs with the <see cref="Akka.Actor.IScheduler"/>, then
        /// forwarding the message to the <see cref="BarAggregator"/> for the symbol.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(UnsubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.DictionaryContainsKey(message.Symbol, nameof(message.Symbol), this.barAggregators);

            foreach (var barSpec in message.BarSpecifications)
            {
                var job = (new BarJob(message.Symbol, barSpec));
            }

            this.barAggregators[message.Symbol].Tell(message);
        }

        private void OnMessage(JobCreated message)
        {
            Debug.NotNull(message, nameof(message));

            Log.Debug($"{message.JobKey}, {message.TriggerKey}");
        }

//        /// <summary>
//        /// Adds the given bar job to the <see cref="IScheduler"/>.
//        /// </summary>
//        /// <param name="job">The bar job.</param>
//        private void AddJob(BarJob job)
//        {
//            if (!this.barJobs.ContainsKey(job))
//            {
//                var timeNow = this.TimeNow();
//                var delay = timeNow.CeilingOffsetMilliseconds(job.BarSpecification.Duration);
//                var startTime = timeNow + Duration.FromMilliseconds(delay);
//                var interval = (int)job.BarSpecification.Duration.TotalMilliseconds;
//
//                var cancellationToken = this.scheduler.ScheduleTellRepeatedlyCancelable(
//                    delay,
//                    interval,
//                    this.Self,
//                    job,
//                    this.Self);
//
//                this.barJobs.Add(job, cancellationToken);
//
//                Log.Debug($"Bar job added {job} starting at {startTime.ToIsoString()} " +
//                          $"initial delay {delay} then every {interval / 1000}s");
//            }
//        }
//
//        /// <summary>
//        /// Cancels the associated job token, then removes the given bar job.
//        /// </summary>
//        /// <param name="job">The bar job.</param>
//        private void RemoveJob(BarJob job)
//        {
//            if (this.barJobs.ContainsKey(job))
//            {
//                Log.Debug($"Bar job token {this.barJobs[job]} cancelling...");
//
//                this.barJobs[job].Cancel();
//                this.barJobs.Remove(job);
//
//                Log.Debug($"Bar job removed {job}");
//            }
//        }

        /// <summary>
        /// Handles the message by forwarding the given <see cref="Tick"/> to the relevant
        /// <see cref="BarAggregator"/>.
        /// </summary>
        /// <param name="tick">The received tick.</param>
        private void OnMessage(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));

            if (this.barAggregators.ContainsKey(tick.Symbol))
            {
                this.barAggregators[tick.Symbol].Tell(tick);

                return;
            }

            Log.Warning($"Does not contain aggregator for {tick.Symbol} ticks.");
        }

        /// <summary>
        /// Handles the message by creating a <see cref="CloseBar"/> command message which is then
        /// forwarded to the relevant <see cref="BarAggregator"/>.
        /// </summary>
        /// <param name="job">The received job.</param>
        private void OnMessage(BarJob job)
        {
            if (this.barAggregators.ContainsKey(job.Symbol))
            {
                var closeBar = new CloseBar(
                    job.BarSpecification,
                    this.TimeNow().Floor(job.BarSpecification.Duration),
                    this.NewGuid(),
                    this.TimeNow());

                this.barAggregators[job.Symbol].Tell(closeBar);

                // Log for unit testing only.
                Log.Debug($"Received {job} at {DateTime.UtcNow.Millisecond}.");
                return;
            }

            Log.Warning($"Does not contain aggregator to close bar for {job}.");
        }

        /// <summary>
        /// Handles the message by creating a new event message which is then forwarded to the
        /// list of held bar data event receivers.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(BarClosed message)
        {
            var @event = new EventMessage(
                message,
                this.NewGuid(),
                this.TimeNow());

            this.Send(this.barDataReceivers, @event);
        }
    }
}
