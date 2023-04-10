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
        public static List<EquipmentEnrichedMessage> messageQueue = new List<EquipmentEnrichedMessage>();
        public DataAggregator()
        {
        }
        public void Add(EquipmentEnrichedMessage newMessage)
        {
            messageQueue.Add(newMessage);
        }
    }
}
