using EdgeService.gRPC;
using EdgeServices.gRPC;
using Grpc.Core;

namespace EdgeService.gRPC.Services
{
    public class IoTMessageService : IoTMessage.IoTMessageBase
    {
        private readonly ILogger<GreeterService> _logger;
        public IoTMessageService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<EdgeResponse> Send(EdgeRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EdgeResponse
            {
                Message = "Hello " + request.Data
            });
        }
    }
}