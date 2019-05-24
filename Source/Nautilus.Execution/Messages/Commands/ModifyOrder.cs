﻿//--------------------------------------------------------------------------------------------------
// <copyright file="ModifyOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command to modify an order.
    /// </summary>
    [Immutable]
    public sealed class ModifyOrder : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyOrder"/> class.
        /// </summary>
        /// <param name="order">The commands order to modify.</param>
        /// <param name="modifiedPrice">The modified price.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public ModifyOrder(
            Order order,
            Price modifiedPrice,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                typeof(ModifyOrder),
                commandId,
                commandTimestamp)
        {
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.Order = order;
            this.ModifiedPrice = modifiedPrice;
        }

        /// <summary>
        /// Gets the commands order.
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Gets the commands modified order price.
        /// </summary>
        public Price ModifiedPrice { get; }
    }
}
