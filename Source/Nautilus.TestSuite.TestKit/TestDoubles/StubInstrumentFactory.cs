﻿//--------------------------------------------------------------------------------------------------
// <copyright file="StubInstrumentFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubInstrumentFactory
    {
        public static Instrument AUDUSD()
        {
            var symbol = new Symbol($"AUDUSD", Venue.FXCM);

            var instrument = new Instrument(
                    symbol,
                    new InstrumentId(symbol.ToString()),
                    new BrokerSymbol("AUD/USD"),
                    CurrencyCode.AUD,
                    SecurityType.Forex,
                    5,
                    0.00001m,
                    1,
                    5,
                    1,
                    0,
                    0,
                    0,
                    0,
                    1,
                    50000000,
                    9,
                    1,
                    1,
                    StubZonedDateTime.UnixEpoch());

            return instrument;
        }

        public static Instrument EURUSD()
        {
            var symbol = new Symbol($"EURUSD", Venue.FXCM);

            var instrument = new Instrument(
                    symbol,
                    new InstrumentId(symbol.ToString()),
                    new BrokerSymbol("EUR/USD"),
                    CurrencyCode.EUR,
                    SecurityType.Forex,
                    5,
                    0.00001m,
                    1,
                    5,
                    1,
                    0,
                    0,
                    0,
                    0,
                    1,
                    50000000,
                    9,
                    1,
                    1,
                    StubZonedDateTime.UnixEpoch());

            return instrument;
        }

        public static Instrument USDJPY()
        {
            var symbol = new Symbol("USDJPY", Venue.FXCM);

            var instrument = new Instrument(
                   symbol,
                   new InstrumentId(symbol.ToString()),
                   new BrokerSymbol("USD/JPY"),
                   CurrencyCode.JPY,
                   SecurityType.Forex,
                   3,
                   0.001m,
                   0.1m,
                   5,
                   1,
                   0,
                   0,
                   0,
                   0,
                   1,
                   50000000,
                   10,
                   1,
                   1,
                   StubZonedDateTime.UnixEpoch());

            return instrument;
        }
    }
}
