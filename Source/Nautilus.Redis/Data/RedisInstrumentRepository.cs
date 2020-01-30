﻿// -------------------------------------------------------------------------------------------------
// <copyright file="RedisInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Redis.Data.Builders;
    using NodaTime;
    using StackExchange.Redis;
    using Venue = Nautilus.DomainModel.Identifiers.Venue;

    /// <summary>
    /// Provides a Redis implementation for an instrument repository.
    /// </summary>
    public sealed class RedisInstrumentRepository : IInstrumentRepository
    {
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;
        private readonly IDictionary<Symbol, Instrument> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisInstrumentRepository"/> class.
        /// </summary>
        /// <param name="connection">The redis connection multiplexer.</param>
        public RedisInstrumentRepository(ConnectionMultiplexer connection)
        {
            this.redisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
            this.cache = new Dictionary<Symbol, Instrument>();
        }

        /// <inheritdoc />
        public void ResetCache()
        {
            this.cache.Clear();
        }

        /// <inheritdoc />
        public void CacheAll()
        {
            var keys = this.GetAllKeys();

            foreach (var key in keys)
            {
                var query = this.Read(key);

                if (!query.IsSuccess || this.cache.ContainsKey(query.Value.Symbol))
                {
                    continue; // Did not contain instruct or already cached.
                }

                var instrument = query.Value;
                this.cache.Add(instrument.Symbol, instrument);
            }
        }

        /// <inheritdoc />
        public void Delete(Symbol symbol)
        {
            this.redisDatabase.KeyDelete(KeyProvider.GetInstrumentKey(symbol));
        }

        /// <inheritdoc />
        public void DeleteAll()
        {
            var keys = this.GetAllKeys();

            foreach (var key in keys)
            {
                this.redisDatabase.KeyDelete(key);
            }
        }

        /// <summary>
        /// Returns all instrument keys from the Redis database.
        /// </summary>
        /// <returns>The keys.</returns>
        public IReadOnlyCollection<string> GetAllKeys()
        {
            return this.redisServer.Keys(pattern: KeyProvider.GetInstrumentWildcardKey())
                .Select(k => k.ToString())
                .ToArray();
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public CommandResult Add(Instrument instrument, ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var symbol = instrument.Symbol;

            if (!this.cache.ContainsKey(symbol))
            {
                this.cache.Add(symbol, instrument);
                return this.Write(instrument);
            }

            var instrumentBuilder = new InstrumentBuilder(this.cache[symbol]).Update(instrument);

            if (instrumentBuilder.Changes.Count == 0)
            {
                return CommandResult.Ok($"Instrument {symbol.Value} unchanged in cache");
            }

            var changesString = new StringBuilder();

            foreach (var change in instrumentBuilder.Changes)
            {
                changesString.Append(change);
            }

            var updatedInstrument = instrumentBuilder.Build(timeNow);

            this.cache[symbol] = updatedInstrument;
            this.Write(instrument);

            return CommandResult.Ok($"Updated instrument {symbol.Value}" + changesString);
        }

        /// <inheritdoc />
        public QueryResult<Instrument> FindInCache(Symbol symbol)
        {
            return this.cache.ContainsKey(symbol)
                ? QueryResult<Instrument>.Ok(this.cache[symbol])
                : QueryResult<Instrument>.Fail($"Cannot find instrument {symbol.Value}");
        }

        /// <inheritdoc />
        public QueryResult<IEnumerable<Instrument>> FindInCache(Venue venue)
        {
            var instruments = this.cache
                .Where(kvp => kvp.Key.Venue == venue)
                .Select(kvp => kvp.Value)
                .ToList();

            return instruments.Count > 0
                ? QueryResult<IEnumerable<Instrument>>.Ok(instruments)
                : QueryResult<IEnumerable<Instrument>>.Fail($"Cannot find any instrument for {venue}");
        }

        /// <summary>
        /// Returns the instrument from the Redis database matching the given key.
        /// </summary>
        /// <param name="key">The instrument key to read.</param>
        /// <returns>The keys.</returns>
        public QueryResult<Instrument> Read(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            if (!this.redisDatabase.KeyExists(key))
            {
                return QueryResult<Instrument>.Fail($"Cannot find {key}");
            }

            var instrumentDict = this.redisDatabase.HashGetAll(key)
                .ToDictionary(
                    hashEntry => hashEntry.Name.ToString(),
                    hashEntry => hashEntry.Value.ToString());

            var securityType = instrumentDict[nameof(Instrument.SecurityType)].ToEnum<SecurityType>();
            if (securityType == SecurityType.Forex)
            {
                var forexCcy = new ForexInstrument(
                    Symbol.FromString(instrumentDict[nameof(Instrument.Symbol)]),
                    new BrokerSymbol(instrumentDict[nameof(Instrument.BrokerSymbol)]),
                    Convert.ToInt32(instrumentDict[nameof(Instrument.TickPrecision)]),
                    Convert.ToDecimal(instrumentDict[nameof(Instrument.TickSize)]),
                    Convert.ToInt32(instrumentDict[nameof(Instrument.RoundLotSize)]),
                    Convert.ToInt32(instrumentDict[nameof(Instrument.MinStopDistanceEntry)]),
                    Convert.ToInt32(instrumentDict[nameof(Instrument.MinLimitDistanceEntry)]),
                    Convert.ToInt32(instrumentDict[nameof(Instrument.MinStopDistance)]),
                    Convert.ToInt32(instrumentDict[nameof(Instrument.MinLimitDistance)]),
                    Convert.ToInt32(instrumentDict[nameof(Instrument.MinTradeSize)]),
                    Convert.ToInt32(instrumentDict[nameof(Instrument.MaxTradeSize)]),
                    Convert.ToDecimal(instrumentDict[nameof(Instrument.RolloverInterestBuy)]),
                    Convert.ToDecimal(instrumentDict[nameof(Instrument.RolloverInterestSell)]),
                    instrumentDict[nameof(Instrument.Timestamp)].ToZonedDateTimeFromIso());

                return QueryResult<Instrument>.Ok(forexCcy);
            }

            var instrument = new Instrument(
                Symbol.FromString(instrumentDict[nameof(Instrument.Symbol)]),
                new BrokerSymbol(instrumentDict[nameof(Instrument.BrokerSymbol)]),
                instrumentDict[nameof(Instrument.QuoteCurrency)].ToEnum<Currency>(),
                securityType,
                Convert.ToInt32(instrumentDict[nameof(Instrument.TickPrecision)]),
                Convert.ToDecimal(instrumentDict[nameof(Instrument.TickSize)]),
                Convert.ToInt32(instrumentDict[nameof(Instrument.RoundLotSize)]),
                Convert.ToInt32(instrumentDict[nameof(Instrument.MinStopDistanceEntry)]),
                Convert.ToInt32(instrumentDict[nameof(Instrument.MinLimitDistanceEntry)]),
                Convert.ToInt32(instrumentDict[nameof(Instrument.MinStopDistance)]),
                Convert.ToInt32(instrumentDict[nameof(Instrument.MinLimitDistance)]),
                Convert.ToInt32(instrumentDict[nameof(Instrument.MinTradeSize)]),
                Convert.ToInt32(instrumentDict[nameof(Instrument.MaxTradeSize)]),
                Convert.ToDecimal(instrumentDict[nameof(Instrument.RolloverInterestBuy)]),
                Convert.ToDecimal(instrumentDict[nameof(Instrument.RolloverInterestSell)]),
                instrumentDict[nameof(Instrument.Timestamp)].ToZonedDateTimeFromIso());

            return QueryResult<Instrument>.Ok(instrument);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Symbol> GetInstrumentSymbols() => this.cache.Keys.ToArray();

        private CommandResult Write(Instrument instrument)
        {
            if (instrument is ForexInstrument forexCcy)
            {
                var forexHash = new[]
                {
                    new HashEntry(nameof(ForexInstrument.Symbol), forexCcy.Symbol.Value),
                    new HashEntry(nameof(ForexInstrument.BrokerSymbol), forexCcy.BrokerSymbol.Value),
                    new HashEntry(nameof(ForexInstrument.BaseCurrency), forexCcy.BaseCurrency.ToString()),
                    new HashEntry(nameof(ForexInstrument.QuoteCurrency), forexCcy.QuoteCurrency.ToString()),
                    new HashEntry(nameof(ForexInstrument.SecurityType), forexCcy.SecurityType.ToString()),
                    new HashEntry(nameof(ForexInstrument.TickPrecision), forexCcy.TickPrecision),
                    new HashEntry(nameof(ForexInstrument.TickSize), forexCcy.TickSize.ToString(CultureInfo.InvariantCulture)),
                    new HashEntry(nameof(ForexInstrument.RoundLotSize), forexCcy.RoundLotSize),
                    new HashEntry(nameof(ForexInstrument.MinStopDistanceEntry), forexCcy.MinStopDistanceEntry),
                    new HashEntry(nameof(ForexInstrument.MinLimitDistanceEntry), forexCcy.MinLimitDistanceEntry),
                    new HashEntry(nameof(ForexInstrument.MinStopDistance), forexCcy.MinStopDistance),
                    new HashEntry(nameof(ForexInstrument.MinLimitDistance), forexCcy.MinLimitDistance),
                    new HashEntry(nameof(ForexInstrument.MinTradeSize), forexCcy.MinTradeSize),
                    new HashEntry(nameof(ForexInstrument.MaxTradeSize), forexCcy.MaxTradeSize),
                    new HashEntry(nameof(ForexInstrument.RolloverInterestBuy), forexCcy.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture)),
                    new HashEntry(nameof(ForexInstrument.RolloverInterestSell), forexCcy.RolloverInterestSell.ToString(CultureInfo.InvariantCulture)),
                    new HashEntry(nameof(ForexInstrument.Timestamp), forexCcy.Timestamp.ToIsoString()),
                };

                this.redisDatabase.HashSet(KeyProvider.GetInstrumentKey(instrument.Symbol), forexHash);

                return CommandResult.Ok($"Added instrument {instrument}");
            }

            var instrumentHash = new[]
            {
                new HashEntry(nameof(Instrument.Symbol), instrument.Symbol.Value),
                new HashEntry(nameof(Instrument.BrokerSymbol), instrument.BrokerSymbol.Value),
                new HashEntry(nameof(Instrument.QuoteCurrency), instrument.QuoteCurrency.ToString()),
                new HashEntry(nameof(Instrument.SecurityType), instrument.SecurityType.ToString()),
                new HashEntry(nameof(Instrument.TickPrecision), instrument.TickPrecision),
                new HashEntry(nameof(Instrument.TickSize), instrument.TickSize.ToString(CultureInfo.InvariantCulture)),
                new HashEntry(nameof(Instrument.RoundLotSize), instrument.RoundLotSize),
                new HashEntry(nameof(Instrument.MinStopDistanceEntry), instrument.MinStopDistanceEntry),
                new HashEntry(nameof(Instrument.MinLimitDistanceEntry), instrument.MinLimitDistanceEntry),
                new HashEntry(nameof(Instrument.MinStopDistance), instrument.MinStopDistance),
                new HashEntry(nameof(Instrument.MinLimitDistance), instrument.MinLimitDistance),
                new HashEntry(nameof(Instrument.MinTradeSize), instrument.MinTradeSize),
                new HashEntry(nameof(Instrument.MaxTradeSize), instrument.MaxTradeSize),
                new HashEntry(nameof(Instrument.RolloverInterestBuy), instrument.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture)),
                new HashEntry(nameof(Instrument.RolloverInterestSell), instrument.RolloverInterestSell.ToString(CultureInfo.InvariantCulture)),
                new HashEntry(nameof(Instrument.Timestamp), instrument.Timestamp.ToIsoString()),
            };

            this.redisDatabase.HashSet(KeyProvider.GetInstrumentKey(instrument.Symbol), instrumentHash);

            return CommandResult.Ok($"Added instrument {instrument}");
        }
    }
}
