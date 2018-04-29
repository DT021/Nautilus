﻿//--------------------------------------------------------------
// <copyright file="AtomicOrderPacketTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests.OrderTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class AtomicOrderPacketTests
    {
        [Fact]
        internal void CanZonedDateTimeiate_NewZonedDateTimeiation_ReturnsCorrectDefaultValues()
        {
            // Arrange
            var orderPacket = new AtomicOrderPacket(
                new Symbol("AUDUSD", Exchange.FXCM),
                new TradeType("TestTrade"),
                new List<AtomicOrder>(),
                new EntityId("StubOrderPacket"),
                StubDateTime.Now());

            // Act
            var result1 = orderPacket.Orders.Count;
            var result2 = orderPacket.OrderIdList.Count;
            var result3 = orderPacket.OrderPacketId;

            // Assert
            Assert.Equal(0, result1);
            Assert.Equal(0, result2);
            Assert.Equal("StubOrderPacket", result3.ToString());
        }

        [Fact]
        internal void AddOrder_NewZonedDateTimeiationELSOrderAdded_ReturnsExpectedResults()
        {
            // Arrange
            var atomicOrders = new List<AtomicOrder> { StubAtomicOrderBuilder.Build() };

            // Act
            var orderPacket = new AtomicOrderPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new EntityId("NONE"),
                StubDateTime.Now());

            // Assert
            Assert.Equal(1, orderPacket.Orders.Count);
            Assert.Equal(3, orderPacket.OrderIdList.Count);
            Assert.Equal(atomicOrders[0].EntryOrder, orderPacket.Orders[0].EntryOrder);
            Assert.Equal(atomicOrders[0].StopLossOrder, orderPacket.Orders[0].StopLossOrder);
            Assert.Equal(atomicOrders[0].ProfitTargetOrder, orderPacket.Orders[0].ProfitTargetOrder);
        }

        [Fact]
        internal void Label_NewZonedDateTimeiationWithOneELSOrder_ReturnsCorrectLabel()
        {
            // Arrange
            var orderPacket = StubOrderPacketBuilder.Build();

            // Act
            var result = orderPacket.OrderPacketId;

            // Assert
            Assert.Equal("StubOrderPacket", result.ToString());
        }

        [Fact]
        internal void OrderIdList_ThreeUnits_ReturnsCorrectOrderList()
        {
            // Arrange
            var orderPacket = StubOrderPacketBuilder.ThreeUnitsAndExpireTime(StubDateTime.Now() + Period.FromMinutes(5).ToDuration());

            // Act
            var expectedList = new List<EntityId>
                                   {
                                       new EntityId("EntryOrderId1"),
                                       new EntityId("StoplossOrderId1"),
                                       new EntityId("ProfitTargetOrderId1"),
                                       new EntityId("EntryOrderId2"),
                                       new EntityId("StoplossOrderId2"),
                                       new EntityId("ProfitTargetOrderId2"),
                                       new EntityId("EntryOrderId3"),
                                       new EntityId("StoplossOrderId3")
                                   };

            var result = orderPacket.OrderIdList;


            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        internal void OrderIdList_NewOrderPacket_ReturnsEmptyListOfStrings()
        {
            // Arrange
            var orderPacket = new AtomicOrderPacket(
                new Symbol("AUDUSD", Exchange.FXCM),
                new TradeType("TestTrade"),
                new List<AtomicOrder>(),
                new EntityId("NONE"),
                StubDateTime.Now());

            // Act
            var result = orderPacket.OrderIdList.Count;

            // Assert
            Assert.Equal(0, result);
        }
    }
}