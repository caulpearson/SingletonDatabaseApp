using System.Data;

namespace SingletonDatabaseApp.Data
{
    public interface IWorkerSingleton
    {
        DataTable GetWorkers();
    }
}
