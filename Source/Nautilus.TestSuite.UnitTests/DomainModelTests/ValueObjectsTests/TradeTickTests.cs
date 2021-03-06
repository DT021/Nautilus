﻿//--------------------------------------------------------------------------------------------------
// <copyright file="TickTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TradeTickTests
    {
        private readonly Symbol symbol;

        public TradeTickTests()
        {
            // Fixture Setup
            this.symbol = new Symbol("AUD/USD", new Venue("FXCM"));
        }

        [Fact]
        internal void InitializedTick_WithZonedDateTime_HasExpectedProperties()
        {
            // Arrange
            // Act
            var tick = new TradeTick(
                this.symbol,
                Price.Create(1.00000m),
                Quantity.Create(10000),
                Maker.Buyer,
                new MatchId("123456789"),
                StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("AUD/USD", tick.Symbol.Code);
            Assert.Equal(decimal.One, tick.Price.Value);
            Assert.Equal(10000m, tick.Size.Value);
            Assert.Equal(1970, tick.Timestamp.Year);
        }

        [Fact]
        internal void InitializedTick_WithUnixTimeMs_HasExpectedProperties()
        {
            // Arrange
            // Act
            var tick = new TradeTick(
                this.symbol,
                Price.Create(1.00000m),
                Quantity.Create(10000),
                Maker.Buyer,
                new MatchId("123456789"),
                1000);

            // Assert
            Assert.Equal("AUD/USD", tick.Symbol.Code);
            Assert.Equal(decimal.One, tick.Price.Value);
            Assert.Equal(10000m, tick.Size.Value);
            Assert.Equal(1970, tick.Timestamp.Year);
        }

        [Theory]
        [InlineData(1.00000, 1.00000, 0, true)]
        [InlineData(1.00001, 1.00000, 0, false)]
        [InlineData(1.00000, 1.00000, 1, false)]
        internal void Equals_VariousValues_ReturnsExpectedResult(
            decimal price1,
            decimal price2,
            int millisecondsOffset,
            bool expected)
        {
            // Arrange
            var tick1 = new TradeTick(
                this.symbol,
                Price.Create(price1),
                Quantity.Create(10000),
                Maker.Buyer,
                new MatchId("123456789"),
                StubZonedDateTime.UnixEpoch());

            var tick2 = new TradeTick(
                this.symbol,
                Price.Create(price2),
                Quantity.Create(10000),
                Maker.Buyer,
                new MatchId("123456789"),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(millisecondsOffset));

            // Act
            var result1 = tick1.Equals(tick2);
            var result2 = tick1 == tick2;

            // Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }

        [Fact]
        internal void FromString_WithValidString_ReturnsExpectedTick()
        {
            // Arrange
            var tick = new TradeTick(
                this.symbol,
                Price.Create(1.00000m),
                Quantity.Create(10000),
                Maker.Buyer,
                new MatchId("123456789"),
                StubZonedDateTime.UnixEpoch());

            // Act
            var tickString = tick.ToSerializableString();
            var result = TradeTick.FromSerializableString(this.symbol, tickString);

            // Assert
            Assert.Equal(tick, result);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            // Act
            var tick = new TradeTick(
                this.symbol,
                Price.Create(1.00000m),
                Quantity.Create(50000),
                Maker.Buyer,
                new MatchId("123456789"),
                StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("AUD/USD.FXCM,1.00000,50000,Buyer,123456789,1970-01-01T00:00:00.000Z", tick.ToString());
        }

        [Fact]
        internal void ToSerializableString_ReturnsExpectedString()
        {
            // Arrange
            // Act
            var tick = new TradeTick(
                this.symbol,
                Price.Create(1.00000m),
                Quantity.Create(50000),
                Maker.Buyer,
                new MatchId("123456789"),
                StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("1.00000,50000,Buyer,123456789,0", tick.ToSerializableString());
        }
    }
}
