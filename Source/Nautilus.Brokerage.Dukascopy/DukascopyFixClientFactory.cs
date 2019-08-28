﻿//--------------------------------------------------------------------------------------------------
// <copyright file="DukascopyFixClientFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.Dukascopy
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Fix;

    /// <summary>
    /// Provides a factory for creating Dukascopy FIX clients.
    /// </summary>
    public static class DukascopyFixClientFactory
    {
        /// <summary>
        /// Creates and returns a new Dukascopy FIX client.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="config">The FIX configuration.</param>
        /// <param name="symbolConverter">The symbol provider.</param>
        /// <returns>The Dukascopy FIX client.</returns>
        public static IFixClient Create(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            FixConfiguration config,
            SymbolConverter symbolConverter)
        {
            return new FixClient(
                container,
                messageBusAdapter,
                config,
                new DukascopyFixMessageHandler(container, symbolConverter),
                new DukascopyFixMessageRouter(
                    container,
                    symbolConverter,
                    config.Credentials.AccountNumber));
        }
    }
}
