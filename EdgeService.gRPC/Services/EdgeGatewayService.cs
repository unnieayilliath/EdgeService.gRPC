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
        private static DataProcessor _dataProcessor;
        private readonly DataAggregator _dataAggregator;
        public EdgeGatewayService(ILogger<EdgeGatewayService> logger)
        {
            _logger = logger;
            if (_dataProcessor == null)
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
            await _dataProcessor.Run(request);
            return new EdgeResponse
            {
                ReceivedTime = receivedTime
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<EdgeResponse> SendEquipmentStream(IAsyncStreamReader<EquipmentMessage> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var currentMessage = requestStream.Current;
                // Process the current message.
                _dataProcessor.Run(currentMessage);
            }
            Timestamp receivedTime = DateTime.UtcNow.ToTimestamp();
            return new EdgeResponse
            {
                ReceivedTime = receivedTime
            };
        }
        /// <summary>
        /// Bi-directional streaming implementation
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task SendEquipmentBiDirectionalStream(IAsyncStreamReader<EquipmentMessage> requestStream, IServerStreamWriter<EdgeResponse> responseStream, ServerCallContext context)
        {

            await foreach (var requestMessage in requestStream.ReadAllAsync(context.CancellationToken))
            {
                // Process the current message.
                _dataProcessor.Run(requestMessage);
                Timestamp receivedTime = DateTime.UtcNow.ToTimestamp();
                var response = new EdgeResponse
                {
                    MessageId = requestMessage.MessageId,
                    ReceivedTime = receivedTime
                };
                await responseStream.WriteAsync(response);
            }
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