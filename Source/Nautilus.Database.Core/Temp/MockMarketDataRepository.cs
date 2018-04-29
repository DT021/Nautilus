﻿namespace Nautilus.Database.Core.Temp
{
    using NautechSystems.CSharp.CQS;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    public class MockMarketDataRepository : IMarketDataRepository
    {
        public long BarsCount(SymbolBarData symbolBarData)
        {
            throw new System.NotImplementedException();
        }

        public long AllBarsCount()
        {
            throw new System.NotImplementedException();
        }

        public CommandResult Add(MarketDataFrame marketData)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<ZonedDateTime> LastBarTimestamp(SymbolBarData barSpec)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<MarketDataFrame> Find(BarSpecification barSpec, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            throw new System.NotImplementedException();
        }
    }
}