//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessage.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Mocks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Types;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockMessage : Message
    {
        public MockMessage(
            string payload,
            Guid id,
            ZonedDateTime timestamp)
            : base(typeof(MockMessage), id, timestamp)
        {
            this.Payload = payload;
        }

        public string Payload { get; }
    }
}