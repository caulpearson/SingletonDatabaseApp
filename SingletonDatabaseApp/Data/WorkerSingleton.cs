using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace SingletonDatabaseApp.Data
{
    public sealed class WorkerSingleton : IWorkerSingleton
    {
        private static WorkerSingleton instance;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private static object syncObject = new object();
        DataTable dt = new DataTable();
        public WorkerSingleton(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
            RetrieveWorkers();
        }

        private void RetrieveWorkers()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection Conn = new SqlConnection(connectionString))
            {
                Conn.Open();
                if (Conn.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("successfully connected");
                }
                string sql = "select * from worker";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = Conn;
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                        _memoryCache.CreateEntry(dt);
                    }
                }
            }
        }

        public DataTable GetWorkers()
        {
            return dt;
        }
    }
}
