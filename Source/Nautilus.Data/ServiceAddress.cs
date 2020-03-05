//--------------------------------------------------------------------------------------------------
// <copyright file="ServiceAddress.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using Nautilus.Messaging;

    /// <summary>
    /// Provides data service component messaging addresses.
    /// </summary>
    public static class ServiceAddress
    {
        /// <summary>
        /// Gets the <see cref="DataService"/> component messaging address.
        /// </summary>
        public static Address DataService { get; } = new Address(nameof(DataService));

        /// <summary>
        /// Gets the <see cref="Scheduler"/> component messaging address.
        /// </summary>
        public static Address Scheduler { get; } = new Address(nameof(Scheduler));

        /// <summary>
        /// Gets the <see cref="DataGateway"/> component messaging address.
        /// </summary>
        public static Address DataGateway { get; } = new Address(nameof(DataGateway));

        /// <summary>
        /// Gets the <see cref="DataServer"/> component messaging address.
        /// </summary>
        public static Address DataServer { get; } = new Address(nameof(DataServer));

        /// <summary>
        /// Gets the <see cref="DataPublisher"/> component messaging address.
        /// </summary>
        public static Address DataPublisher { get; } = new Address(nameof(DataPublisher));

        /// <summary>
        /// Gets the <see cref="TickPublisher"/> component messaging address.
        /// </summary>
        public static Address TickPublisher { get; } = new Address(nameof(TickPublisher));

        /// <summary>
        /// Gets the <see cref="TickProvider"/> component messaging address.
        /// </summary>
        public static Address TickProvider { get; } = new Address(nameof(TickProvider));

        /// <summary>
        /// Gets the <see cref="TickRepository"/> component messaging address.
        /// </summary>
        public static Address TickRepository { get; } = new Address(nameof(TickRepository));

        /// <summary>
        /// Gets the <see cref="BarAggregationController"/> component messaging address.
        /// </summary>
        public static Address BarAggregationController { get; } = new Address(nameof(BarAggregationController));

        /// <summary>
        /// Gets the <see cref="BarProvider"/> component messaging address.
        /// </summary>
        public static Address BarProvider { get; } = new Address(nameof(BarProvider));

        /// <summary>
        /// Gets the <see cref="BarRepository"/> component messaging address.
        /// </summary>
        public static Address BarRepository { get; } = new Address(nameof(BarRepository));

        /// <summary>
        /// Gets the <see cref="InstrumentProvider"/> component messaging address.
        /// </summary>
        public static Address InstrumentProvider { get; } = new Address(nameof(InstrumentProvider));

        /// <summary>
        /// Gets the <see cref="InstrumentRepository"/> component messaging address.
        /// </summary>
        public static Address InstrumentRepository { get; } = new Address(nameof(InstrumentRepository));
    }
}
