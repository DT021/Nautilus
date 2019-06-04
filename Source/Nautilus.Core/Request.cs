//--------------------------------------------------------------------------------------------------
// <copyright file="Request.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// The base class for all <see cref="Request"/> messages.
    /// </summary>
    [Immutable]
    public abstract class Request : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="type">The request type.</param>
        /// <param name="id">The request identifier.</param>
        /// <param name="timestamp">The request timestamp.</param>
        protected Request(
            Type type,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                type,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }

        /// <summary>
        /// Gets the requests requester identifier.
        /// </summary>
        public string RequesterId { get; private set; } = string.Empty;

        /// <summary>
        /// Sets the requester identifier to the given requester identifier.
        /// </summary>
        /// <param name="requesterId">The requester identifier.</param>
        public void SetRequesterId(string requesterId)
        {
            this.RequesterId = requesterId;
        }
    }
}
