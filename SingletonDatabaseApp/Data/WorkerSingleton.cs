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
        private static Boolean updatingIndicator { get; set; } = false;
        public WorkerSingleton(IConfiguration configuration)
        {
            _configuration = configuration;
            RetrieveWorkers();
        }

        public void RetrieveWorkers()
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
                    }
                }
            }
        }

        public DataTable GetWorkers()
        {
            return dt;
        }

        public Boolean getUpdatingIndicator()
        {
            return updatingIndicator;
        }

        public void setUpdatingIndicator(Boolean updating)
        {
            updatingIndicator = updating;
        }
    }
}