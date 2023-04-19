namespace EdgeService.gRPC.ERP
{
    public class LocationData
    {
        public string factoryId { get; set; }
        public string DutyManager { get; set; }
        public DateTime DutyStartTime { get; set; }
        public DateTime DutyEndTime { get; set; }
    }
}
