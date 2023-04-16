using EdgeService.gRPC.CloudConnector;
using CommonModule.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using EdgeService.ProcessingModule;
using Microsoft.Extensions.Logging;

namespace EdgeService.gRPC.Services
{
#pragma warning disable CS0436 // Type conflicts with imported type
    public class EdgeGatewayService : EdgeGateway.EdgeGatewayBase
#pragma warning restore CS0436 // Type conflicts with imported type
    {
        private readonly ILogger<EdgeGatewayService> _logger;
        private readonly DataProcessor _dataProcessor;
        private readonly DataAggregator _dataAggregator;
        public EdgeGatewayService(ILogger<EdgeGatewayService> logger)
        {
            _logger = logger;
            _dataProcessor = new DataProcessor();
            _dataAggregator = new DataAggregator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<EdgeResponse> SendEquipment(EquipmentMessage request, ServerCallContext context)
        {
            _logger.LogInformation("Received equipment request!");
            Timestamp receivedTime = DateTime.UtcNow.ToTimestamp();
            _dataProcessor.Run(request);
            return new EdgeResponse
            {
                ReceivedTime = receivedTime
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<EdgeResponse> SendFacility(FacilityMessage request, ServerCallContext context)
        {
            Timestamp receivedTime = DateTime.UtcNow.ToTimestamp();
            _logger.LogInformation("Received facility request!");
            _dataAggregator.PushToFacilityList(request);
            return new EdgeResponse
            {
                ReceivedTime = receivedTime
            };
        }
    }
}