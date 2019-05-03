﻿//--------------------------------------------------------------------------------------------------
// <copyright file="DataCollectionSchedule.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Orchestration
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Keys;
    using NodaTime;

    /// <summary>
    /// Represents a schedule for data collection.
    /// </summary>
    public sealed class DataCollectionSchedule
    {
        private readonly IsoDayOfWeek collectionDay;
        private readonly int collectionHour;
        private readonly int collectionMinute;
        private readonly ZonedDateTime previousScheduledTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCollectionSchedule"/> class.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <param name="collectionDay">The collection day.</param>
        /// <param name="collectionHour">The collection hour.</param>
        /// <param name="collectionMinute">The collection minute.</param>
        /// <param name="intervalicCollection">The intervalic collection flag.</param>
        /// <param name="intervalMinutes">The interval minutes.</param>
        public DataCollectionSchedule(
            ZonedDateTime timeNow,
            IsoDayOfWeek collectionDay,
            int collectionHour,
            int collectionMinute,
            bool intervalicCollection,
            int intervalMinutes)
        {
            Precondition.NotDefault(collectionDay, nameof(collectionDay));
            Precondition.NotOutOfRangeInt32(collectionHour, 0, 23, nameof(collectionHour));
            Precondition.NotOutOfRangeInt32(collectionMinute, 0, 59, nameof(collectionMinute));
            Precondition.NotNegativeInt32(intervalMinutes, nameof(intervalMinutes));

            this.collectionDay = collectionDay;
            this.collectionHour = collectionHour;
            this.collectionMinute = collectionMinute;

            if (intervalicCollection && intervalMinutes > 0)
            {
                this.IntervalicCollection = true;
                this.IntervalDuration = Duration.FromMinutes(intervalMinutes);
            }

            this.previousScheduledTime = this.SetPreviousScheduledTime(timeNow);
            this.NextCollectionTime = this.SetNextCollectionTime(timeNow);
        }

        /// <summary>
        /// Gets a value indicating whether interval collection is active for this schedule.
        /// </summary>
        public bool IntervalicCollection { get; }

        /// <summary>
        /// Gets the schedules duration of collection intervals.
        /// </summary>
        public OptionVal<Duration> IntervalDuration { get; }

        /// <summary>
        /// Gets the schedules last collected date and time (optional).
        /// </summary>
        public OptionVal<ZonedDateTime> LastCollectedTime { get; private set; }

        /// <summary>
        /// Gets the schedules next major collection date time.
        /// </summary>
        public ZonedDateTime NextCollectionTime { get; private set; }

        /// <summary>
        /// Updates the last collected date time with the given date time.
        /// </summary>
        /// <param name="timeNow">The last collected time.</param>
        public void UpdateLastCollectedTime(ZonedDateTime timeNow)
        {
            Precondition.NotDefault(timeNow, nameof(timeNow));

            this.LastCollectedTime = timeNow;
            this.NextCollectionTime = this.SetNextCollectionTime(timeNow);
        }

        /// <summary>
        /// Returns the time <see cref="Duration"/> to go from the next collection time.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <returns>A <see cref="Duration"/>.</returns>
        public Duration GetNextCollectionTimeToGo(ZonedDateTime timeNow)
        {
            Precondition.NotDefault(timeNow, nameof(timeNow));

            return this.NextCollectionTime - timeNow;
        }

        /// <summary>
        /// Returns a value indicating whether the data is due for collection.
        /// </summary>
        /// <param name="currentTime">The current date and time.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool IsDataDueForCollection(ZonedDateTime currentTime)
        {
            Precondition.NotDefault(currentTime, nameof(currentTime));

            return this.LastCollectedTime.HasNoValue || currentTime.Compare(this.NextCollectionTime) >= 0;
        }

        private ZonedDateTime SetPreviousScheduledTime(ZonedDateTime timeNow)
        {
            var lastCollectionDaysInterval = Duration.FromDays(Math.Abs(timeNow.DayOfWeek - this.collectionDay));
            var dateKey = new DateKey(
                timeNow.Year,
                timeNow.Month,
                timeNow.Day);

            return dateKey.StartOfDay
                   - lastCollectionDaysInterval
                   + Duration.FromHours(this.collectionHour)
                   + Duration.FromMinutes(this.collectionMinute);
        }

        private ZonedDateTime SetNextCollectionTime(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var nextMajorCollectionTime = this.CalculateNextMajorCollectionTime(timeNow);

            // If no intervalic collection to calculate then return the next major collection time.
            if (!this.IntervalicCollection || this.IntervalDuration.HasNoValue)
            {
                return nextMajorCollectionTime;
            }

            var nextIntervalicCollectionTime = this.CalculateNextIntervalicCollectionTime(timeNow);

            return nextIntervalicCollectionTime.IsLessThan(nextMajorCollectionTime)
                 ? nextIntervalicCollectionTime
                 : nextMajorCollectionTime;
        }

        private ZonedDateTime CalculateNextMajorCollectionTime(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var nextCollectionDaysInterval = Duration.FromDays(Math.Abs(timeNow.DayOfWeek - this.collectionDay));
            var dateKey = new DateKey(
                timeNow.Year,
                timeNow.Month,
                timeNow.Day);

            return dateKey.StartOfDay
                   + nextCollectionDaysInterval
                   + Duration.FromHours(this.collectionHour)
                   + Duration.FromMinutes(this.collectionMinute);
        }

        private ZonedDateTime CalculateNextIntervalicCollectionTime(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var intervalDuration = this.IntervalDuration.HasValue
                ? this.IntervalDuration.Value
                : Duration.Zero;

            var nextIntervalTime = this.previousScheduledTime + intervalDuration;

            while (nextIntervalTime.IsLessThanOrEqualTo(timeNow))
            {
                nextIntervalTime = nextIntervalTime + intervalDuration;
            }

            return nextIntervalTime;
        }
    }
}
