using Microsoft.AspNetCore.Mvc;
using SingletonDatabaseApp.Data;
using SingletonDatabaseApp.Models;
using System.Data;
using System.Timers;

namespace SingletonDatabaseApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerSingleton _workerSingleton;
        private static System.Timers.Timer? _timer;
        private static int _counter = 0;
        public WorkerController(IWorkerSingleton workerSingleton)
        {
            _workerSingleton = workerSingleton;

            //SetTimer();
        }

        private void SetTimer()
        {
            Console.WriteLine("SetTimer called");
            _timer = new System.Timers.Timer();
            _timer.Interval = 50;
            _timer.Enabled = true;
            _timer.Elapsed += Handler;
        }


        [HttpGet("GetWorkers")]
        public IEnumerable<Worker> Get()
        {
            var ws = _workerSingleton.GetWorkers();
            //UpdateWorkers();
            var updating = _workerSingleton.getUpdatingIndicator();
            List<Worker> workers = new List<Worker>();
            try
            {
                Monitor.Enter(_workerSingleton.getUpdatingIndicator());
                _workerSingleton.setUpdatingIndicator(true);
                try
                {
                    foreach (DataRow row in ws.Rows)
                    {
                        //do something
                        Worker worker = new Worker();
                        var rowData = row.ItemArray;
                        worker.WORKER_ID = (int)row.ItemArray.ElementAt(0);
                        worker.FIRST_NAME = (string)row.ItemArray.ElementAt(1);
                        worker.LAST_NAME = (string)row.ItemArray.ElementAt(2);
                        worker.SALARY = (int)row.ItemArray.ElementAt(3);
                        worker.JOINING_DATE = (DateTime)row.ItemArray.ElementAt(4);
                        worker.DEPARTMENT = (string)row.ItemArray.ElementAt(5);
                        workers.Add(worker);
                    }
                }
                finally
                {
                    Monitor.Exit(_workerSingleton.getUpdatingIndicator());
                    _workerSingleton.setUpdatingIndicator(false);
                }
            }catch ( SynchronizationLockException SyncEx)
            {
                Console.WriteLine(SyncEx.Message);
            }
            return workers;
        }

        private async void Handler(object sender, ElapsedEventArgs args)
        {
            Console.WriteLine("Timer handler called at {0} with counter: {1}", args.SignalTime.Second +"."+ args.SignalTime.Millisecond, _counter++);
            var updating = await _workerSingleton.getUpdatingIndicator();
            if (!updating)
            {
                _workerSingleton.setUpdatingIndicator(true).Wait();
                _workerSingleton.RetrieveWorkers().Wait();
                _workerSingleton.setUpdatingIndicator(false).Wait();
            }
        }

        /*[HttpGet("SetControllerTimer")]
        public Boolean SetControllerTimer()
        {
            SetTimer();
            return true;
        }*/

        [HttpGet("SetUpdateTimer")]
        public Boolean SetSingletonTimer()
        {
            _workerSingleton.SetTimer();
            return true;
        }

        [HttpGet("UpdateWorkers")]
        public Boolean UpdateWorkers()
        {
            _workerSingleton.RetrieveWorkers();
            return true;
        }
    }
}
