﻿//--------------------------------------------------------------
// <copyright file="BarSpecification.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="BarSpecification"/> class. Represents a bar profile being a time frame and
    /// period.
    /// </summary>
    [Immutable]
    public sealed class BarSpecification : ValueObject<BarSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarSpecification"/> class.
        /// </summary>
        /// <param name="resolution">The bar time frame.</param>
        /// <param name="period">The bar period.</param>
        /// <exception cref="ValidationException">Throws if the period is zero or negative.</exception>
        public BarSpecification(BarResolution resolution, int period)
        {
            Validate.Int32NotOutOfRange(period, nameof(period), 0, int.MaxValue, RangeEndPoints.Exclusive);

            this.Resolution = resolution;
            this.Period = period;
            this.TimePeriod = this.GetTimePeriod(period);
            this.Duration = this.TimePeriod.ToDuration();
        }

        /// <summary>
        /// Gets the bars time frame.
        /// </summary>
        public BarResolution Resolution { get; }

        /// <summary>
        /// Gets the bar period.
        /// </summary>
        public int Period { get;  }

        /// <summary>
        /// Gets the bar time span.
        /// </summary>
        public Period TimePeriod { get; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public Duration Duration { get; }

        /// <summary>
        /// Returns a value indicating whether this bars time period is one day.
        /// </summary>
        public bool IsOneDayBar => this.Resolution == BarResolution.Day && this.Period == 1;

        /// <summary>
        /// Returns a string representation of the <see cref="BarSpecification"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => $"{this.Resolution}({this.Period})";

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A collection of objects.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[]
                       {
                           this.ToString()
                       };
        }

        private Period GetTimePeriod(int barPeriod)
        {
            Debug.Int32NotOutOfRange(barPeriod, nameof(barPeriod), 0, int.MaxValue, RangeEndPoints.Exclusive);

            switch (this.Resolution)
            {
                case BarResolution.Tick:
                    return NodaTime.Period.Zero;

                case BarResolution.Second:
                    return NodaTime.Period.FromSeconds(barPeriod);

                case BarResolution.Minute:
                    return NodaTime.Period.FromMinutes(barPeriod);

                case BarResolution.Hour:
                    return NodaTime.Period.FromHours(barPeriod);

                case BarResolution.Day:
                    return NodaTime.Period.FromDays(barPeriod);

                case BarResolution.Week:
                    return NodaTime.Period.FromWeeks(barPeriod);

                case BarResolution.Month:
                    return NodaTime.Period.FromMonths(barPeriod);

                default: return NodaTime.Period.Zero;
            }
        }
    }
}
