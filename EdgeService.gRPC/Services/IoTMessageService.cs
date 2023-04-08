using EdgeService.gRPC;
using EdgeServices.gRPC;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace EdgeService.gRPC.Services
{
    public class IoTMessageService : IoTMessage.IoTMessageBase
    {
        private readonly ILogger<IoTMessageService> _logger;
        public IoTMessageService(ILogger<IoTMessageService> logger)
        {
            _logger = logger;
        }

        public override Task<EdgeResponse> Send(EdgeRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EdgeResponse
            {
                ReceivedTime = DateTime.UtcNow.ToTimestamp()
            });
        }
    }
}