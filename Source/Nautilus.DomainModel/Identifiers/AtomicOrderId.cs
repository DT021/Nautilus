//--------------------------------------------------------------------------------------------------
// <copyright file="AtomicOrderId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Model;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// Represents a valid and unique identifier for atomic orders.
    /// </summary>
    [Immutable]
    public sealed class AtomicOrderId : EntityId<AtomicOrder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicOrderId"/> class.
        /// </summary>
        /// <param name="value">The entity id value.</param>
        public AtomicOrderId(string value)
            : base(value)
        {
            Debug.NotNull(value, nameof(value));
        }
    }
}