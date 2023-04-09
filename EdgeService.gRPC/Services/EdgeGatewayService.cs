using EdgeService.gRPC.CloudConnector;
using EdgeServices.gRPC;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace EdgeService.gRPC.Services
{
    public class EdgeGatewayService : EdgeGateway.EdgeGatewayBase
    {
        private readonly ILogger<EdgeGatewayService> _logger;
        private CloudConnector.CloudConnector _cloudConnector;
        public EdgeGatewayService(ILogger<EdgeGatewayService> logger)
        {
            _logger = logger;
            _cloudConnector= new CloudConnector.CloudConnector();
        }

        public override async Task<EdgeResponse> Send(EdgeRequest request, ServerCallContext context)
        {
            Timestamp receivedTime = DateTime.UtcNow.ToTimestamp();
            var cloudRequest = new CloudRequest { Data = request.Data, SendTime = request.SendTime,EdgeReceivedTime= receivedTime };
            await _cloudConnector.SendToCloudAsync(cloudRequest);
            return new EdgeResponse
            {
                ReceivedTime = receivedTime
            };
        }
    }
}