using OutOfTimePrototype.DAL.Models;

namespace OutOfTimePrototype.DTO;

public class CampusBuildingDto
{
    public Guid? Id { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public List<Guid> LectureHallIds { get; set; } = new List<Guid>();

    public CampusBuildingDto()
    {

    }

    public CampusBuildingDto(CampusBuilding campusBuilding)
    {
        Id = campusBuilding.Id;
        Address = campusBuilding.Address;
        Name = campusBuilding.Name;
        LectureHallIds = campusBuilding.LectureHalls.Select(x => x.Id).ToList();
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