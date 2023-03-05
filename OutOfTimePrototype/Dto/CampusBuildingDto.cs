using OutOfTimePrototype.DAL.Models;

namespace OutOfTimePrototype.DTO;

public class CampusBuildingDto
{
    public Guid? Id { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public List<LectureHallDto> LectureHalls { get; set; }

    public CampusBuildingDto()
    {

    }

    public CampusBuildingDto(CampusBuilding campusBuilding)
    {
        Id = campusBuilding.Id;
        Address = campusBuilding.Address;
        Name = campusBuilding.Name;
        LectureHalls = campusBuilding.LectureHalls.Select(x => new LectureHallDto(x)).ToList();
    }
}
public class CampusBuildingCreateDto
{
    public string? Address { get; set; }

    public string? Name { get; set; }
}

public class CampusBuildingEditDto
{
    public Guid? Id { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }
}