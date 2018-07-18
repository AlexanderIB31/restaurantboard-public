﻿using Dodo.Core.Common;
using Dodo.RestaurantBoard.Domain.Services;
using Dodo.Tracker.Contracts;
using Dodo.Tracker.Contracts.Enums;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;


namespace Dodo.Tests
{
    public static class Extensions
    {
       public static ProductionOrder[] Orders(this int orderCount)
        {
            var orders = new List<ProductionOrder>();
            for (int i =0; i< orderCount; i++)
            {
                orders.Add(new ProductionOrder());
            }

            return orders.ToArray();
        }
    }

    public static class Create
    {
        public static TrackerClientBuilder TrackerClient ()
        {
            return new TrackerClientBuilder();
        }
    }

    public static class Get
    {
        public static ProductionOrder[] OrdersFrom(ITrackerClient trackerClient)
        {
            return trackerClient.GetOrders(new Uuid(), OrderType.Delivery, new OrderState[1], 0);
        }
    }

    public class TrackerClientBuilder
    {
        private ProductionOrder[] _orders;
        
        public TrackerClientBuilder WithOrders (ProductionOrder[] productionOrders)
        {
            _orders = productionOrders;
            return this;
        }

        public TrackerClient Please()
        {
            var ordersProviderStub = new Mock<IOrdersProvider>();
            var dateProviderDummy = new DateProvider();
            ordersProviderStub.Setup(p => p.GetOrders()).Returns(_orders);
            return new TrackerClient(ordersProviderStub.Object, dateProviderDummy);
        }
    }
    
    public class TrackerClientShould
    {                
        [Fact]
        public void ReturnAllOrders_WhenGetOrdersIsCalledWithoutExpiringOnlyParameter()
        {
            var orders = 2.Orders();
            var trackerClient = Create.TrackerClient().WithOrders(orders).Please();
            
            var givenOrders = Get.OrdersFrom(trackerClient);
            // get orders from tracker client

            // assert that expected orders equal to actual ones


//            var expectedOrders = new ProductionOrder[]
//            {
//                new ProductionOrder(),
//                new ProductionOrder()
//            };
//            var ordersProviderStub = new Mock<IOrdersProvider>();
//            var dateProviderDummy = new DateProvider();
//            ordersProviderStub.Setup(p => p.GetOrders()).Returns(expectedOrders);
//            var trackerClient = new TrackerClient(ordersProviderStub.Object, dateProviderDummy);
//
//            var actualOrders = GetOrdersWithoutExpiringOnlyParameter(trackerClient);
//            
//            Assert.Equal(expectedOrders, actualOrders);
        }        

        [Fact]
        public void ReturnOnlyExpiringOrders_WhenGetOrdersIsCalledWithExpiringOnlyParameterEqualToTrue()
        {
            var dateProviderStub = new Mock<IDateProvider>();
            dateProviderStub.Setup(p => p.Now()).Returns(new DateTime(2018, 07, 11, 23, 00, 00));
            var notExpiringOrder = new ProductionOrder
            {
                ChangeDate = new DateTime(2018, 07, 11, 22, 00, 00)
            };
            var expiringOrder = new ProductionOrder
            {
                ChangeDate = new DateTime(2018, 07, 11, 21, 59, 00)
            };
            var expectedOrders = new ProductionOrder[]
            {
                notExpiringOrder,
                expiringOrder
            };
            var ordersProviderStub = new Mock<IOrdersProvider>();
            ordersProviderStub.Setup(p => p.GetOrders()).Returns(expectedOrders);
            var trackerClient = new TrackerClient(ordersProviderStub.Object, dateProviderStub.Object);

            var actualOrders = GetOrdersWithExpiringOnlyParameterEqualToTrue(trackerClient);

            Assert.Equal(new[]{ expiringOrder }, actualOrders);
        }


        [Fact]
        public void CallGetOrdersOnOrdersProvider_WhenGetOrdersIsCalled()
        {
            var ordersProviderMock = new Mock<IOrdersProvider>();
            var dateProviderDummy = new DateProvider();
            var trackerClient = new TrackerClient(ordersProviderMock.Object, dateProviderDummy);

            var orders = GetOrdersWithoutExpiringOnlyParameter(trackerClient);

            ordersProviderMock.Verify(op => op.GetOrders(), Times.Once);
        }
        
        [Fact]
        public void NotCallNowOnDateProvider_WhenGetOrdersIsCalledWithoutExpiringOnlyParameter()
        {
            var ordersProviderStub = new OrdersProvider();
            var dateProviderMock = new Mock<IDateProvider>();
            var trackerClient = new TrackerClient(ordersProviderStub, dateProviderMock.Object);

            var orders = GetOrdersWithoutExpiringOnlyParameter(trackerClient);

            dateProviderMock.Verify(dp => dp.Now(), Times.Never);
        }
        
        [Fact]
        public void CallNowOnDateProvider_WhenGetOrdersIsCalledWithExpiringOnlyParameterEqualToTrue()
        {
            var ordersProviderStub = new OrdersProvider();
            var dateProviderMock = new Mock<IDateProvider>();
            var trackerClient = new TrackerClient(ordersProviderStub, dateProviderMock.Object);

            var orders = GetOrdersWithExpiringOnlyParameterEqualToTrue(trackerClient);

            dateProviderMock.Verify(dp => dp.Now(), Times.Once);
        }
        
        [Fact]
        public void NotCallIsExpiringOnEachProductionOrder_WhenGetOrdersIsCalledWithoutExpiringOnlyParameter()
        {
            var productionOrderMock = new Mock<ProductionOrder>();
            var expectedOrders = new ProductionOrder[]
            {
                productionOrderMock.Object
            };
            var ordersProviderStub = new Mock<IOrdersProvider>();
            ordersProviderStub.Setup(p => p.GetOrders()).Returns(expectedOrders);
            var dateProviderDummy = new DateProvider();
            var trackerClient = new TrackerClient(ordersProviderStub.Object, dateProviderDummy);
            
            var orders = GetOrdersWithoutExpiringOnlyParameter(trackerClient);

            productionOrderMock.Verify(pom => pom.IsExpiring(It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public void CallIsExpiringOnEachProductionOrder_WhenGetOrdersIsCalledWithExpiringOnlyParameterEqualToTrue()
        {
            var productionOrderMock = new Mock<ProductionOrder>();
            var expectedOrders = new ProductionOrder[]
            {
                productionOrderMock.Object
            };
            var ordersProviderStub = new Mock<IOrdersProvider>();
            ordersProviderStub.Setup(p => p.GetOrders()).Returns(expectedOrders);
            var dateProviderStub = new DateProvider();
            var trackerClient = new TrackerClient(ordersProviderStub.Object, dateProviderStub);

            var orders = GetOrdersWithExpiringOnlyParameterEqualToTrue(trackerClient);

            productionOrderMock.Verify(pom => pom.IsExpiring(It.IsAny<DateTime>()), Times.Once);
        }
        
        
        private ProductionOrder[] GetOrdersWithoutExpiringOnlyParameter(ITrackerClient trackerClient)
        {
            return trackerClient.GetOrders(new Uuid(), OrderType.Delivery, new OrderState[1], 0);
        }
        
        private ProductionOrder[] GetOrdersWithExpiringOnlyParameterEqualToTrue(ITrackerClient trackerClient)
        {
            return trackerClient.GetOrders(new Uuid(), OrderType.Delivery, new OrderState[1], 0, true);
        }
    }

    
}