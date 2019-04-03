﻿//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubBarBuilder
    {
        public static Bar Build()
        {
            return new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80025m, 5),
                Price.Create(0.79980m, 5),
                Price.Create(0.80008m, 5),
                Quantity.Create(1000),
                StubZonedDateTime.UnixEpoch());
        }

        public static IList<Bar> BuildList()
        {
            return new List<Bar>
            {
                new Bar(Price.Create(0.80000m, 5), Price.Create(0.80010m, 5), Price.Create(0.80000m, 5), Price.Create(0.80008m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(45).ToDuration()),
                new Bar(Price.Create(0.80008m, 5), Price.Create(0.80020m, 5), Price.Create(0.80005m, 5), Price.Create(0.80015m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(40).ToDuration()),
                new Bar(Price.Create(0.80015m, 5), Price.Create(0.80030m, 5), Price.Create(0.80010m, 5), Price.Create(0.80020m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(35).ToDuration()),
                new Bar(Price.Create(0.80020m, 5), Price.Create(0.80030m, 5), Price.Create(0.80000m, 5), Price.Create(0.80010m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(30).ToDuration()),
                new Bar(Price.Create(0.80010m, 5), Price.Create(0.80015m, 5), Price.Create(0.79990m, 5), Price.Create(0.79995m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(25).ToDuration()),
                new Bar(Price.Create(0.79995m, 5), Price.Create(0.80000m, 5), Price.Create(0.79980m, 5), Price.Create(0.79985m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(20).ToDuration()),
                new Bar(Price.Create(0.80000m, 5), Price.Create(0.80010m, 5), Price.Create(0.80000m, 5), Price.Create(0.80008m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(15).ToDuration()),
                new Bar(Price.Create(0.80000m, 5), Price.Create(0.80010m, 5), Price.Create(0.80000m, 5), Price.Create(0.80008m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(10).ToDuration()),
                new Bar(Price.Create(0.80000m, 5), Price.Create(0.80010m, 5), Price.Create(0.80000m, 5), Price.Create(0.80008m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(05).ToDuration()),
                new Bar(Price.Create(0.80000m, 5), Price.Create(0.80015m, 5), Price.Create(0.79990m, 5), Price.Create(0.80005m, 5), Quantity.Create(1000), StubZonedDateTime.UnixEpoch()),
            };
        }
    }
}
