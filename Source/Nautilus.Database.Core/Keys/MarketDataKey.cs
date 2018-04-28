﻿//--------------------------------------------------------------
// <copyright file="MarketDataKey.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System;
using NautechSystems.CSharp.Annotations;
using NautechSystems.CSharp.Validation;

namespace Nautilus.Database.Core.Keys
{
    /// <summary>
    /// Represents a strongly typed Redis Key based on the given market data specification.
    /// </summary>
    [Immutable]
    public struct MarketDataKey : IEquatable<MarketDataKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataKey"/> struct. The bar period
        /// must be 1 for a valid key.
        /// </summary>
        /// <param name="barSpec">The bar specification the key is based on.</param>
        /// <param name="dateKey">The date key the key is based on.</param>
        /// <exception cref="ValidationException">Throws if the bar period != 1.</exception>
        public MarketDataKey(BarSpecification barSpec, DateKey dateKey)
        {
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.NotDefault(dateKey, nameof(dateKey));
            Validate.EqualTo(1, nameof(barSpec.Period), barSpec.Period);

            this.BarSpecification = barSpec;
            this.DateKey = dateKey;
        }

        /// <summary>
        /// Gets the <see cref="MarketDataKey"/>(s) bar specification.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets the <see cref="MarketDataKey"/>(s) date key.
        /// </summary>
        public DateKey DateKey { get; }

        /// <summary>
        /// Returns a value indicating whether this entity is equal to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals([CanBeNull] object other)
        {
            return other is MarketDataKey key && this.Equals(key);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="MarketDataKey"/> is equal to the given
        /// <see cref="MarketDataKey"/>.
        /// </summary>
        /// <param name="other">The other <see cref="MarketDataKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(MarketDataKey other)
        {
            return this.BarSpecification.Equals(other.BarSpecification) &&
                   this.DateKey.Equals(other.DateKey);
        }

        /// <summary>
        /// Returns the hash code of the <see cref="MarketDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return this.BarSpecification.GetHashCode() +
                   this.DateKey.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="MarketDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() =>
            $"market_data" +
            $":{this.BarSpecification.Exchange.ToString().ToLower()}" +
            $":{this.BarSpecification.Symbol.ToLower()}" +
            $":{this.BarSpecification.Resolution.ToString().ToLower()}" +
            $":{this.BarSpecification.QuoteType.ToString().ToLower()}" +
            $":{this.DateKey}";
    }
}