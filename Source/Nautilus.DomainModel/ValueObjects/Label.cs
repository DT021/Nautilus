﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Label.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Primitives;

    /// <summary>
    /// Represents a validated label.
    /// </summary>
    [Immutable]
    public sealed class Label : ValidString<Label>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="value">The label value.</param>
        public Label(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
