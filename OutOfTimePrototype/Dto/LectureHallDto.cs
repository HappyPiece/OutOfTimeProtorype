using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO;

public class LectureHallDto
{
    public Guid? Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public Guid HostBuildingId { get; set; }
    [Required] public int Capacity { get; set; }
}

public class LectureHallUpdateDto
{
    public string? Name { get; set; }
    public Guid? HostBuildingId { get; set; }
    public int? Capacity { get; set; }
}