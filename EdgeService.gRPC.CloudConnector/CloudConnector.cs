using CommonModule.Protos;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using System.Text.Json;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EdgeService.gRPC.CloudConnector
{
    public class CloudConnector : IAsyncDisposable
    {
        private CloudBroker.CloudBrokerClient _cloudBrokerClient;
        private Grpc.Core.AsyncClientStreamingCall<EquipmentEnrichedMessage, CloudResponse> _clientStreamingCall;
        private Grpc.Core.AsyncDuplexStreamingCall<EquipmentEnrichedMessage, CloudResponse> _biStreamingCall;
        private Task _readResponsesTask;
        public List<string> logs = new List<string>();
        private string _streamingMode;
        public CloudConnector()
        {
            // create a httpHandler
            var httpHandler = new HttpClientHandler();
            //ignore certificate validations
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var httpClient = new HttpClient(httpHandler);
            // The port number must match the port of the gRPC server.
            var channel = GrpcChannel.ForAddress("https://localhost:5003", new GrpcChannelOptions { HttpClient = httpClient });
            _cloudBrokerClient = new CloudBroker.CloudBrokerClient(channel);
            //change the default streaming mode
            _streamingMode = StreamingMode.BiDirectional;
            if (_streamingMode == StreamingMode.ClientStreaming)
            {
                Create_ClientStreamingCall();
            }
            else if (_streamingMode == StreamingMode.BiDirectional)
            {
                Create_BiStreamingCall();
            }
        }

        public async Task<CloudResponse> SendToCloud_UnaryAsync(EquipmentEnrichedMessage request)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var reply = await _cloudBrokerClient.SendAsync(request);
                var receivedTime = DateTime.UtcNow;
                TimeSpan ts = receivedTime - startTime;
                string jsonData = JsonSerializer.Serialize(request);
                logs.Add($"ProtocolBufferSize={request.CalculateSize()},JSONSize={Encoding.UTF8.GetByteCount(jsonData)},RTT={ts.TotalMilliseconds}");
                return reply;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SendToCloud_ClientStreamAsync(EquipmentEnrichedMessage request)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                await _clientStreamingCall.RequestStream.WriteAsync(request);
                string jsonData = JsonSerializer.Serialize(request);
                logs.Add($"ProtocolBufferSize={request.CalculateSize()},JSONSize={Encoding.UTF8.GetByteCount(jsonData)},SendTime={startTime},messageId={request.MessageId}");
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SendToCloud_BiStreamAsync(EquipmentEnrichedMessage request)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                await _biStreamingCall.RequestStream.WriteAsync(request);
                string jsonData = JsonSerializer.Serialize(request);
                logs.Add($"ProtocolBufferSize={request.CalculateSize()},JSONSize={Encoding.UTF8.GetByteCount(jsonData)},SendTime={startTime},messageId={request.MessageId}");
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw ex;
            }
        }
        /// <summary>
        /// This method creates a client streaming call
        /// </summary>
        public void Create_ClientStreamingCall()
        {
            _clientStreamingCall = _cloudBrokerClient.SendStream();
        }
        /// <summary>
        /// This method marks the client streaming call as completed.
        /// </summary>
        public async Task Complete_ClientStreamingCallAsync()
        {
            await _clientStreamingCall.RequestStream.CompleteAsync();
            var response = await _clientStreamingCall.ResponseAsync;
            PerformanceLogger.WriteDataToFile($"clientstream_${DateTime.Now.Ticks.ToString()}.txt",logs);
        }

        /// <summary>
        /// This method creates a client streaming call
        /// </summary>
        public void Create_BiStreamingCall()
        {
            _biStreamingCall = _cloudBrokerClient.SendBiDirectionalStream();
            // register all response calls 
            _readResponsesTask = Task.Run(async () =>
            {
                await foreach (var responseMessage in _biStreamingCall.ResponseStream.ReadAllAsync())
                {
                    //get the response for the message
                    logs.Add($",,,ReceivedTime={DateTime.UtcNow},messageId={responseMessage.MessageId}");
                }
            });
        }
        /// <summary>
        /// This method marks the client streaming call as completed.
        /// </summary>
        public async Task Complete_BiStreamingCallAsync()
        {
            await _biStreamingCall.RequestStream.CompleteAsync();
            await _readResponsesTask;
            PerformanceLogger.WriteDataToFile($"bistream_${DateTime.Now.Ticks.ToString()}.txt",logs);
        }


        public async ValueTask DisposeAsync()
        {
            if (_streamingMode == StreamingMode.ClientStreaming)
            {
                await Complete_ClientStreamingCallAsync();
            }
            else if (_streamingMode == StreamingMode.BiDirectional)
            {
                await Complete_BiStreamingCallAsync();
            }
        }

        public void SendToCloudAsync(EquipmentEnrichedMessage enrichedMessage)
        {
            if (_streamingMode == StreamingMode.ClientStreaming)
            {
                SendToCloud_ClientStreamAsync(enrichedMessage);
            }
            else if (_streamingMode == StreamingMode.BiDirectional)
            {
                SendToCloud_BiStreamAsync(enrichedMessage);
            }
            else
            {
                SendToCloud_UnaryAsync(enrichedMessage);
            }
        }
    }
}