//--------------------------------------------------------------------------------------------------
// <copyright file="FixSessionConnected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using NodaTime;

    /// <summary>
    /// Represents an event where a FIX session has been connected.
    /// </summary>
    [Immutable]
    public sealed class FixSessionConnected : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixSessionConnected"/> class.
        /// </summary>
        /// <param name="broker">The events brokerage connected to.</param>
        /// <param name="sessionId">The events FIX session identifier.</param>
        /// <param name="id">The events identifier (cannot be default).</param>
        /// <param name="timestamp">The events timestamp (cannot be default).</param>
        public FixSessionConnected(
            Brokerage broker,
            string sessionId,
            Guid id,
            ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Broker = broker;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Gets the events brokerage.
        /// </summary>
        public Brokerage Broker { get; }

        /// <summary>
        /// Gets the events session information.
        /// </summary>
        public string SessionId { get; }
    }
}
