using EdgeService.gRPC.CloudConnector;
using CommonModule.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using EdgeService.ProcessingModule;
using Microsoft.Extensions.Logging;

namespace EdgeService.gRPC.Services
{
#pragma warning disable CS0436 // Type conflicts with imported type
    public class FacilityService : Facility.FacilityBase
#pragma warning restore CS0436 // Type conflicts with imported type
    {
        private readonly ILogger<FacilityService> _logger;
        private readonly DataAggregator _dataAggregator;
        public FacilityService(ILogger<FacilityService> logger)
        {
            _logger = logger;
            _dataAggregator = new DataAggregator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<EdgeResponse> Send(FacilityMessage request, ServerCallContext context)
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