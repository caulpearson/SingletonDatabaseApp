using Microsoft.AspNetCore.Mvc;
using SingletonDatabaseApp.Data;
using SingletonDatabaseApp.Models;

namespace SingletonDatabaseApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
        [HttpGet(Name = "GetWorkers")]
        public IEnumerable<Worker> Get()
        { 
            var ws = WorkerSingleton.Instance;
            return new List<Worker>();
        }
    }
}
