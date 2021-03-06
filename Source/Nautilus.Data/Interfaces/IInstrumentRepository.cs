﻿//--------------------------------------------------------------------------------------------------
// <copyright file="IInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Identifiers;

namespace Nautilus.Data.Interfaces
{
    /// <summary>
    /// Provides a repository for accessing <see cref="Instrument"/> data.
    /// </summary>
    public interface IInstrumentRepository : IInstrumentRepositoryReadOnly
    {
        /// <summary>
        /// Clears all instruments from the in-memory cache.
        /// </summary>
        void ResetCache();

        /// <summary>
        /// Adds all persisted instruments to the in-memory cache.
        /// </summary>
        void CacheAll();

        /// <summary>
        /// Deletes all instruments from the database.
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Deletes the instrument of the given symbol from the database.
        /// </summary>
        /// <param name="symbol">The instrument symbol to delete.</param>
        void Delete(Symbol symbol);

        /// <summary>
        /// Updates the given instrument in the database.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        void Add(Instrument instrument);

        /// <summary>
        /// Save a snapshot of the database to disk.
        /// </summary>
        void SnapshotDatabase();
    }
}
