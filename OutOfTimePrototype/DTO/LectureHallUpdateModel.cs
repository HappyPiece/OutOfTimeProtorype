using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO;

public class LectureHallUpdateModel
{
    [Required] public Guid Id { get; set; }
    public string? Name { get; set; }
    public Guid? HostBuildingId { get; set; }
    public int? Capacity { get; set; }
}