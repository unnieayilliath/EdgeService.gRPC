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
        public DataProcessor()
        {
            _cloudConnector = new CloudConnector();
        }

        public void Run(EquipmentMessage message)
        {

        }
    }
}
