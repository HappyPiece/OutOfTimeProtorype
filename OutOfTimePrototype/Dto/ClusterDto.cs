using OutOfTimePrototype.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO
{
    public class ClusterDto
    {
        [Required]
        public string Number { get; set; } = "Undefined";

        public string? SuperClusterNumber { get; set; } = null;

        public ClusterDto() { }
        public ClusterDto(Cluster cluster)
        {
            Number = cluster.Number;
            SuperClusterNumber = cluster.SuperCluster?.Number;
        }
    }

    public class ClusterEditDto
    {
        public string? Number { get; set; }

        public string? SuperClusterNumber { get; set; } = null;
    }
}

