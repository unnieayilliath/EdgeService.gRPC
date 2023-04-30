// See https://aka.ms/new-console-template for more information
using CommonModule.Protos;
using EdgeService.gRPC.CloudConnector;
using EdgeService.gRPC.ERP;
using EdgeService.ProcessingModule;
using Google.Protobuf.WellKnownTypes;


var _dbContext = new ERPDbContext();
_dbContext.GetCurrentLocationData(DateTime.Now.AddDays(1));
//var _cloudConnector= new CloudConnector();
//var enrichedData = new EquipmentEnrichedMessage
//{
//    DeviceId = "Device-001",
//    EnergyConsumption = 10,
//    Payload = "dummy",
//    Status = "Running",
//    Temperature = 20,
//    Timestamp = DateTime.UtcNow.ToTimestamp(),
//};
//enrichedData.FactoryId = "Factory123";
//enrichedData.Dutymanager = "Unnie Ayilliath";
//var reply=await _cloudConnector.SendToCloudAsync(enrichedData);
//Console.WriteLine("Hello, World!");
//Console.WriteLine("Reply by cloud="+ reply.ReceivedTime.ToString());
