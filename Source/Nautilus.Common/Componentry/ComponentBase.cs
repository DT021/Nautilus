﻿//--------------------------------------------------------------------------------------------------
// <copyright file="ComponentBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using NodaTime;

    /// <summary>
    /// The base class for all system components.
    /// </summary>
    public abstract class ComponentBase : MessagingAgent
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly CommandHandler commandHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase"/> class.
        /// </summary>
        /// <param name="serviceContext">The components service context.</param>
        /// <param name="container">The components componentry container.</param>
        protected ComponentBase(NautilusService serviceContext, IComponentryContainer container)
        {
            this.Name = new Label(this.GetType().Name);
            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;
            this.commandHandler = new CommandHandler(this.Log);
            this.Log = container.LoggerFactory.Create(serviceContext, this.Name);
            this.StartTime = this.clock.TimeNow();

            this.RegisterHandler<SystemStart>(this.OnMessage);
            this.RegisterHandler<SystemShutdown>(this.OnMessage);
        }

        /// <summary>
        /// Gets the components name label.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        protected Label Name { get; }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        /// Gets the time the component was last started or reset.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        protected ZonedDateTime StartTime { get; }

        /// <summary>
        /// Starts the component.
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Stops the component.
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Returns the current time of the system clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        protected ZonedDateTime TimeNow() => this.clock.TimeNow();

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid() => this.guidFactory.NewGuid();

        /// <summary>
        /// Passes the given <see cref="Action"/> to the <see cref="commandHandler"/> for execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        protected void Execute(Action action)
        {
            this.commandHandler.Execute(action);
        }

        /// <summary>
        /// Passes the given <see cref="Action"/> to the <see cref="commandHandler"/> for execution.
        /// </summary>
        /// <typeparam name="T">The exception type.</typeparam>
        /// <param name="action">The action to execute.</param>
        protected void Execute<T>(Action action)
            where T : Exception
        {
            this.commandHandler.Execute<T>(action);
        }

        private void OnMessage(SystemStart message)
        {
            this.Start();
        }

        private void OnMessage(SystemShutdown message)
        {
            this.Stop();
        }
    }
}
