//--------------------------------------------------------------------------------------------------
// <copyright file="ICommandSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Core;

    /// <summary>
    /// Provides an interface for command serializers.
    /// </summary>
    public interface ICommandSerializer
    {
        /// <summary>
        /// Serialize the given command.
        /// </summary>
        /// <param name="command">The command to serialize.</param>
        /// <returns>The serialized command.</returns>
        byte[] Serialize(Command command);

        /// <summary>
        /// Deserialize the given command bytes.
        /// </summary>
        /// <param name="commandBytes">The command bytes to deserialize.</param>
        /// <returns>The deserialized command.</returns>
        Command Deserialize(byte[] commandBytes);
    }
}
