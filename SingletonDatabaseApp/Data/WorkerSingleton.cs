using Microsoft.Data.SqlClient;
using System.Data;

namespace SingletonDatabaseApp.Data
{
    public sealed class WorkerSingleton : IWorkerSingleton
    {
        private static WorkerSingleton instance;
        private IConfiguration _configuration;
        private static object syncObject = new object();
        private readonly DataContext _context;
        DataTable dt = new DataTable();
        public WorkerSingleton(IConfiguration configuration)
        {
            _configuration = configuration;
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
        /*public static WorkerSingleton Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (syncObject)
                    {
                        if(instance == null)
                        {
                            //InitializeInstance();
                            instance = new WorkerSingleton();
                        }
                    }
                }
                return instance;
            }
        }*/
        /*private static void InitializeInstance()
        {
            instance = new WorkerSingleton();
        }

        public DataTable Read()
        {
            return dt;
        }*/
    }
}
