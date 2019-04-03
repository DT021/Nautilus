﻿//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregationControllerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.AggregatorTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Data.Aggregators;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarAggregationControllerTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly IActorRef controllerRef;

        public BarAggregationControllerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.logger = setupFactory.LoggingAdapter;
            var container = setupFactory.Create();

            var messagingAdapter = new MockMessagingAdapter(this.TestActor);

            var props = Props.Create(() => new BarAggregationController(
                container,
                messagingAdapter));

            this.controllerRef = this.ActorOfAsTestActorRef<BarAggregator>(props, this.TestActor);
        }

        [Fact]
        internal void GivenSubscribeBarDataMessage_CreatesAggregatorAndJobs()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol, new BarSpecification(QuoteType.Bid, Resolution.Second, 1));
            var barType2 = new BarType(symbol, new BarSpecification(QuoteType.Bid, Resolution.Minute, 1));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controllerRef.Tell(subscribe1);
            this.controllerRef.Tell(subscribe2);

            // LogDumper.Dump(this.logger, this.output);
            // Assert
        }

        [Fact]
        internal void GivenMultipleSubscribeBarDataMessages_CreatesNeededJobs()
        {
            // Arrange
            var symbol1 = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol1, new BarSpecification(QuoteType.Bid, Resolution.Second, 1));
            var barType2 = new BarType(symbol1, new BarSpecification(QuoteType.Bid, Resolution.Minute, 1));

            var symbol2 = new Symbol("GBPUSD", Venue.FXCM);
            var barType3 = new BarType(symbol2, new BarSpecification(QuoteType.Bid, Resolution.Second, 1));
            var barType4 = new BarType(symbol2, new BarSpecification(QuoteType.Bid, Resolution.Minute, 1));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe3 = new Subscribe<BarType>(
                barType3,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe4 = new Subscribe<BarType>(
                barType4,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controllerRef.Tell(subscribe1);
            this.controllerRef.Tell(subscribe2);
            this.controllerRef.Tell(subscribe3);
            this.controllerRef.Tell(subscribe4);

            // LogDumper.Dump(this.logger, this.output);
            // Assert
        }

        [Fact]
        internal void GivenUnsubscribeBarDataMessage_RemovesJobs()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol, new BarSpecification(QuoteType.Bid, Resolution.Second, 1));
            var barType2 = new BarType(symbol, new BarSpecification(QuoteType.Bid, Resolution.Minute, 1));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<BarType>(
                barType2,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controllerRef.Tell(subscribe1);
            this.controllerRef.Tell(subscribe2);
            Thread.Sleep(2000);
            this.controllerRef.Tell(unsubscribe);

            // LogDumper.Dump(this.logger, this.output);
            // Assert
        }

        [Fact]
        internal void GivenUnsubscribeBarDataMessage_WithMultipleJobsDoesNotRemoveTrigger()
        {
            // Arrange
            var symbol1 = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol1, new BarSpecification(QuoteType.Bid, Resolution.Second, 1));
            var barType2 = new BarType(symbol1, new BarSpecification(QuoteType.Bid, Resolution.Minute, 1));

            var symbol2 = new Symbol("GBPUSD", Venue.FXCM);
            var barType3 = new BarType(symbol2, new BarSpecification(QuoteType.Bid, Resolution.Second, 1));
            var barType4 = new BarType(symbol2, new BarSpecification(QuoteType.Bid, Resolution.Minute, 1));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe3 = new Subscribe<BarType>(
                barType3,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe4 = new Subscribe<BarType>(
                barType4,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<BarType>(
                barType2,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controllerRef.Tell(subscribe1);
            this.controllerRef.Tell(subscribe2);
            this.controllerRef.Tell(subscribe3);
            this.controllerRef.Tell(subscribe4);
            Thread.Sleep(5000);
            this.controllerRef.Tell(unsubscribe);

            // LogDumper.Dump(this.logger, this.output);
            // Assert
        }
    }
}
