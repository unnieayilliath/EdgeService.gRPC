using EdgeService.gRPC.CloudConnector;
using CommonModule.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using EdgeService.ProcessingModule;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Azure.Core;

namespace EdgeService.gRPC.Services
{
#pragma warning disable CS0436 // Type conflicts with imported type
    public class EquipmentService : Equipment.EquipmentBase
#pragma warning restore CS0436 // Type conflicts with imported type
    {
        private List<Task> _runningTasks;
        private readonly ILogger<EquipmentService> _logger;
        private readonly DataProcessor _dataProcessor;
        public EquipmentService(ILogger<EquipmentService> logger)
        {
            _logger = logger;
            _dataProcessor = new DataProcessor();
            _runningTasks = new List<Task>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<EdgeResponse> Send(EquipmentMessage request, ServerCallContext context)
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
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<EdgeResponse> SendStream(IAsyncStreamReader<EquipmentMessage> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var currentMessage = requestStream.Current;
                // Process the current message.
                _runningTasks.Add(_dataProcessor.Run(currentMessage));
            }
            Timestamp receivedTime = DateTime.UtcNow.ToTimestamp();
            WaitForAllRunningTasksTocomplete();
            //client has closed stream, so close the cloud connector stream as well.
            _dataProcessor._cloudConnector.Complete_ClientStreamingCallAsync();
            return new EdgeResponse
            {
                ReceivedTime = receivedTime
            };
        }
        /// <summary>
        /// 
        /// </summary>
        private void WaitForAllRunningTasksTocomplete()
        {
            //wait for all the async processing tasks to complete
            Task.WaitAll(_runningTasks.ToArray());
            //reset the tasks
            _runningTasks = new List<Task>();
        }

        /// <summary>
        /// Bi-directional streaming implementation
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task SendBiDirectionalStream(IAsyncStreamReader<EquipmentMessage> requestStream, IServerStreamWriter<EdgeResponse> responseStream, ServerCallContext context)
        {
            try
            {
                while (await requestStream.MoveNext(context.CancellationToken))
                {
                    var requestMessage = requestStream.Current;
                    // Process the current message.
                    _runningTasks.Add(_dataProcessor.Run(requestMessage));
                    Timestamp receivedTime = DateTime.UtcNow.ToTimestamp();
                    var response = new EdgeResponse
                    {
                        MessageId = requestMessage.MessageId,
                        ReceivedTime = receivedTime
                    };
                    await responseStream.WriteAsync(response);
                }
                WaitForAllRunningTasksTocomplete();
                //client has closed stream, so close the cloud connector stream as well.
                await _dataProcessor._cloudConnector.Complete_BiStreamingCallAsync();
            }
            catch (Exception Ex)
            {
                WaitForAllRunningTasksTocomplete();
                //client has closed stream, so close the cloud connector stream as well.
                await _dataProcessor._cloudConnector.Complete_BiStreamingCallAsync();
            }
        }
    }
}