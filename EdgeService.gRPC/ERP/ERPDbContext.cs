
using Microsoft.Data.SqlClient;
using System.Data;

namespace EdgeService.gRPC.ERP
{
    public class ERPDbContext : IDisposable
    {
        private SqlConnection _connection;
        public ERPDbContext()
        {
            _connection = new SqlConnection(@"Server=vm-edge\SQLEXPRESS01;Database=ERP;Trusted_Connection=True;TrustServerCertificate=True");
            _connection.Open();
        }
        public LocationData GetCurrentLocationData(DateTime readingTime)
        {

            using(var cmd= new SqlCommand())
            {
                try
                {
                    var dateTimeParam = new SqlParameter("@DateTimeParam", SqlDbType.DateTime);
                    dateTimeParam.Value = readingTime;
                    cmd.Parameters.Add(dateTimeParam);
                    LocationData locationData = null;
                    cmd.Connection = _connection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = $@"SELECT TOP(1) [factoryId],[DutyManager],[DutyStartTime],[DutyEndTime] FROM [erp].[dbo].[LocationData] where DutyStartTime<=@DateTimeParam and DutyEndTime>=@DateTimeParam";
                    var dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        locationData= new LocationData()
                        {
                            factoryId = dataReader.GetFieldValue<string>(0),
                            DutyManager = dataReader.GetFieldValue<string>(1)
                        };
                        break;
                    }
                    dataReader.Close();
                    return locationData;
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
