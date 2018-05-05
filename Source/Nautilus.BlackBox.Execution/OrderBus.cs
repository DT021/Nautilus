﻿//--------------------------------------------------------------------------------------------------
// <copyright file="OrderBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Execution
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The immutable sealed <see cref="OrderBus"/> class.
    /// </summary>
    [Immutable]
    public sealed class OrderBus : ActorComponentBusConnectedBase
    {
        private IBrokerageGateway brokerageGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBus"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public OrderBus(
            ComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(
            BlackBoxService.Execution,
            new Label(nameof(OrderBus)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.SetupCommandMessageHandling();
            this.SetupServiceMessageHandling();
        }

        private void SetupCommandMessageHandling()
        {
            this.Receive<SubmitTrade>(msg => this.OnMessage(msg));
            this.Receive<CancelOrder>(msg => this.OnMessage(msg));
            this.Receive<ModifyStopLoss>(msg => this.OnMessage(msg));
            this.Receive<ClosePosition>(msg => this.OnMessage(msg));
        }

        private void SetupServiceMessageHandling()
        {
            this.Receive<InitializeBrokerageGateway>(msg => this.OnMessage(msg));
        }

        private void OnMessage(InitializeBrokerageGateway message)
        {
            Debug.NotNull(message, nameof(message));

            this.brokerageGateway = message.BrokerageGateway;
            this.Log.Information($"{this.brokerageGateway} initialized.");

            Debug.NotNull(this.brokerageGateway, nameof(this.brokerageGateway));
        }

        private void OnMessage(SubmitTrade message)
        {
            Debug.NotNull(message, nameof(message));
            Debug.True(this.IsConnectedToBroker(), nameof(this.brokerageGateway));

            foreach (var atomicOrder in message.OrderPacket.Orders)
            {
                this.RouteOrder(atomicOrder);

                this.Log.Debug($"Routing ELS Order {atomicOrder.Symbol} => {this.brokerageGateway.Broker}");
            }
        }

        private void OnMessage(CancelOrder message)
        {
            Validate.True(this.IsConnectedToBroker(), nameof(this.brokerageGateway));

            this.brokerageGateway.CancelOrder(message.Order);
        }

        private void OnMessage(ModifyStopLoss message)
        {
            Validate.True(this.IsConnectedToBroker(), nameof(this.brokerageGateway));

            foreach (var stoplossModification in message.StopLossModificationsIndex)
            {
                var orderModification = new KeyValuePair<Order, Price>(stoplossModification.Key, stoplossModification.Value);
                this.brokerageGateway.ModifyStoplossOrder(orderModification);
                var symbol = stoplossModification.Key.Symbol;

                this.Log.Debug($"Routing StoplossReplaceRequest {symbol} => {this.brokerageGateway.Broker}");
            }
        }

        private void OnMessage(ClosePosition message)
        {
            Validate.True(this.IsConnectedToBroker(), nameof(this.brokerageGateway));

            var tradeUnit = message.ForTradeUnit;
            this.brokerageGateway.ClosePosition(tradeUnit.Position);

            this.Log.Debug($"Routing ClosePosition ({tradeUnit.TradeUnitLabel} => {this.brokerageGateway.Broker}");
        }

        private void RouteOrder(AtomicOrder atomicOrder)
        {
            Validate.True(this.IsConnectedToBroker(), nameof(this.brokerageGateway));
            Debug.NotNull(atomicOrder, nameof(atomicOrder));

            if (atomicOrder.ProfitTargetOrder.HasNoValue)
            {
                this.brokerageGateway.SubmitEntryStopOrder(atomicOrder);

                return;
            }

            this.brokerageGateway.SubmitEntryLimitStopOrder(atomicOrder);
        }

        private bool IsConnectedToBroker()
        {
            Debug.NotNull(this.brokerageGateway, nameof(this.brokerageGateway));

            if (!this.brokerageGateway.IsConnected)
            {
                this.Log.Warning("Cannot process orders (not connected to broker).");

                return false;
            }

            return true;
        }
    }
}
