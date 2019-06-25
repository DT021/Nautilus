//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Providers
{
    using System;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Messaging;
    using Nautilus.Network;

    /// <summary>
    /// Provides <see cref="Instrument"/> data to requests.
    /// </summary>
    public sealed class InstrumentProvider : MessageServer<Request, Response>
    {
        private readonly IInstrumentRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public InstrumentProvider(
            IComponentryContainer container,
            IInstrumentRepository repository,
            IMessageSerializer<Request> inboundSerializer,
            IMessageSerializer<Response> outboundSerializer,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                inboundSerializer,
                outboundSerializer,
                host,
                port,
                Guid.NewGuid())
        {
            this.repository = repository;

            this.RegisterHandler<Envelope<InstrumentRequest>>(this.OnMessage);
            this.RegisterHandler<Envelope<InstrumentsRequest>>(this.OnMessage);
        }

        private void OnMessage(Envelope<InstrumentRequest> envelope)
        {
            var request = envelope.Message;
            var query = this.repository.FindInCache(request.Symbol);

            if (query.IsFailure)
            {
                this.SendQueryFailure(query.Message, request.Id, envelope.Sender);
                this.Log.Warning($"{envelope.Message} query failed ({query.Message}).");
                return;
            }

            var instrument = new[] { query.Value };
            var response = new InstrumentResponse(
                instrument,
                request.Id,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(response, envelope.Sender);
        }

        private void OnMessage(Envelope<InstrumentsRequest> envelope)
        {
            var request = envelope.Message;
            var query = this.repository.FindInCache(request.Venue);

            if (query.IsFailure)
            {
                this.SendQueryFailure(query.Message, request.Id, envelope.Sender);
                this.Log.Warning($"{envelope.Message} query failed ({query.Message}).");
                return;
            }

            var instruments = query
                .Value
                .ToArray();

            var response = new InstrumentResponse(
                instruments,
                request.Id,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(response, envelope.Sender);
        }
    }
}
