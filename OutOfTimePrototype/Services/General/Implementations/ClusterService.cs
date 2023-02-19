using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.Services.Interfaces;
using System.Net;

namespace OutOfTimePrototype.Services.Implementations
{
    public class ClusterService : IClusterService
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;

        public ClusterService(OutOfTimeDbContext outOfTimeDbContext)
        {
            _outOfTimeDbContext = outOfTimeDbContext;
        }
        public async Task<List<Cluster>> GetSuperClusters(Cluster cluster)
        {
            var result = new List<Cluster>();
            var currentClusters = new List<Cluster>();
            var nextClusters = new List<Cluster>();

            currentClusters.Add(cluster);

            while (currentClusters.Count != 0)
            {
                nextClusters = new List<Cluster>();
                foreach (var clust in currentClusters)
                {
                    if (clust.SuperCluster is not null)
                        nextClusters.AddRange(await _outOfTimeDbContext.Clusters.Include(x => x.SuperCluster).
                        Where(x => x == clust.SuperCluster)
                        .ToListAsync());
                }

                currentClusters = nextClusters;
                if (currentClusters.Count == 0)
                {
                    continue;
                }

                result.AddRange(currentClusters);
            }
            return result;
        }

        public async Task<List<Cluster>> GetAssociatedClusters(Cluster cluster)
        {
            var result = new List<Cluster>();
            result.Add(cluster);
            result.AddRange(await GetSubClusters(cluster));
            result.AddRange(await GetSuperClusters(cluster));
            return result;
        }

        public async Task<List<Cluster>> GetSubClusters(Cluster cluster)
        {
            var result = new List<Cluster>();
            var currentClusters = new List<Cluster>();
            var nextClusters = new List<Cluster>();

            currentClusters.Add(cluster);

            while (currentClusters.Count != 0)
            {
                nextClusters = new List<Cluster>();
                foreach (var clust in currentClusters)
                {
                    nextClusters.AddRange(await _outOfTimeDbContext.Clusters.Include(x => x.SuperCluster).
                        Where(x => x.SuperCluster != null).
                        Where(x => x.SuperCluster!.Number == clust.Number).
                        ToListAsync());
                }

                currentClusters = nextClusters;
                if (currentClusters.Count == 0)
                {
                    continue;
                }

                result.AddRange(currentClusters);
            }
            return result;
        }
    }
}
