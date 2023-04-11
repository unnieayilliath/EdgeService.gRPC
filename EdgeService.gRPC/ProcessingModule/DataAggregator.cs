using CommonModule.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeService.ProcessingModule
{
    internal class DataAggregator
    {
        public static List<FacilityMessage> facilityDataList = new List<FacilityMessage>();
        public DataAggregator()
        {
        }
        /// <summary>
        /// This method aggregates data from the facility client with the equipment client.
        /// </summary>
        /// <param name="newMessage"></param>
        public void Run(EquipmentEnrichedMessage newMessage)
        {
            FindClosestFacilityReading(newMessage);
        }
        /// <summary>
        /// This method creates a queue of facility messages
        /// </summary>
        /// <param name="message"></param>
        public void PushToFacilityList(FacilityMessage message)
        {
            facilityDataList.Add(message);
        }
       
        private static void FindClosestFacilityReading(EquipmentEnrichedMessage newMessage)
        {
            var closestFacilityReading = new FacilityMessage();
            DateTime closestTimestamp = DateTime.MinValue;
            TimeSpan closestTimeSpan = TimeSpan.MaxValue;

            foreach (var facilityReading in facilityDataList)
            {
                TimeSpan diff = facilityReading.TimestampStart.ToDateTime() - newMessage.Timestamp.ToDateTime();
                if (diff < closestTimeSpan)
                {
                    closestFacilityReading = facilityReading;
                    closestTimeSpan = diff;
                }
            }
            newMessage.RoomHumidity = closestFacilityReading.Humidity;
            newMessage.RoomTemperature = closestFacilityReading.Temperature;

        }
    }
}
