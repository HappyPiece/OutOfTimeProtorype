using OutOfTimePrototype.DAL.Models;

namespace OutOfTimePrototype.Services
{
    public interface IClusterService
    {
        Task<List<Cluster>> GetSuperClusters(Cluster cluster);
        Task<List<Cluster>> GetSubClusters(Cluster cluster);
        Task<List<Cluster>> GetAssociatedClusters(Cluster cluster);
    }
}
