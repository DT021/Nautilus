﻿//--------------------------------------------------------------------------------------------------
// <copyright file="GuidFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides <see cref="Guid"/>(s) for the system.
    /// </summary>
    [Stateless]
    public sealed class GuidFactory : IGuidFactory
    {
        /// <summary>
        /// Returns a new <see cref="Guid"/>.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        public Guid NewGuid()
        {
            return Guid.NewGuid();
        }
    }
}
