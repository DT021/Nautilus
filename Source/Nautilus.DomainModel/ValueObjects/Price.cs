﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Price.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System.Globalization;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Primitives;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a positive financial market price.
    /// </summary>
    [Immutable]
    public sealed class Price : PositiveDecimal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Price"/> class.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <param name="decimalPrecision">The price decimal precision.</param>
        private Price(decimal value, int decimalPrecision)
            : base(value)
        {
            Debug.PositiveDecimal(value, nameof(value));
            Debug.NotNegativeInt32(decimalPrecision, nameof(decimalPrecision));
            Debug.True(value.GetDecimalPlaces() <= decimalPrecision, nameof(decimalPrecision));

            this.DecimalPrecision = decimalPrecision;
        }

        /// <summary>
        /// Gets the prices number of decimal places.
        /// </summary>
        public int DecimalPrecision { get; }

        /// <summary>
        /// Returns a new <see cref="Price"/> with the given value and decimal places.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <param name="decimalPrecision">The price decimal precision.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public static Price Create(decimal value, int decimalPrecision)
        {
            return new Price(value, decimalPrecision);
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> as the result of the sum of this price value and the
        /// given price value.
        /// </summary>
        /// <param name="other">The other price.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public Price Add(Price other)
        {
            Debug.NotNull(other, nameof(other));

            return new Price(this.Value + other.Value, this.DecimalPrecision);
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> as the result of the given <see cref="Price"/>
        /// subtracted from this <see cref="Price"/> (cannot return a <see cref="Price"/>
        /// with a negative value).
        /// </summary>
        /// <param name="other">The other price.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public Price Subtract(Price other)
        {
            Debug.NotNull(other, nameof(other));

            return new Price(this.Value - other.Value, this.DecimalPrecision);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Price"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Value.ToString($"F{this.DecimalPrecision}", CultureInfo.InvariantCulture)}";
    }
}
