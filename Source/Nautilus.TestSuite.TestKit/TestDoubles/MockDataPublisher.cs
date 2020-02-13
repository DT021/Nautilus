//--------------------------------------------------------------------------------------------------
// <copyright file="MockDataPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Network;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockDataPublisher : DataPublisher<string>
    {
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

        private sealed class MockSerializer : ISerializer<string>, IDataSerializer<string>
        {
            public DataEncoding BlobEncoding => DataEncoding.Undefined;

            public DataEncoding ObjectEncoding => DataEncoding.Undefined;

            byte[] IDataSerializer<string>.Serialize(string dataObject)
            {
                return Encoding.UTF8.GetBytes(dataObject);
            }

            public byte[][] Serialize(string[] dataObjects)
            {
                throw new NotImplementedException();
            }

            public byte[] SerializeBlob(byte[][] dataObjectsArray, Dictionary<string, string> metadata)
            {
                throw new NotImplementedException();
            }

            public string[] Deserialize(byte[][] dataBytesArray, object? metadata = null)
            {
                throw new NotImplementedException();
            }

            public string[] DeserializeBlob(byte[] dataBytes)
            {
                throw new NotImplementedException();
            }

            public byte[] Serialize(string message)
            {
                return Encoding.UTF8.GetBytes(message);
            }

            public string Deserialize(byte[] dataBytes)
            {
                return Encoding.UTF8.GetString(dataBytes);
            }
        }
    }
}
