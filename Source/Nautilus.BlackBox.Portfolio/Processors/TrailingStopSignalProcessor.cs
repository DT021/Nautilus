﻿//--------------------------------------------------------------------------------------------------
// <copyright file="TrailingStopSignalProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Processors
{
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Commands;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="TrailingStopSignalProcessor"/> class.
    /// </summary>
    public sealed class TrailingStopSignalProcessor : ComponentBusConnectedBase
    {
        private readonly ITradeBook tradeBook;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrailingStopSignalProcessor"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="tradeBook">The trade book.</param>
        public TrailingStopSignalProcessor(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            Instrument instrument,
            ITradeBook tradeBook)
            : base(
            BlackBoxService.Portfolio,
            LabelFactory.Component(nameof(TrailingStopSignalProcessor), instrument.Symbol),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeBook, nameof(tradeBook));

            this.tradeBook = tradeBook;
        }

        /// <summary>
        /// Processes the given trailing stop signal.
        /// </summary>
        /// <param name="signal">The trailing stop signal.</param>
        /// <exception cref="ValidationException">Throws if the signal is null.</exception>
        public void Process(TrailingStopSignal signal)
        {
            Validate.NotNull(signal, nameof(signal));

            var tradeType = signal.TradeType;
            var trades = this.tradeBook.GetTradesByTradeType(tradeType);

            foreach (var trade in trades)
            {
                if (this.IsValidSignalForTrade(trade, signal))
                {
                    foreach (var tradeUnit in trade.TradeUnits)
                    {
                        foreach (var forUnitStopLoss in signal.ForUnitStopLossPrices)
                        {
                            if (IsValidSignalForStopLoss(tradeUnit, forUnitStopLoss, signal))
                            {
                                var stopLossOrder = tradeUnit.StopLoss;
                                var modifiedOrderId = EntityIdFactory.ModifiedOrderId(
                                    stopLossOrder.Id,
                                    stopLossOrder.IdCount);
                                stopLossOrder.AddModifiedOrderId(modifiedOrderId);

                                var modifyOrder= new ModifyOrder(
                                    stopLossOrder,
                                    forUnitStopLoss.Value,
                                    this.NewGuid(),
                                    this.TimeNow());

                                this.Send(BlackBoxService.Execution, modifyOrder);
                            }
                        }
                    }
                }
            }
        }

        private bool IsValidSignalForTrade(Trade trade, TrailingStopSignal signal)
        {
            Debug.NotNull(trade, nameof(trade));
            Debug.NotNull(signal, nameof(signal));

            if (trade.TradeStatus != TradeStatus.Active)
            {
                this.Log.Debug(
                    $"TrailingStop Signal {signal.ForMarketPosition}-{trade.TradeType} ignored... "
                  + $"(trade not active TradeStatus={trade.TradeStatus})");

                return false;
            }

            if (trade.MarketPosition != signal.ForMarketPosition)
            {
                this.Log.Debug(
                    $"TrailingStop Signal {signal.ForMarketPosition}-{trade.TradeType} ignored... "
                  + $"(signal MarketPosition does not equal trade MarketPosition={trade.MarketPosition})");

                return false;
            }

            if (trade.TradeTimestamp == signal.SignalTimestamp)
            {
                this.Log.Debug(
                    $"TrailingStop Signal {signal.ForMarketPosition}-{trade.TradeType} ignored... "
                  + $"(signal time {signal.SignalTimestamp} coincides with trade timestamp {trade.TradeTimestamp}");

                return false;
            }

            return true;
        }

        private static bool IsValidSignalForStopLoss(
            TradeUnit tradeUnit,
            KeyValuePair<int, Price> forUnitStopLoss,
            TrailingStopSignal signal)
        {
            Debug.NotNull(tradeUnit, nameof(tradeUnit));
            Debug.NotDefault(forUnitStopLoss, nameof(forUnitStopLoss));
            Debug.NotNull(signal, nameof(signal));

            if (tradeUnit.TradeStatus != TradeStatus.Active)
            {
                return false;
            }

            if (forUnitStopLoss.Key != 0
             && tradeUnit.Label.ToString() != "U" + forUnitStopLoss.Key)
            {
                return false;
            }

            if (signal.ForMarketPosition == MarketPosition.Long
             && tradeUnit.StopLoss.Price.Value > forUnitStopLoss.Value)
            {
                return false;
            }

            if (signal.ForMarketPosition == MarketPosition.Short
             && tradeUnit.StopLoss.Price.Value < forUnitStopLoss.Value)
            {
                return false;
            }

            return true;
        }
    }
}
