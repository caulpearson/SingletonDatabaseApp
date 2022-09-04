using System.Data;

namespace SingletonDatabaseApp.Data
{
    public interface IWorkerSingleton
    { 
        DataTable GetWorkers();
        void RetrieveWorkers();
        Boolean getUpdatingIndicator();
        void setUpdatingIndicator(Boolean newUpdatingIndicator);
    }
}
