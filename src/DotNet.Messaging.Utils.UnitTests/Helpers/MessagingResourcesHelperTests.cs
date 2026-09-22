using DotNet.Messaging.Utils.Enums;
using DotNet.Messaging.Utils.Errors;
using DotNet.Messaging.Utils.Helpers;
using DotNet.Messaging.Utils.Options;
using BryanCM.AspNet.Utils.Abstractions;

namespace DotNet.Messaging.Utils.UnitTests.Helpers
{
    public class MessagingResourcesHelperTests
    {
        private readonly static string _routingKeyForSingleQueue = "single-queue-key";
        private readonly static string _routingKeyForQueueWithDeadLetter = "queue-with-dead-letter-key";
        private readonly static string _routingKeyForQueueWithParkingLot = "queue-with-parking-lot-key";
        private readonly MessagingOptions _options = new()
        {
            Resources = new()
            {
                Exchanges = [
                    new()
                    {
                        Name = "ex.exchange",
                        Type = "direct",
                        For = "main"
                    },
                    new()
                    {
                        Name = "dlx.exchange",
                        Type = "direct",
                        For = "deadLetter"
                    },
                    new()
                    {
                        Name = "ex.exchange.parking",
                        Type = "direct",
                        For = "parkingLot"
                    }
                ],
                Queues = [
                    new()
                    {
                        Name = "q.single",
                        RoutingKey = _routingKeyForSingleQueue
                    },
                    new()
                    {
                        Name = "q.with-deadletter",
                        RoutingKey = _routingKeyForQueueWithDeadLetter,
                        DeadLetter = new()
                        {
                            Name = "dlq.queue",
                            RoutingKey = _routingKeyForQueueWithDeadLetter,
                            TtlInSeconds = 5
                        }
                    },
                    new()
                    {
                        Name = "q.with-parking",
                        RoutingKey = _routingKeyForQueueWithParkingLot,
                        ParkingLot = new()
                        {
                            Name = "q.parking-lot",
                            RoutingKey = _routingKeyForQueueWithParkingLot
                        }
                    }
                ]
            }
        };

        [Theory]
        [InlineData(ResourceType.Main)]
        [InlineData(ResourceType.DeadLetter)]
        [InlineData(ResourceType.ParkingLot)]
        public void GetExchangeByType_ShouldReturnExchange_WhenFound(ResourceType type)
        {
            var exchange = MessagingResourcesHelper.GetExchangeByType(_options, type);

            Assert.NotNull(exchange);
            Assert.NotNull(exchange.Type);
            Assert.Equal(type, exchange.ResourceType);
        }

        [Fact]
        public void GetExchangeByType_ShouldReturnNull_WhenNotFound()
        {
            var exchangeNotFound = MessagingResourcesHelper.GetExchangeByType(_options, (ResourceType)int.MaxValue);
            Assert.NotNull(_options.Resources);
            Assert.NotEmpty(_options.Resources.Exchanges);
            Assert.Null(exchangeNotFound);
        }

        [Fact]
        public void GetParkingLotFromRoutingKeyOfMainQueue_ShouldReturnSuccessResult_WhenFound()
        {
            var result = MessagingResourcesHelper.GetParkingLotFromRoutingKeyOfMainQueue(_options, _routingKeyForQueueWithParkingLot);
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(Error.None, result.Error);
            Assert.Equal("q.parking-lot", result.Data.Name);
        }

        [Fact]
        public void GetParkingLotFromRoutingKeyOfMainQueue_ShouldReturnFailureResult_WhenQueueNotFound()
        {
            _options.Resources.Queues!.RemoveAll(e => e.RoutingKey == "some-routing-key");
            var result = MessagingResourcesHelper.GetParkingLotFromRoutingKeyOfMainQueue(_options, "some-routing-key");
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(MessagingErrors.QueueNotFound, result.Error);
        }

        [Fact]
        public void GetParkingLotFromRoutingKeyOfMainQueue_ShouldReturnFailureResult_WhenParkingLotNotConfigured()
        {
            var result = MessagingResourcesHelper.GetParkingLotFromRoutingKeyOfMainQueue(_options, _routingKeyForSingleQueue);
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(MessagingErrors.ParkingLotQueueNotConfigured, result.Error);
        }

        [Fact]
        public void GetQueueOrDefault_ShouldReturnQueue_WhenFound()
        {
            var queue = MessagingResourcesHelper.GetQueueOrDefault(_options, _routingKeyForSingleQueue);
            Assert.NotNull(queue);
            Assert.Equal("q.single", queue.Name);
            Assert.Equal(_routingKeyForSingleQueue, queue.RoutingKey);
        }

        [Fact]
        public void GetQueueOrDefault_ShouldReturnQueueWithDeadLetter_WhenFound()
        {
            var queue = MessagingResourcesHelper.GetQueueOrDefault(_options, _routingKeyForQueueWithDeadLetter);

            Assert.NotNull(queue);
            Assert.NotNull(queue.DeadLetter);
            Assert.Equal("q.with-deadletter", queue.Name);
            Assert.Equal(_routingKeyForQueueWithDeadLetter, queue.RoutingKey);
        }

        [Fact]
        public void GetQueueOrDefault_ShouldReturnQueueWithParkingLot_WhenFound()
        {
            var queue = MessagingResourcesHelper.GetQueueOrDefault(_options, _routingKeyForQueueWithParkingLot);
            Assert.NotNull(queue);
            Assert.NotNull(queue.ParkingLot);
            Assert.Equal("q.with-parking", queue.Name);
            Assert.Equal(_routingKeyForQueueWithParkingLot, queue.RoutingKey);
        }

        [Fact]
        public void GetQueueOrDefault_ShouldReturnNull_WhenFound()
        {
            var queueNotFound = MessagingResourcesHelper.GetQueueOrDefault(_options, "unknown-routing-key");
            Assert.NotNull(_options.Resources);
            Assert.NotEmpty(_options.Resources.Queues);
            Assert.Null(queueNotFound);
        }
    }
}
