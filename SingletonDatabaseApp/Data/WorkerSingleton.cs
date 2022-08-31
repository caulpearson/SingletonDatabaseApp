using Microsoft.Data.SqlClient;
using System.Data;

namespace SingletonDatabaseApp.Data
{
    public sealed class WorkerSingleton
    {
        private static WorkerSingleton instance;
        private static object syncObject = new object();
        private readonly DataContext _context;
        DataTable dt = new DataTable();
        private WorkerSingleton()
        {
            var connectionString = "";
            using (SqlConnection Conn = new SqlConnection(connectionString))
            {
                Conn.Open();
                if (Conn.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("successfully connected");
                }
                string sql = "select * from Workers";
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
        public static WorkerSingleton Instance
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
        }
        private static void InitializeInstance()
        {
            instance = new WorkerSingleton();
        }

        public DataTable Read()
        {
            return dt;
        }
    }
}
