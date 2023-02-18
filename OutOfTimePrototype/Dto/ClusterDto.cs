using OutOfTimePrototype.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO
{
    public class ClusterDto
    {
        [Required]
        public string Number { get; set; }

        public string? SuperClusterNumber { get; set; } = null;
    }

    public class ClusterEditDTO
    {
        public string? Number { get; set; }

        public string? SuperClusterNumber { get; set; } = null;
    }
}
