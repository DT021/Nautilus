// -------------------------------------------------------------------------------------------------
// <copyright file="Throttler{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Factories;
    using NautilusMQ;
    using NodaTime;

    /// <summary>
    /// Provides a message throttling component.
    /// </summary>
    /// <typeparam name="T">The message type to throttle.</typeparam>
    [PerformanceOptimized]
    public sealed class Throttler<T> : ComponentBase
    {
        private readonly IEndpoint receiver;
        private readonly TimeSpan interval;
        private readonly int limit;
        private readonly Queue<T> queue;

        private bool isIdle;
        private int vouchers;
        private int totalCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="Throttler{T}"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="receiver">The receiver service.</param>
        /// <param name="interval">The throttle timer interval.</param>
        /// <param name="limit">The message limit per second.</param>
        public Throttler(
            IComponentryContainer container,
            NautilusService serviceContext,
            IEndpoint receiver,
            Duration interval,
            int limit)
            : base(
                serviceContext,
                LabelFactory.Create(nameof(Throttler<T>)),
                container)
        {
            Precondition.NotDefault(interval, nameof(interval));
            Precondition.PositiveInt32(limit, nameof(limit));

            this.receiver = receiver;
            this.interval = interval.ToTimeSpan();
            this.limit = limit;
            this.queue = new Queue<T>();

            this.isIdle = true;
            this.vouchers = limit;
            this.totalCount = 0;

            this.RegisterHandler<T>(this.OnMessage);
            this.RegisterHandler<TimeSpan>(this.OnMessage);
        }

        private void OnMessage(T message)
        {
            this.queue.Enqueue(message);

            this.totalCount++;
            this.ProcessQueue();
        }

        private void OnMessage(TimeSpan message)
        {
            this.vouchers = this.limit;

            if (this.queue.Count <= 0)
            {
                this.isIdle = true;
                this.Log.Debug("Is Idle.");

                return;
            }

            // this.RunTimer().PipeTo(this.Endpoint);
            if (this.isIdle)
            {
                this.isIdle = false;
                this.Log.Debug("Is Active.");
            }

            this.ProcessQueue();
        }

        private void ProcessQueue()
        {
            if (this.isIdle)
            {
                // this.RunTimer().PipeTo(this.Self);
                this.isIdle = false;
                this.Log.Debug("Is Active.");
            }

            while (this.vouchers > 0 & this.queue.Count > 0)
            {
                var message = this.queue.Dequeue();

                if (message == null)
                {
                    continue;  // Cannot send a null message.
                }

                this.receiver.Send(message);
                this.vouchers--;

                this.Log.Debug($"Sent message {message} (total_count={this.totalCount}).");
            }

            if (this.vouchers <= 0 & this.queue.Count > 0)
            {
                // At message limit.
                this.Log.Debug($"At message limit of {this.limit} per {this.interval} (queueing_count={this.queue.Count}).");
            }
        }

        private Task<TimeSpan> RunTimer()
        {
            Task.Delay(this.interval).Wait();

            return Task.FromResult(this.interval);
        }
    }
}