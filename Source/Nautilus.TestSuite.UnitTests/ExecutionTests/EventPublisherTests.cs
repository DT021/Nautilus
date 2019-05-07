//--------------------------------------------------------------------------------------------------
// <copyright file="EventPublisherTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.ExecutionTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Events;
    using Nautilus.Execution;
    using Nautilus.MsgPack;
    using Nautilus.Network;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NautilusMQ;
    using NautilusMQ.Tests;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class EventPublisherTests
    {
        private const string ExecutionEvents = "nautilus_execution_events";

        private readonly NetworkAddress localHost = new NetworkAddress("127.0.0.1");
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly IEndpoint testReceiver;

        public EventPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;
            this.testReceiver = new MockMessagingAgent().Endpoint;
        }

        [Fact]
        internal void Test_can_publish_events()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:56601";
            var subscriber = new SubscriberSocket(TestAddress);
            subscriber.Connect(TestAddress);
            subscriber.Subscribe(ExecutionEvents);

            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var rejected = new OrderRejected(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "INVALID_ORDER",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var publisher = new EventPublisher(
                this.setupContainer,
                new MsgPackEventSerializer(),
                this.localHost,
                new Port(56601));

            // Act
            publisher.Endpoint.Send(rejected);
            this.output.WriteLine("Waiting for published events...");

            // var message = subscriber.ReceiveFrameBytes();
            // var @event = serializer.Deserialize(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert.Equal(ExecutionEvents, Encoding.UTF8.GetString(topic));
            // Assert.Equal(rejected, @event);

            // Tear Down
//            publisher.GracefulStop(TimeSpan.FromMilliseconds(1000));
//            subscriber.Unsubscribe(ExecutionEvents);
//            subscriber.Disconnect(TestAddress);
//            subscriber.Dispose();
        }
    }
}
