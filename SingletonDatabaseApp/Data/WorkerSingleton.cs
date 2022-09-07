using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Timers;

namespace SingletonDatabaseApp.Data
{
    public sealed class WorkerSingleton : IWorkerSingleton
    {
        private readonly IConfiguration _configuration;
        private static object syncObject = new object();
        DataTable dt = new DataTable();
        private static Boolean _updatingIndicator = false;
        private static int _counter = 0;
        private static int _handlerCounter = 0;
        private static System.Timers.Timer? _timer;
        public WorkerSingleton(IConfiguration configuration)
        {
            _configuration = configuration;
            //SetTimer();
        }
        public void SetTimer()
        {
            Console.WriteLine("SetTimer called");
            _timer = new System.Timers.Timer();
            _timer.Interval = 500;
            _timer.Enabled = true;
            _timer.Elapsed += Handler;
        }

        private async void Handler(object sender, ElapsedEventArgs args)
        {
            Console.WriteLine("Timer handler called at {0} with handler counter: {1}", args.SignalTime.Second + "." + args.SignalTime.Millisecond, _handlerCounter++);
            var updating = await getUpdatingIndicator();
            if (!updating)
            {
                setUpdatingIndicator(true).Wait();
                RetrieveWorkers().Wait();
                setUpdatingIndicator(false).Wait();
            }
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