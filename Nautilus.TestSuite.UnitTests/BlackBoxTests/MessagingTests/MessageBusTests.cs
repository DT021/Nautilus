﻿//--------------------------------------------------------------
// <copyright file="MessageBusTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.MessagingTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Akka.Event;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MessageBusTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLogger mockLogger;
        private readonly IActorRef messageBusRef;

        public MessageBusTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLogger = setupFactory.Logger;

            var testActorSystem = ActorSystem.Create(nameof(MessagingTests));

            this.messageBusRef = testActorSystem.ActorOf(Props.Create(() => new MessageBus<CommandMessage>(
                ServiceContext.Messaging,
                new Label(BlackBoxService.CommandBus.ToString()),
                setupContainer,
                new StandardOutLogger())));

            var mockAlphaModelServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));
            var mockDataServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));
            var mockExecutionServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));
            var mockPortfolioServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));
            var mockRiskServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));

            var addresses = new Dictionary<Enum, IActorRef>
            {
                { BlackBoxService.AlphaModel, mockAlphaModelServiceRef },
                { BlackBoxService.Data, mockDataServiceRef },
                { BlackBoxService.Execution, mockExecutionServiceRef },
                { BlackBoxService.Portfolio, mockPortfolioServiceRef },
                { BlackBoxService.Risk, mockRiskServiceRef }
            };

            this.messageBusRef.Tell(new InitializeMessageSwitchboard(
                new Switchboard(addresses),
                Guid.NewGuid(),
                setupContainer.Clock.TimeNow()));
        }

        [Fact]
        internal void GivenNullObjectMessage_Handles()
        {
            // Arrange

            // Act
            this.messageBusRef.Tell(string.Empty);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);

            CustomAssert.EventuallyContains(
                "CommandBus: Unhandled message",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }
    }
}