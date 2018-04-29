﻿//--------------------------------------------------------------
// <copyright file="LabelTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class LabelTests
    {
        [Fact]
        internal void Equals_IdenticalComponentLabels_ReturnsTrue()
        {
            // Arrange
            var label1 = LabelFactory.Component("TimeBarAggregator", new Symbol("AUDUSD", Exchange.LMAX));
            var label2 = LabelFactory.Component("TimeBarAggregator", new Symbol("AUDUSD", Exchange.LMAX));

            // Act
            var result = label1.Equals(label2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        internal void GetHashcode_WithNormalComponentLabel_ReturnsExpectedHashCode()
        {
            // Arrange
            var label = new Label("ExecutionService");

            // Act
            var result = label.GetHashCode();

            // Assert
            Assert.Equal(typeof(int), result.GetType());
        }

        [Fact]
        internal void Equals_WithTheSameValue_ReturnsTrue()
        {
            // Arrange
            var label1 = LabelFactory.Component("Portfolio", new Symbol("AUDUSD", Exchange.LMAX));
            var label2 = LabelFactory.Component("Portfolio", new Symbol("AUDUSD", Exchange.LMAX));

            // Act
            var result1 = label1.Equals(label2);
            var result2 = label1 == label2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        internal void Equals_WithNullObject_ReturnsFalse()
        {
            // Arrange
            var label1 = LabelFactory.Component("Portfolio", new Symbol("AUDUSD", Exchange.LMAX));

            // Act
            var result = label1.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void Equals_WithObjectOfDifferentType_ReturnsFalse()
        {
            // Arrange
            var label1 = LabelFactory.Component("Portfolio", new Symbol("AUDUSD", Exchange.LMAX));
            const string obj = "some_random_object";

            // Act (ignore the warning, this is why the result is false!).
            var result = label1.Equals(obj);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void Equals_WithDifferentComponentLabels_ReturnsFalse()
        {
            // Arrange
            var label1 = new Label("SecurityPortfolio");
            var label2 = new Label("TradeBook");

            // Act
            var result = label1.Equals(label2);

            // Assert
            Assert.False(result);
        }
    }
}