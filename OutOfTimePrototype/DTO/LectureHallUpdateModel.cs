using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO;

public class LectureHallUpdateModel
{
    public string? Name { get; set; }
    public Guid? HostBuildingId { get; set; }
    public int? Capacity { get; set; }
}