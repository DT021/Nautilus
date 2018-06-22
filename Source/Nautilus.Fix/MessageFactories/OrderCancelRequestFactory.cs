﻿//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelRequestFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.DomainModel.Aggregates;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// The order cancel request.
    /// </summary>
    public static class OrderCancelRequestFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="order">The order.</param>
        /// <param name="transactionTime">The transaction time.</param>
        /// <returns>The <see cref="OrderCancelRequest"/>.</returns>
        public static OrderCancelRequest Create(
            string brokerSymbol,
            Order order,
            ZonedDateTime transactionTime)
        {
            var orderMessage = new OrderCancelRequest();

            orderMessage.SetField(new OrigClOrdID(order.OrderId.ToString()));
            orderMessage.SetField(new OrderID(order.BrokerOrderId.ToString()));
            orderMessage.SetField(new ClOrdID(order.CurrentOrderId.ToString()));
            orderMessage.SetField(new Symbol(brokerSymbol));
            orderMessage.SetField(new Quantity(order.Quantity.Value));
            orderMessage.SetField(FxcmFixMessageHelper.GetFixOrderSide(order.OrderSide));
            orderMessage.SetField(new TransactTime(transactionTime.ToDateTimeUtc()));

            return orderMessage;
        }
    }
}
