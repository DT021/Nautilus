﻿//--------------------------------------------------------------------------------------------------
// <copyright file="TickDataProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Processors
{
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Aggregators;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides an entry point for ticks into the <see cref="Nautilus.Database"/> system. The given
    /// tick size index must contain all symbols which the processor can expect to receive ticks for.
    /// </summary>
    public sealed class TickProcessor : ComponentBase, ITickProcessor
    {
        private readonly IActorRef tickPublisher;
        private readonly IActorRef barAggregationControllerRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickProcessor"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="tickPublisher">The tick publisher.</param>
        /// <param name="barAggregationControllerRef">The bar aggregator controller actor address.</param>
        public TickProcessor(
            IComponentryContainer container,
            IActorRef tickPublisher,
            IActorRef barAggregationControllerRef) : base(
            ServiceContext.Database,
            LabelFactory.Component(nameof(TickProcessor)),
            container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(tickPublisher, nameof(tickPublisher));
            Validate.NotNull(barAggregationControllerRef, nameof(barAggregationControllerRef));

            this.tickPublisher = tickPublisher;
            this.barAggregationControllerRef = barAggregationControllerRef;
        }

        /// <summary>
        /// Creates a new <see cref="Tick"/> and sends it to the <see cref="IQuoteProvider"/> and
        /// the <see cref="BarAggregationController"/>.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="exchange">The tick exchange.</param>
        /// <param name="bid">The tick bid price.</param>
        /// <param name="ask">The tick ask price.</param>
        /// <param name="decimals">The expected decimal precision of the tick prices.</param>
        /// <param name="timestamp">The tick timestamp.</param>
        public void OnTick(
            string symbol,
            Exchange exchange,
            decimal bid,
            decimal ask,
            int decimals,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.DecimalNotOutOfRange(bid, nameof(bid), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
                Validate.DecimalNotOutOfRange(ask, nameof(ask), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
                Debug.Int32NotOutOfRange(decimals, nameof(decimals), 0, int.MaxValue);

                var tick = new Tick(
                    new Symbol(symbol, exchange),
                    Price.Create(bid, decimals),
                    Price.Create(ask, decimals),
                    timestamp);

                this.tickPublisher.Tell(tick);
                this.barAggregationControllerRef.Tell(tick);
            });
        }
    }
}
