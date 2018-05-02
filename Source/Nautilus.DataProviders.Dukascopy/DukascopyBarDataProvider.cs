﻿//--------------------------------------------------------------------------------------------------
// <copyright file="DukascopyBarDataProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DataProviders.Dukascopy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Database.Core.Configuration;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides the <see cref="Dukascopy"/> meta-data.
    /// </summary>
    public class DukascopyBarDataProvider : IBarDataProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DukascopyBarDataProvider"/> class.
        /// </summary>
        /// <param name="config">The market data provider configuration.</param>
        /// <param name="initialFromDateString">The initial from date string.</param>
        /// <param name="collectionOffsetMinutes">The collection offset minutes.</param>
        public DukascopyBarDataProvider(
            MarketDataProviderConfig config,
            string initialFromDateString,
            int collectionOffsetMinutes)
        {
            Validate.NotNull(config, nameof(config));
            Validate.NotNull(initialFromDateString, nameof(initialFromDateString));
            Validate.Int32NotOutOfRange(collectionOffsetMinutes, nameof(collectionOffsetMinutes), 0, int.MaxValue);

            this.SymbolBarDatas = BuildBarSpecifications(config.CurrencyPairs, config.BarResolutions);
            this.DataPath = new DirectoryInfo(config.CsvDataDirectory);
            this.TimestampParsePattern = config.TimestampParsePattern;
            this.VolumeMultiple = config.VolumeMultiple;
            this.IsBarDataCheckOn = config.IsBarDataCheckOn;
        }

        public IReadOnlyCollection<SymbolBarSpec> SymbolBarDatas { get; }

        /// <summary>
        /// Gets the <see cref="Dukascopy"/> CSV data path.
        /// </summary>
        public DirectoryInfo DataPath { get; }

        /// <summary>
        /// Gets the <see cref="Dukascopy"/> <see cref="ZonedDateTime"/> parse pattern.
        /// </summary>
        public string TimestampParsePattern { get; }

        /// <summary>
        /// Gets the <see cref="Dukascopy"/> volume multiple for bars data.
        /// </summary>
        public int VolumeMultiple { get; }

        /// <summary>
        /// Gets a value indicating whether the bar data integrity check is on.
        /// </summary>
        public bool IsBarDataCheckOn { get; }

        /// <summary>
        /// Returns the <see cref="Dukascopy"/> label for the given <see cref="BarResolution"/>.
        /// </summary>
        /// <param name="resolution">The bar resolution.</param>
        /// <returns>The data providers label for the given <see cref="BarResolution"/>.</returns>
        public string GetResolutionLabel(BarResolution resolution)
        {
            switch (resolution)
            {
                case BarResolution.Second:
                    return "Second";

                case BarResolution.Minute:
                    return "Min";

                case BarResolution.Hour:
                    return "Hourly";

                case BarResolution.Day:
                    return "Day";

                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        private static IReadOnlyCollection<SymbolBarSpec> BuildBarSpecifications(
            IReadOnlyCollection<string> currencyPairs,
            IReadOnlyCollection<string> barResolutions)
        {
            var barSpecs = new List<SymbolBarSpec>();

            foreach (var symbol in currencyPairs.Distinct())
            {
                foreach (var resolution in barResolutions)
                {
                    barSpecs.Add(new SymbolBarSpec(new Symbol(symbol, Exchange.Dukascopy), new BarSpecification(BarQuoteType.Bid, resolution.ToEnum<BarResolution>(), 1)));
                    barSpecs.Add(new SymbolBarSpec(new Symbol(symbol, Exchange.Dukascopy), new BarSpecification(BarQuoteType.Ask, resolution.ToEnum<BarResolution>(), 1)));
                }
            }

            return barSpecs;
        }
    }
}