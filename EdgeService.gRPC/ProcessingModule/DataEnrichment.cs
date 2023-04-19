using CommonModule.Protos;
using EdgeService.gRPC.ERP;

namespace EdgeService.ProcessingModule
{
    public class DataEnrichment
    {
        public DataEnrichment()
        {

        }
        /// <summary>
        /// This method enriches the message with data from the location ERP systems.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public EquipmentEnrichedMessage Run(EquipmentMessage message)
        {
            var enrichedData= new EquipmentEnrichedMessage
            {
                DeviceId= message.DeviceId,
                EnergyConsumption= message.EnergyConsumption,
                Payload= message.Payload,
                Status= message.Status,
                Temperature= message.Temperature,
                Timestamp= message.Timestamp
            };
            // get the location data from local erp database
            var locationData= GetCurrentLocationData(message.Timestamp.ToDateTime());
            enrichedData.FactoryId = locationData!=null?locationData.factoryId:"Not found";
            enrichedData.Dutymanager = locationData != null ? locationData.DutyManager : "Not found";
            return enrichedData;
        }
        /// <summary>
        /// This method gets the current location data for the specified time
        /// </summary>
        /// <param name="readingTime"></param>
        /// <returns></returns>
        public LocationData GetCurrentLocationData(DateTime readingTime)
        {
            using (var context = new ERPDbContext())
            {
                var currentDutyManager = context.LocationDatas
                    .Where(d => d.DutyStartTime <= readingTime && d.DutyEndTime > readingTime)
                    .FirstOrDefault();

                if (currentDutyManager != null)
                {
                    return currentDutyManager;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}