using CommonModule.Protos;
using EdgeService.gRPC.ERP;

namespace EdgeService.ProcessingModule
{
    public class DataEnrichment
    {
        private readonly ERPDbContext _erpDbContext;
        public DataEnrichment()
        {
            _erpDbContext=new ERPDbContext();
        }
        /// <summary>
        /// This method enriches the message with data from the location ERP systems.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public EquipmentEnrichedMessage Run(EquipmentMessage message)
        {
            var enrichedData = new EquipmentEnrichedMessage
            {
                DeviceId = message.DeviceId,
                EnergyConsumption = message.EnergyConsumption,
                Payload = message.Payload,
                Status = message.Status,
                Temperature = message.Temperature,
                Timestamp = message.Timestamp
            };
            // get the location data from local erp database
            var locationData = _erpDbContext.GetCurrentLocationData(message.Timestamp.ToDateTime());
            enrichedData.FactoryId = locationData != null ? locationData.factoryId : "Not found";
            enrichedData.Dutymanager = locationData != null ? locationData.DutyManager : "Not found";
            return enrichedData;
        }
    }
}