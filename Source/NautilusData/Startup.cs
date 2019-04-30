﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Nautilus.Common.Configuration;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;
    using Newtonsoft.Json.Linq;
    using Serilog.Events;

    /// <summary>
    /// The main ASP.NET Core Startup class to configure and build the web hosting services.
    /// </summary>
    public class Startup
    {
        private NautilusDatabase dataSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The hosting environment.</param>
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Precondition.NotNull(configuration, nameof(configuration));
            Precondition.NotNull(environment, nameof(environment));

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json");

            this.Configuration = builder.Build();
            this.Environment = environment;
        }

        /// <summary>
        /// Gets the ASP.NET Core configuration.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the ASP.NET Core hosting environment.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public IHostingEnvironment Environment { get; }

        /// <summary>
        /// Configures the ASP.NET Core web hosting services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            Precondition.NotNull(services, nameof(services));

            var config = JObject.Parse(File.ReadAllText("config.json"));

            var logLevel = this.Environment.IsDevelopment()
                ? LogEventLevel.Information
                : ((string)config[ConfigSection.Logging]["logLevel"]).ToEnum<LogEventLevel>();

            var isCompression = (bool)config[ConfigSection.Database]["compression"];
            var compressionCodec = (string)config[ConfigSection.Database]["compressionCodec"];
            var barRollingWindow = (int)config[ConfigSection.Database]["barDataRollingWindow"];

            var configFile = (string)config[ConfigSection.Fix44]["config"];
            var assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var configPath = Path.GetFullPath(Path.Combine(assemblyDirectory, configFile));

            var fixSettings = ConfigReader.LoadConfig(configPath);
            var broker = fixSettings["Brokerage"].ToEnum<Brokerage>();
            var credentials = new FixCredentials(
                account: fixSettings["Account"],
                username: fixSettings["Username"],
                password: fixSettings["Password"]);

            var fixConfig = new FixConfiguration(
                broker,
                configPath,
                credentials,
                fixSettings["InstrumentData"],
                Convert.ToBoolean(fixSettings["SendAccountTag"]),
                Convert.ToBoolean(fixSettings["UpdateInstruments"]));

            var symbolsJArray = (JArray)config[ConfigSection.Symbols];
            var symbolsList = new List<string>();
            foreach (var ccy in symbolsJArray)
            {
                symbolsList.Add(ccy.ToString());
            }

            var symbols = symbolsList
                .Distinct()
                .ToList()
                .AsReadOnly();

            var resolutions = new List<Resolution>
            {
                Resolution.Second,
                Resolution.Minute,
                Resolution.Hour,
            }.ToList().AsReadOnly();

            this.dataSystem = NautilusDatabaseFactory.Create(
                logLevel,
                isCompression,
                compressionCodec,
                fixConfig,
                symbols,
                resolutions,
                barRollingWindow);

            this.dataSystem.Start();
        }

        /// <summary>
        /// Configures the ASP.NET Core web request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        /// <param name="env">The hosting environment.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public void Configure(
            IApplicationBuilder app,
            IApplicationLifetime appLifetime,
            IHostingEnvironment env)
        {
            Precondition.NotNull(app, nameof(app));
            Precondition.NotNull(appLifetime, nameof(appLifetime));
            Precondition.NotNull(env, nameof(env));

            appLifetime.ApplicationStopping.Register(this.OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }

        private void OnShutdown()
        {
            this.dataSystem.Shutdown();
        }
    }
}