namespace OutOfTimePrototype.DTO;

public class CampusBuildingDto
{
    public Guid? Id { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public List<Guid>? LectureHallsIds { get; set; }
}