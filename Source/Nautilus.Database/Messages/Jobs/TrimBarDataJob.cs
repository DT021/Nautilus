﻿// -------------------------------------------------------------------------------------------------
// <copyright file="TrimBarData.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages.Jobs
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// A command message representing an instruction to trim the bar data keys held in the database
    /// to be equal to the size of the given rolling window.
    /// </summary>
    [Immutable]
    public class TrimBarDataJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrimBarDataJob"/> class.
        /// </summary>
        public TrimBarDataJob()
        {
        }
    }
}