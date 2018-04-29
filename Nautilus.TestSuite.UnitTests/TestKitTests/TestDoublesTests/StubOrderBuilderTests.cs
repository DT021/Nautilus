﻿//--------------------------------------------------------------
// <copyright file="StubOrderBuilderTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.TestKitTests.TestDoublesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StubOrderBuilderTests
    {
        [Fact]
        internal void Build_WithNoParametersModified_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarket();

            // Assert
            Assert.Equal(new Symbol("AUDUSD", Exchange.FXCM), order.Symbol);
            Assert.Equal("StubOrderId", order.OrderId.ToString());
            Assert.Equal("StubOrderLabel", order.OrderLabel.ToString());
            Assert.Equal(OrderSide.Buy, order.OrderSide);
            Assert.Equal(OrderType.StopMarket, order.OrderType);
            Assert.Equal(Quantity.Create(1), order.Quantity);
            Assert.Equal(Price.Create(1, 1), order.Price);
            Assert.Equal(TimeInForce.DAY, order.TimeInForce);
            Assert.True(order.ExpireTime.HasNoValue);
            Assert.Equal(StubDateTime.Now(), order.OrderTimestamp);
        }

        [Fact]
        internal void Build_WithAllParametersModified_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder()
               .WithSymbol(new Symbol("AUDUSD", Exchange.FXCM))
               .WithOrderId("TestOrderId")
               .WithOrderLabel("TestOrderLabel")
               .WithOrderSide(OrderSide.Sell)
               .WithOrderQuantity(Quantity.Create(100000))
               .WithOrderPrice(Price.Create(1.00000m, 0.00001m))
               .WithTimeInForce(TimeInForce.GTD)
               .WithExpireTime(StubDateTime.Now() + Period.FromMinutes(5).ToDuration())
               .WithTimestamp(StubDateTime.Now() + Period.FromMinutes(1).ToDuration())
               .BuildStopMarket();

            // Assert
            Assert.Equal(new Symbol("AUDUSD", Exchange.FXCM), order.Symbol);
            Assert.Equal("TestOrderId", order.OrderId.ToString());
            Assert.Equal("TestOrderLabel", order.OrderLabel.ToString());
            Assert.Equal(OrderSide.Sell, order.OrderSide);
            Assert.Equal(OrderType.StopMarket, order.OrderType);
            Assert.Equal(Quantity.Create(100000), order.Quantity);
            Assert.Equal(Price.Create(1.00000m, 0.00001m), order.Price);
            Assert.Equal(TimeInForce.GTD, order.TimeInForce);
            Assert.Equal(StubDateTime.Now() + Period.FromMinutes(5).ToDuration(), order.ExpireTime);
            Assert.Equal(StubDateTime.Now() + Period.FromMinutes(1).ToDuration(), order.OrderTimestamp);
        }

        [Fact]
        internal void BuildMarket_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildMarket();

            // Assert
            Assert.Equal(OrderType.Market, order.OrderType);
        }

        [Fact]
        internal void BuildStopMarket_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarket();

            // Assert
            Assert.Equal(OrderType.StopMarket, order.OrderType);
        }

        [Fact]
        internal void BuildStopLimit_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopLimit();

            // Assert
            Assert.Equal(OrderType.StopLimit, order.OrderType);
        }
    }
}