using EdgeService.gRPC.CloudConnector;
using CommonModule.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using EdgeService.ProcessingModule;

namespace EdgeService.gRPC.Services
{
    public class EdgeGatewayService : EdgeGateway.EdgeGatewayBase
    {
        private readonly ILogger<EdgeGatewayService> _logger;
        private readonly DataProcessor _dataProcessor;
        public EdgeGatewayService(ILogger<EdgeGatewayService> logger)
        {
            _logger = logger;
            _dataProcessor = new DataProcessor();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<EdgeResponse> Send(EquipmentMessage request, ServerCallContext context)
        {
            Timestamp receivedTime = DateTime.UtcNow.ToTimestamp();
            _dataProcessor.Run(request);
            return new EdgeResponse
            {
                ReceivedTime = receivedTime
            };
        }
    }
}