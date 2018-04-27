﻿// -------------------------------------------------------------------------------------------------
// <copyright file="SecurityPortfolioTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.BlackBox.Messaging.MessageStore;
    using Nautilus.BlackBox.Portfolio;
    using Nautilus.BlackBox.Portfolio.Orders;
    using Nautilus.BlackBox.Portfolio.Processors;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class SecurityPortfolioTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLogger mockLogger;
        private readonly MessageWarehouse messageWarehouse;
        private readonly IQuoteProvider quoteProvider;
        private readonly TradeBook tradeBook;
        private readonly IActorRef portfolioRef;

        public SecurityPortfolioTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var instrument = StubInstrumentFactory.AUDUSD();

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLogger = setupFactory.Logger;

            var testActorSystem = ActorSystem.Create(nameof(SecurityPortfolioTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                BlackBoxEnvironment.Live,
                setupContainer.Clock,
                setupContainer.LoggerFactory);

            this.messageWarehouse = messagingServiceFactory.MessageWarehouse;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.quoteProvider = setupFactory.QuoteProvider;

            this.tradeBook = new TradeBook(setupContainer, instrument.Symbol);

            var orderExpiryController = new OrderExpiryController(
                setupContainer,
                messagingAdapter,
                instrument.Symbol);

            var entrySignalProcessor = new EntrySignalProcessor(
                setupContainer,
                messagingAdapter,
                instrument,
                this.tradeBook);

            var exitSignalProcessor = new ExitSignalProcessor(
                setupContainer,
                messagingAdapter,
                instrument,
                this.tradeBook);

            var trailingStopSignalProcessor = new TrailingStopSignalProcessor(
                setupContainer,
                messagingAdapter,
                instrument,
                this.tradeBook);

            this.portfolioRef = testActorSystem.ActorOf(Props.Create(() => new SecurityPortfolio(
                setupContainer,
                messagingAdapter,
                instrument,
                this.tradeBook,
                orderExpiryController,
                entrySignalProcessor,
                exitSignalProcessor,
                trailingStopSignalProcessor)));
        }


        [Fact]
        internal void GivenMarketDataEvent_WhenEventValid_SendsEventToCorrectPortfolio()
        {
            // Arrange
            var message = ValidMarketDataEventBullBar();

            // Act
            this.portfolioRef.Tell(message);

            // Allow trade to initialize
            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
        }

        [Fact]
        internal void GivenTradeApprovedEvent_WithValidTrade_InitializesNewTrade()
        {
            // Arrange
            var orderPacket = StubOrderPacketBuilder.Build();

            var message = new TradeApproved(
                orderPacket,
                2,
                Guid.NewGuid(),
                StubDateTime.Now());

            // Act
            this.portfolioRef.Tell(message);

            // Allow trade to initialize
            Task.Delay(100).Wait();
            var result = this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"));

            // Assert
            Assert.Equal(3, this.tradeBook.GetAllActiveOrderIds().Count);
            Assert.Equal(TradeStatus.Initialized, result[0].TradeStatus);
            Assert.Equal(MarketPosition.Flat, result[0].MarketPosition);
        }

        [Fact]
        internal void GivenTradeApprovedEvent_WithValidTrade_SendsSubmitTradeCommandToCommandBus()
        {
            // Arrange
            var orderPacket = StubOrderPacketBuilder.Build();

            var message = new TradeApproved(
                orderPacket,
                2,
                Guid.NewGuid(),
                StubDateTime.Now());

            // Act
            this.portfolioRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyContains(
                typeof(SubmitTrade),
                this.messageWarehouse.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenEntrySignalWrapper_ThenSendsSignalForProcessing()
        {
            // Arrange
            var quote = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubDateTime.Now());
            this.quoteProvider.OnQuote(quote);

            var signal = StubSignalBuilder.BuyEntrySignal();
            var message = new SignalEvent(
                signal,
                Guid.NewGuid(),
                StubDateTime.Now());

            // Act
            this.portfolioRef.Tell(message);
            LogDumper.Dump(this.mockLogger, this.output);

            // Assert
            CustomAssert.EventuallyContains(
                typeof(RequestTradeApproval),
                this.messageWarehouse.DocumentEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenExitSignalWrapper_ThenSendsSignalForProcessing()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            var signal = StubSignalBuilder.LongExitSignal(
                new TradeType("TestTrade"),
                new List<int> { 0 },
                Period.FromMinutes(5));

            var message = new SignalEvent(signal, Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.portfolioRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyContains(
                typeof(ClosePosition),
                this.messageWarehouse.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenTrailingStopSignalWrapper_ThenSendsSignalForProcessing()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            var signal = StubSignalBuilder.LongTrailingStopSignal(
                new TradeType("TestTrade"),
                new Dictionary<int, Price> { { 0, Price.Create(0.79990m, 0.00001m) } },
                Period.FromMinutes(5));

            var message = new SignalEvent(signal, Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.portfolioRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyContains(
                typeof(ModifyStopLoss),
                this.messageWarehouse.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenOrderRejectedEvent_ThenSendsEventToOrder()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);
            var order = trade.TradeUnits[0].Entry;

            var message = StubEventMessages.OrderRejectedEvent(order);

            // Act
            Task.Delay(100).Wait();
            this.portfolioRef.Tell(message);

            // Assert
            Task.Delay(100).Wait();
            Assert.Equal(OrderStatus.Rejected, order.OrderStatus);
        }

        [Fact]
        internal void GivenOrderExpiredEvent_ThenSendsEventToOrder()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);
            var order = trade.TradeUnits[0].Entry;

            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = StubEventMessages.OrderExpiredEvent(order);

            // Act
            this.portfolioRef.Tell(message1);
            this.portfolioRef.Tell(message2);

            // Assert
            Task.Delay(100).Wait();
            Assert.Equal(OrderStatus.Expired, order.OrderStatus);
        }

        [Fact]
        internal void GivenOrderCancelledEvent_ThenSendsEventToOrder()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);
            var order = trade.TradeUnits[0].Entry;

            var message = StubEventMessages.OrderCancelledEvent(order);

            // Act
            this.portfolioRef.Tell(message);

            // Assert
            Task.Delay(150).Wait();
            Assert.Equal(OrderStatus.Cancelled, order.OrderStatus);
        }

        [Fact]
        internal void GivenOrderWorkingEvent_ThenSendsEventToOrder()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);
            var order = trade.TradeUnits[0].Entry;

            var message = StubEventMessages.OrderWorkingEvent(order);

            // Act
            this.portfolioRef.Tell(message);

            // Assert
            Task.Delay(100).Wait();
            Assert.Equal(OrderStatus.Working, order.OrderStatus);
        }

        [Fact]
        internal void GivenOrderFilledEvent_ThenSendsEventToOrder()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);
            var order = trade.TradeUnits[0].Entry;

            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = StubEventMessages.OrderFilledEvent(order);

            // Act
            this.portfolioRef.Tell(message1);
            this.portfolioRef.Tell(message2);

            // Assert
            Task.Delay(100).Wait();
            Assert.Equal(OrderStatus.Filled, order.OrderStatus);
        }

        [Fact]
        internal void GivenOrderPartiallyFilledEvent_ThenSendsEventToOrder()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);
            var order = trade.TradeUnits[0].Entry;

            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = StubEventMessages.OrderPartiallyFilledEvent(order, 50000, 50000);

            // Act
            this.portfolioRef.Tell(message1);
            this.portfolioRef.Tell(message2);

            // Assert
            Task.Delay(100).Wait();
            Assert.Equal(OrderStatus.PartiallyFilled, order.OrderStatus);
        }

        [Fact]
        internal void GivenOrderModifiedEvent_ThenSendsEventToOrder()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);
            var order = trade.TradeUnits[0].Entry;

            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = StubEventMessages.OrderModifiedEvent(order, Price.Create(0.80050m, 0.00001m));

            // Act
            this.portfolioRef.Tell(message1);
            this.portfolioRef.Tell(message2);

            // Assert
            Task.Delay(100).Wait();
            Assert.Equal(OrderStatus.Working, order.OrderStatus);
            Assert.Equal(Price.Create(0.80050m, 0.00001m), order.Price);
        }

        private static MarketDataEvent ValidMarketDataEventBullBar()
        {
            return new MarketDataEvent(
                new Symbol("AUDUSD", Exchange.FXCM),
                new TradeType("TestTrade"),
                new BarSpecification(BarTimeFrame.Minute, 5),
                new Bar(
                    Price.Create(0.80100m, 0.00001m),
                    Price.Create(0.80200m, 0.00001m),
                    Price.Create(0.80110m, 0.00001m),
                    Price.Create(0.80150m, 0.00001m),
                    Quantity.Create(1000),
                    StubDateTime.Now() + Period.FromMinutes(5).ToDuration()),
                StubTickFactory.Create(new Symbol("SYMBOL", Exchange.LMAX)),
                0.00001m,
                false,
                Guid.NewGuid(),
                StubDateTime.Now() + Period.FromMinutes(5).ToDuration());
        }
    }
}
