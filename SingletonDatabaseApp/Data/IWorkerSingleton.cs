using System.Data;

namespace SingletonDatabaseApp.Data
{
    public interface IWorkerSingleton
    { 
        DataTable GetWorkers();
        Task RetrieveWorkers();
        Task<bool> getUpdatingIndicator();
        Task setUpdatingIndicator(bool updating);
        void SetTimer();
    }
}
