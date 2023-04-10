using CommonModule.Protos;
namespace EdgeService.ProcessingModule
{
    public class DataEnrichment
    {
        public DataEnrichment()
        {

        }
        /// <summary>
        /// 
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
            enrichedData.FactoryId = "Factory123";
            enrichedData.Dutymanager = "Unnie Ayilliath";
            return enrichedData;
        }
    }
}