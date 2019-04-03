﻿// -------------------------------------------------------------------------------------------------
// <copyright file="BarWrangler.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Wranglers
{
    using System.Collections.Generic;
    using System.Text;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides wrangling operations for <see cref="Bar"/> objects.
    /// </summary>
    [Immutable]
    public static class BarWrangler
    {
        /// <summary>
        /// Parses the given bar string array and returns a list of <see cref="Bar"/>(s).
        /// </summary>
        /// <param name="barsBytes">The bar strings.</param>
        /// <returns>The list of parsed bars.</returns>
        public static List<Bar> ParseBars(byte[][] barsBytes)
        {
            Validate.NotNull(barsBytes, nameof(barsBytes));

            var barData = new List<Bar>();

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < barsBytes.Length; i++)
            {
                barData.Add(Bar.GetFromString(Encoding.UTF8.GetString(barsBytes[i])));
            }

            return barData;
        }

        /// <summary>
        /// Organizes the given bars array into a dictionary of bar lists indexed by a date key.
        /// </summary>
        /// <param name="bars">The bars array.</param>
        /// <returns>The organized dictionary.</returns>
        [PerformanceOptimized]
        public static Dictionary<DateKey, List<Bar>> OrganizeBarsByDay(Bar[] bars)
        {
            Validate.NotNullOrEmpty(bars, nameof(bars));

            var barsDictionary = new Dictionary<DateKey, List<Bar>>();

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < bars.Length; i++)
            {
                var dateKey = new DateKey(bars[i].Timestamp);

                if (!barsDictionary.ContainsKey(dateKey))
                {
                    barsDictionary.Add(dateKey, new List<Bar>());
                }

                barsDictionary[dateKey].Add(bars[i]);
            }

            return barsDictionary;
        }
    }
}
