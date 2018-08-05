//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusExecutorService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor.Service
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using ServiceStack;

    /// <summary>
    /// Provides a REST API for the <see cref="NautilusExecutor"/> system.
    /// </summary>
    [Immutable]
    public sealed class NautilusExecutorService : Service
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusExecutorService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        public NautilusExecutorService(IComponentryContainer setupContainer)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));

            this.clock = setupContainer.Clock;
            this.guidFactory = setupContainer.GuidFactory;
            this.logger = setupContainer.LoggerFactory.Create(
                NautilusService.Execution,
                LabelFactory.Component(nameof(NautilusExecutorService)));
        }
    }
}