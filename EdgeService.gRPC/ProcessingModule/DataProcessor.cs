using EdgeService.gRPC.CloudConnector;
using CommonModule.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeService.ProcessingModule
{
    public class DataProcessor
    {
        private CloudConnector _cloudConnector;
        private DataEnrichment _dataEnrichment;
        private DataAggregator _dataAggregator;
        public DataProcessor()
        {
            _cloudConnector = new CloudConnector();
            _dataEnrichment = new DataEnrichment();
            _dataAggregator = new DataAggregator();
        }

        public void Run(EquipmentMessage message)
        {
            // enrichment module
            var enrichedMessage = _dataEnrichment.Run(message);
            // aggregate module
            _dataAggregator.Run(enrichedMessage);
            _cloudConnector.SendToCloudAsync(enrichedMessage);
        }
    }
}
