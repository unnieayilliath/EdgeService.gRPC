using EdgeService.gRPC.CloudConnector;
using EdgeServices.gRPC;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace EdgeService.gRPC.Services
{
    public class IoTMessageService : IoTMessage.IoTMessageBase
    {
        private readonly ILogger<IoTMessageService> _logger;
        private CloudConnector.CloudConnector _cloudConnector;
        public IoTMessageService(ILogger<IoTMessageService> logger)
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