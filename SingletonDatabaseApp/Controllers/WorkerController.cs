using Microsoft.AspNetCore.Mvc;
using SingletonDatabaseApp.Data;
using SingletonDatabaseApp.Models;
using System.Data;

namespace SingletonDatabaseApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerSingleton _workerSingleton;
        public WorkerController(IWorkerSingleton workerSingleton)
        {
            _workerSingleton = workerSingleton;
        }

        [HttpGet(Name = "GetWorkers")]
        public IEnumerable<Worker> Get()
        {
            var ws = _workerSingleton.GetWorkers();
            UpdateWorkers();
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

        public void UpdateWorkers()
        {
            try
            {
                Monitor.Enter(_workerSingleton.getUpdatingIndicator());
                _workerSingleton.setUpdatingIndicator(true);
                try
                {
                    _workerSingleton.RetrieveWorkers();
                }
                finally
                {
                    Monitor.Exit(_workerSingleton.getUpdatingIndicator());
                    _workerSingleton.setUpdatingIndicator(false);
                }
            }
            catch (SynchronizationLockException SyncEx)
            {
                Console.WriteLine(SyncEx.Message);
            }
        }
    }
}
