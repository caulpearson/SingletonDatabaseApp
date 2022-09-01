using Microsoft.AspNetCore.Mvc;
using SingletonDatabaseApp.Data;
using SingletonDatabaseApp.Models;

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
            return new List<Worker>();
        }
    }
}
