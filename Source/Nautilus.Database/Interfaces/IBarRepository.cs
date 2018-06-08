﻿//--------------------------------------------------------------------------------------------------
// <copyright file="IMarketDataRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Interfaces
{
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Types;
    using NodaTime;

    /// <summary>
    /// The market data repository interface.
    /// </summary>
    public interface IBarRepository
    {
        /// <summary>
        /// Returns the count of bars persisted within the database with the given
        /// <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="symbolBarSpec">The symbol bar data.</param>
        /// <returns>A <see cref="int"/>.</returns>
        long BarsCount(SymbolBarSpec symbolBarSpec);

        /// <summary>
        /// Returns the total count of bars persisted within the database.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        long AllBarsCount();

        /// <summary>
        /// Returns the result of the add bars command.
        /// </summary>
        /// <param name="barData">The market data to add.</param>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        CommandResult Add(BarDataFrame barData);

        /// <summary>
        /// Returns the result of the find bars query.
        /// </summary>
        /// <param name="symbolBarSpec">The bar specification to find.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The to date time.</param>
        /// <returns>A query result of <see cref="BarDataFrame"/>.</returns>
        QueryResult<BarDataFrame> Find(
            SymbolBarSpec symbolBarSpec,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime);

        /// <summary>
        /// Returns the result of the last bars timestamp of the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="symbolBarSpec">The bar specification.</param>
        /// <returns>A query result of <see cref="ZonedDateTime"/>.</returns>
        QueryResult<ZonedDateTime> LastBarTimestamp(SymbolBarSpec symbolBarSpec);
    }
}
