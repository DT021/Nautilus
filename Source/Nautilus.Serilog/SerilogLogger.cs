﻿//--------------------------------------------------------------------------------------------------
// <copyright file="SerilogLogger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Serilog
{
    using System;
    using System.Reflection;
    using global::Serilog;
    using global::Serilog.Events;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The <see cref="Serilog"/> adapter.
    /// </summary>
    public class SerilogLogger : ILoggingAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
        /// </summary>
        /// <param name="logLevel">The threshold level for the logger.</param>
        public SerilogLogger(LogEventLevel logLevel)
        {
            SerilogLogFactory.Create(logLevel);

            this.AssemblyVersion = $"Serilog v{Assembly.GetAssembly(typeof(Log)).GetName().Version}";
        }

        /// <summary>
        /// Gets the logging adapters assembly library and version.
        /// </summary>
        public string AssemblyVersion { get; }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="service">The system service.</param>
        /// <param name="message">The log message.</param>
        public void Verbose(Enum service, string message)
        {
            Log.Verbose($"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="service">The system service.</param>
        /// <param name="message">The log message.</param>
        public void Debug(Enum service, string message)
        {
            Log.Debug($"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="service">The system service.</param>
        /// <param name="message">The log message.</param>
        public void Information(Enum service, string message)
        {
            Log.Information($"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="service">The system service.</param>
        /// <param name="message">The log message.</param>
        public void Warning(Enum service, string message)
        {
            Log.Warning($"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// Creates an error log event.
        /// </summary>
        /// <param name="service">The system service.</param>
        /// <param name="message">The log message.</param>
        public void Error(Enum service, string message)
        {
            Log.Error($"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// Creates an error log event.
        /// </summary>
        /// <param name="service">The system service.</param>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        public void Error(Enum service, string message, Exception ex)
        {
            Log.Error(ex, $"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// Creates a fatal log event.
        /// </summary>
        /// <param name="service">The system service.</param>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The fatal exception.</param>
        public void Fatal(Enum service, string message, Exception ex)
        {
            Log.Fatal(ex, $"[{ToOutput(service)}] {message}");
        }

        // TODO: Refactor.
        private static string ToOutput(Enum service)
        {
            const int logStringLength = 10;

            if (service.ToString().Length >= logStringLength)
            {
                return service.ToString();
            }

            var lengthDifference = logStringLength - service.ToString().Length;

            var underscoreAppend = string.Empty;
            var builder = new System.Text.StringBuilder();
            builder.Append(underscoreAppend);

            for (int i = 0; i < lengthDifference; i++)
            {
                builder.Append("_");
            }

            underscoreAppend = builder.ToString();

            return service + underscoreAppend;
        }
    }
}
