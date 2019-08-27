﻿//--------------------------------------------------------------------------------------------------
// <copyright file="AccountTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.AggregatesTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class AccountTests
    {
        private readonly IZonedClock clock;

        public AccountTests()
        {
            this.clock = new StubClock();
        }

        [Fact]
        internal void CanSetupAccount_EqualsInitializedValues()
        {
            // Arrange
            // Act
            var account = new Account(
                Brokerage.FXCM,
                "123456789",
                "some username",
                "some password",
                Currency.AUD,
                this.clock.TimeNow());

            // Assert
            Assert.Equal(Brokerage.FXCM, account.Brokerage);
            Assert.Equal("FXCM-123456789", account.Id.ToString());
            Assert.Equal(Currency.AUD, account.Currency);
            Assert.Equal(decimal.Zero, account.CashBalance.Value);
        }

        [Fact]
        internal void Update_MessageCorrect_EqualsUpdatedStatusValues()
        {
            // Arrange
            var account = new Account(
                Brokerage.FXCM,
                "123456789",
                "some username",
                "some password",
                Currency.AUD,
                this.clock.TimeNow());

            var message = new AccountStateEvent(
                AccountId.Create(Brokerage.FXCM, "D123456"),
                Brokerage.FXCM,
                "D123456",
                Currency.AUD,
                Money.Create(150000m, Currency.AUD),
                Money.Create(150000m, Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Zero(Currency.AUD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            account.Apply(message);

            // Assert
            Assert.Equal(150000m, account.CashBalance.Value);
            Assert.Equal(150000m, account.CashStartDay.Value);
            Assert.Equal(0m, account.CashActivityDay.Value);
            Assert.Equal(0m, account.MarginRatio);
            Assert.Equal(0m, account.MarginUsedMaintenance.Value);
            Assert.Equal(0m, account.MarginUsedLiquidation.Value);
            Assert.Equal(string.Empty, account.MarginCallStatus);
        }

        [Fact]
        internal void FreeEquity_MessageCorrect_ReturnsTrue()
        {
            // Arrange
            var account = new Account(
                Brokerage.FXCM,
                "123456789",
                "some username",
                "some password",
                Currency.USD,
                this.clock.TimeNow());

            var message = new AccountStateEvent(
                AccountId.Create(Brokerage.FXCM, "D123456"),
                Brokerage.FXCM,
                "D123456",
                Currency.USD,
                Money.Create(150000m, Currency.AUD),
                Money.Create(150000m, Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Create(2000m, Currency.AUD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            account.Apply(message);

            var result = account.FreeEquity;

            // Assert
            Assert.Equal(150000m - 2000m, result.Value);
        }
    }
}
