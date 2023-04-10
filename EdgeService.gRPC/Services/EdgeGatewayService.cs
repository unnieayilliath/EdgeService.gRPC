using EdgeService.gRPC.CloudConnector;
using EdgeServices.gRPC;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace EdgeService.gRPC.Services
{
    public class EdgeGatewayService : EdgeGateway.EdgeGatewayBase
    {
        private readonly ILogger<EdgeGatewayService> _logger;
        public EdgeGatewayService(ILogger<EdgeGatewayService> logger)
        {
            _logger = logger;
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
            //var cloudRequest = new EquipmentEnrichedMessage { Data = request.Payload, SendTime = request.Timestamp,EdgeReceivedTime= receivedTime };
            //await _cloudConnector.SendToCloudAsync(cloudRequest);
            return new EdgeResponse
            {
                ReceivedTime = receivedTime
            };
        }
    }
}