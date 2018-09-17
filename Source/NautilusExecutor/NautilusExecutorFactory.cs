//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusExecutorFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System.Threading.Tasks;
    using Akka.Actor;
    using global::NautilusExecutor.Build;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common;
    using Nautilus.Common.Build;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Logging;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Validation;
    using Nautilus.Execution;
    using Nautilus.Fix;
    using Nautilus.Messaging.Network;
    using Nautilus.MsgPack;
    using Nautilus.Redis;
    using Nautilus.Serilog;
    using NodaTime;
    using Serilog.Events;
    using ServiceStack.Redis;

    /// <summary>
    /// Provides a factory for creating a <see cref="NautilusExecutor"/> system.
    /// </summary>
    public static class NautilusExecutorFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="NautilusExecutor"/> class.
        /// </summary>
        /// <param name="logLevel">The logger log level threshold.</param>
        /// <param name="fixCredentials">The FIX credentials.</param>
        /// <param name="serviceAddress">The services address.</param>
        /// <param name="commandsPort">The commands port.</param>
        /// <param name="eventsPort">The events port.</param>
        /// <param name="commandsPerSecond">The commands per second throttle limit.</param>
        /// <param name="newOrdersPerSecond">The new orders per second throttle limit.</param>
        /// <returns>The <see cref="NautilusExecutor"/> system.</returns>
        public static NautilusExecutor Create(
            LogEventLevel logLevel,
            FixCredentials fixCredentials,
            NetworkAddress serviceAddress,
            Port commandsPort,
            Port eventsPort,
            int commandsPerSecond,
            int newOrdersPerSecond)
        {
            Validate.NotNull(fixCredentials, nameof(fixCredentials));
            Validate.NotNull(serviceAddress, nameof(serviceAddress));
            Validate.NotNull(commandsPort, nameof(commandsPort));
            Validate.NotNull(eventsPort, nameof(eventsPort));

            var loggingAdapter = new SerilogLogger(logLevel);
            loggingAdapter.Information(NautilusService.Data, $"Starting {nameof(NautilusExecutor)} builder...");
            BuildVersionChecker.Run(loggingAdapter, "NautilusExecutor - Financial Market Execution Service");

            var actorSystem = ActorSystem.Create(nameof(NautilusExecutor));
            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var setupContainer = new ComponentryContainer(
                clock,
                guidFactory,
                new LoggerFactory(loggingAdapter));

            var messagingAdapter = MessagingServiceFactory.Create(
                actorSystem,
                setupContainer,
                new FakeMessageStore());

            var clientManager = new BasicRedisClientManager(
                new[] { RedisConstants.LocalHost },
                new[] { RedisConstants.LocalHost });

            var instrumentRepository = new RedisInstrumentRepository(clientManager);
            instrumentRepository.CacheAll();

            var fixClient = FxcmFixClientFactory.Create(
                setupContainer,
                messagingAdapter,
                fixCredentials);

            var fixGateway = FixGatewayFactory.Create(
                setupContainer,
                instrumentRepository,
                fixClient);

            var switchboard = ExecutionServiceFactory.Create(
                actorSystem,
                setupContainer,
                messagingAdapter,
                fixGateway,
                new MsgPackCommandSerializer(),
                new MsgPackEventSerializer(),
                serviceAddress,
                commandsPort,
                eventsPort,
                commandsPerSecond,
                newOrdersPerSecond);

            var systemController = new SystemController(
                setupContainer,
                actorSystem,
                messagingAdapter,
                switchboard);

            // Allow system to wire up (switchboard to initialize).
            Task.Delay(500).Wait();

            return new NautilusExecutor(
                setupContainer,
                messagingAdapter,
                systemController,
                fixClient);
        }
    }
}
