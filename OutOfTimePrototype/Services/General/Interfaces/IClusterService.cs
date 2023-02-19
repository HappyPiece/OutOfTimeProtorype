using OutOfTimePrototype.DAL.Models;

namespace OutOfTimePrototype.Services.Interfaces
{
    public interface IClusterService
    {
        Task<List<Cluster>> GetSuperClusters(Cluster cluster);
        Task<List<Cluster>> GetSubClusters(Cluster cluster);
        Task<List<Cluster>> GetAssociatedClusters(Cluster cluster);
    }
}
