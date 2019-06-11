//--------------------------------------------------------------------------------------------------
// <copyright file="MockDataPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.Network;

    /// <summary>
    /// Provides a mock publisher for testing.
    /// </summary>
    public sealed class MockDataPublisher : DataPublisher<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockDataPublisher"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public MockDataPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                dataBusAdapter,
                new MockSerializer(),
                host,
                port,
                Guid.NewGuid())
        {
            this.RegisterHandler<(string, string)>(this.OnMessage);
        }

        private void OnMessage((string Topic, string Message) toPublish)
        {
            this.Publish(toPublish.Topic, toPublish.Message);
        }

        private sealed class MockSerializer : ISerializer<string>
        {
            public byte[] Serialize(string message)
            {
                return Encoding.UTF8.GetBytes(message);
            }

            public string Deserialize(byte[] bytes)
            {
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
