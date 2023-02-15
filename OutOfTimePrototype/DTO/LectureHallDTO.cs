using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO;

public class LectureHallDTO
{
    [Required] public string Name { get; set; }
    [Required] public Guid HostBuildingId { get; set; }
    public int Capacity { get; set; }
}