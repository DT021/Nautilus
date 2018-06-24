// -------------------------------------------------------------------------------------------------
// <copyright file="ProtoBufExtensions.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.ProtoBuf
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using ServiceStack;
    using ServiceStack.Web;
    using global::ProtoBuf;

    public class ProtoBufServiceClient : ServiceClientBase
    {
        public override string Format => "x-protobuf";

        public ProtoBufServiceClient(string baseUri)
        {
            SetBaseUri(baseUri);
        }

        public ProtoBufServiceClient(string syncReplyBaseUri, string asyncOneWayBaseUri)
            : base(syncReplyBaseUri, asyncOneWayBaseUri) { }

        public override void SerializeToStream(IRequest requestContext, object request, Stream stream)
        {
            try
            {
                Serializer.NonGeneric.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                throw new SerializationException("ProtoBufServiceClient: Error serializing: " + ex.Message, ex);
            }
        }

        public override T DeserializeFromStream<T>(Stream stream)
        {
            try
            {
                return Serializer.Deserialize<T>(stream);
            }
            catch (Exception ex)
            {
                throw new SerializationException("ProtoBufServiceClient: Error deserializing: " + ex.Message, ex);
            }
        }

        public override string ContentType => MimeTypes.ProtoBuf;

        public override StreamDeserializerDelegate StreamDeserializer => Deserialize;

        private static object Deserialize(Type type, Stream source)
        {
            try
            {
                return Serializer.NonGeneric.Deserialize(type, source);
            }
            catch (Exception ex)
            {
                throw new SerializationException("ProtoBufServiceClient: Error deserializing: " + ex.Message, ex);
            }
        }
    }
}
