//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionTicket.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers.Base;

    /// <summary>
    /// Represents a valid and unique identifier for executions.
    /// </summary>
    [Immutable]
    public sealed class ExecutionTicket : EntityId<Execution>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionTicket"/> class.
        /// </summary>
        /// <param name="value">The entity id value.</param>
        public ExecutionTicket(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}