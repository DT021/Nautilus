﻿//--------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using Nautilus.Network;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TypeExtensionsTests
    {
        [Fact]
        internal void NameFormatted_FromReferenceType_ReturnsExpectedName()
        {
            // Arrange
            // Act
            var result = typeof(Console).NameFormatted();

            // Assert
            Assert.Equal("Console", result);
        }

        [Fact]
        internal void NameFormatted_FromSingleGenericType_ReturnsExpectedName()
        {
            // Arrange
            // Act
            var result1 = typeof(Envelope<Message>).Name;
            var result2 = typeof(Envelope<Message>).NameFormatted();

            // Assert
            Assert.Equal("Envelope`1", result1);
            Assert.Equal("Envelope<Message>", result2);
        }

        [Fact]
        internal void NameFormatted_FromMultipleGenericType_ReturnsExpectedName()
        {
            // Arrange
            // Act
            var result1 = typeof(MessageServer<Request, Response>).Name;
            var result2 = typeof(MessageServer<Request, Response>).NameFormatted();

            // Assert
            Assert.Equal("MessageServer`2", result1);
            Assert.Equal("MessageServer<Request,Response>", result2);
        }
    }
}
