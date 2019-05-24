﻿//--------------------------------------------------------------------------------------------------
// <copyright file="MessageTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MessageTests
    {
        [Fact]
        internal void Equal_WithDifferentMessagesOfTheSameContent_CanEquateById()
        {
            // Arrange
            var message1 = new SystemStatusResponse(
                new Label("SomeComponent1"),
                Status.Running,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new SystemStatusResponse(
                new Label("SomeComponent2"),
                Status.Running,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result1 = message1 == message2;
            var result2 = message1.Equals(message2);
            var result3 = message1.Equals(message1);

            // Assert
            Assert.False(result1);
            Assert.False(result2);
            Assert.True(result3);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var message = new SystemStatusRequest(guid, StubZonedDateTime.UnixEpoch());

            // Act
            var result = message.ToString();

            // Assert
            Assert.True(result.StartsWith("SystemStatusRequest("));
            Assert.True(result.EndsWith(")"));
        }

        [Fact]
        internal void ToString_WhenOverridden_ReturnsExpectedString()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var message = new SystemStatusResponse(
                new Label("CommandBus"),
                Status.Running,
                guid,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = message.ToString();

            // Assert
            Assert.True(result.StartsWith("SystemStatusResponse("));
            Assert.True(result.EndsWith("-CommandBus=Running"));
        }
    }
}
