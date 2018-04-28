﻿//--------------------------------------------------------------
// <copyright file="EconomicNewsEventFrame.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NautechSystems.CSharp.Annotations;
using NautechSystems.CSharp.Validation;
using NodaTime;

namespace NautilusDB.Core.Types
{
    [Immutable]
    [Serializable]
    public class EconomicNewsEventFrame
    {
        public EconomicNewsEventFrame(IReadOnlyCollection<EconomicNewsEvent> events)
        {
            Validate.ReadOnlyCollectionNotNullOrEmpty(events, nameof(events));

            this.Events = events;
        }

        public IReadOnlyCollection<Currency> CurrencySymbols =>
            this.Events.Select(e => e.Currency)
                .Distinct()
                .ToList()
                .AsReadOnly();

        /// <summary>
        /// Gets the economic news event frames list of events.
        /// </summary>
        public IReadOnlyCollection<EconomicNewsEvent> Events { get; }

        /// <summary>
        /// Gets the economic news event frames first event time.
        /// </summary>
        public ZonedDateTime StartDateTime => this.Events.First().Time;

        /// <summary>
        /// Gets the economic news event frames last event time.
        /// </summary>
        public ZonedDateTime EndDateTime => this.Events.Last().Time;
    }
}