using OutOfTimePrototype.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO;

public class LectureHallDto
{
    public Guid? Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public CampusBuildingDto HostBuilding { get; set; }
    [Required] public int Capacity { get; set; }

    public LectureHallDto()
    {

    }

    public LectureHallDto(LectureHall lectureHall)
    {
        Id = lectureHall.Id;
        Name = lectureHall.Name;
        HostBuilding = new CampusBuildingDto(lectureHall.HostBuilding);
        Capacity = lectureHall.Capacity;
    }
}

public class LectureHallCreateDto
{
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