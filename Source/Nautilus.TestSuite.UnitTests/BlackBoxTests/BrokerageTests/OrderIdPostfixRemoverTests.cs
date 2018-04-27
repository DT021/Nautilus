﻿// -------------------------------------------------------------------------------------------------
// <copyright file="OrderIdPostfixRemoverTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.BrokerageTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.Brokerage;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class OrderIdPostfixRemoverTests
    {
        [Fact]
        internal void Remove_WithNormalOrderId_ReturnsExpectedOrderId()
        {
            // Arrange
            var orderId = "79812738_AUD_1";

            // Act
            var result = OrderIdPostfixRemover.Remove(orderId);

            // Assert
            Assert.Equal(orderId, result);
        }

        [Fact]
        internal void Remove_WithModifiedOrderId_ReturnsExpectedOrderId()
        {
            // Arrange
            var orderId = "79812738_AUD_1_R2";

            // Act
            var result = OrderIdPostfixRemover.Remove(orderId);

            // Assert
            Assert.Equal("79812738_AUD_1", result);
        }

        [Fact]
        internal void Remove_WithLongModifiedOrderId_ReturnsExpectedOrderId()
        {
            // Arrange
            var orderId = "79812738111_AUDUSD_51_R921";

            // Act
            var result = OrderIdPostfixRemover.Remove(orderId);

            // Assert
            Assert.Equal("79812738111_AUDUSD_51", result);
        }
    }
}