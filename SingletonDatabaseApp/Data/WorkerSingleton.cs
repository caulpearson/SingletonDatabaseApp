using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace SingletonDatabaseApp.Data
{
    public sealed class WorkerSingleton : IWorkerSingleton
    {
        private readonly IConfiguration _configuration;
        private static object syncObject = new object();
        DataTable dt = new DataTable();
        private static Boolean _updatingIndicator = false;
        private static int _counter = 0;   
        public WorkerSingleton(IConfiguration configuration)
        {
            _configuration = configuration;
            RetrieveWorkers();
        }

        public async Task RetrieveWorkers()
        {
            Console.WriteLine("Counter within WorkerSingleton.RetrieveWorkers: " + _counter++);
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
                    }
                }
                Console.WriteLine("DataTable filled");
            }
        }

        public DataTable GetWorkers()
        {
            return dt;
        }

        public async Task<bool> getUpdatingIndicator()
        {
            Boolean updating;
            lock (syncObject)
            {
                updating = _updatingIndicator;
            }
            return _updatingIndicator;
        }

        public async Task setUpdatingIndicator(bool updating)
        {
            lock (syncObject)
            {
                _updatingIndicator = updating;
            }
        }
    }
}