﻿using CommonModule.Protos;
using Google.Protobuf;
using Grpc.Net.Client;

namespace EdgeService.gRPC.CloudConnector
{
    public class CloudConnector
    {
        private CloudBroker.CloudBrokerClient _cloudBrokerClient;
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
        }

        public async Task<CloudResponse> SendToCloudAsync(EquipmentEnrichedMessage request)
        {
            try
            {
                var reply = await _cloudBrokerClient.SendAsync(request);
                return reply;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw ex;
            }
        }
    }
}