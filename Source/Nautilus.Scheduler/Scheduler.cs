﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Scheduler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System.Collections.Specialized;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Scheduler.Commands;
    using Nautilus.Scheduler.Events;
    using Nautilus.Scheduler.Exceptions;
    using Quartz;
    using Quartz.Impl;

    /// <summary>
    /// Provides a system scheduling actor with an internal quartz scheduler which processes Add
    /// and Remove messages.
    /// </summary>
    public sealed class Scheduler : ActorComponentBase
    {
        private readonly IScheduler quartzScheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        public Scheduler(IComponentryContainer container)
            : base(
                NautilusService.Data,
                LabelFactory.Component(nameof(Scheduler)),
                container)
        {
            Validate.NotNull(container, nameof(container));

            var properties = new NameValueCollection
            {
                { "quartz.threadPool.threadCount", "10" },
            };
            this.quartzScheduler = new StdSchedulerFactory(properties).GetScheduler().Result;

            // Setup message handling.
            this.Receive<CreateJob>(this.OnMessage);
            this.Receive<RemoveJob>(this.OnMessage);
            this.Receive<PauseJob>(this.OnMessage);
            this.Receive<ResumeJob>(this.OnMessage);
        }

        /// <summary>
        /// Runs pre-start of the actor component start.
        /// </summary>
        protected override void PreStart()
        {
            base.PreStart();
            this.quartzScheduler.Start();
        }

        /// <summary>
        /// Runs post-stop of the actor component stopping.
        /// </summary>
        protected override void PostStop()
        {
            this.quartzScheduler.Shutdown();
            base.PostStop();
        }

        private void OnMessage(CreateJob message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                var receiver = message.Receiver;
                var job = Job.CreateBuilderWithData(
                        receiver,
                        message.Message)
                    .WithIdentity(message.Trigger.JobKey)
                    .Build();

                this.quartzScheduler.ScheduleJob(job, message.Trigger);

                receiver.Send(new JobCreated(
                    message.Trigger.JobKey,
                    message.Trigger.Key,
                    message.Message,
                    this.NewGuid(),
                    this.TimeNow()));
            });
        }

        private void OnMessage(PauseJob message)
        {
            Debug.NotNull(message, nameof(message));

            try
            {
                var paused = this.quartzScheduler.PauseJob(message.JobKey);
                if (paused.IsCompletedSuccessfully)
                {
                    this.Log.Information($"Job paused successfully {message.JobKey}.");
                }
                else
                {
                    this.Log.Warning($"Job pause failed for {message.JobKey}.");
                }
            }
            catch (JobNotFoundException ex)
            {
                this.Log.Error($"Job pause failed with error for {message.JobKey}.", ex);
            }
        }

        private void OnMessage(ResumeJob message)
        {
            Debug.NotNull(message, nameof(message));

            try
            {
                var resume = this.quartzScheduler.ResumeJob(message.JobKey);
                if (resume.IsCompletedSuccessfully)
                {
                    this.Log.Information($"Job resumed successfully {message.JobKey}.");
                }
                else
                {
                    this.Log.Error($"Job resume failed for {message.JobKey}.");
                }
            }
            catch (JobNotFoundException ex)
            {
                this.Log.Error($"Job resume failed with error for {message.JobKey}.", ex);
            }
        }

        private void OnMessage(RemoveJob message)
        {
            Debug.NotNull(message, nameof(message));

            try
            {
                var deleted = this.quartzScheduler.DeleteJob(message.JobKey);
                if (deleted.Result)
                {
                    this.Log.Information($"Job removed Key={message.JobKey}, TriggerKey={message.TriggerKey}");
                }
                else
                {
                    this.Log.Error($"Remove job failed Key={message.JobKey}, TriggerKey={message.TriggerKey}, Reason=JobNotFound");
                }
            }
            catch (JobNotFoundException ex)
            {
                this.Log.Error($"Remove job failed Key={message.JobKey}, TriggerKey={message.TriggerKey}, Reason={ex.Message}");
            }
        }
    }
}
