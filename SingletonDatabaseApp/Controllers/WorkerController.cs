using Microsoft.AspNetCore.Mvc;
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
            return new List<Worker>();
        }
    }
}
