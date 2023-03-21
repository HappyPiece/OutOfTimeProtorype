using OutOfTimePrototype.DAL.Models;

namespace OutOfTimePrototype.DTO;

public class EducatorDto
{
    public Guid? Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = "Undefined";
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = "Undefined";

    public EducatorDto() { }

    public EducatorDto(Educator educator)
    {
        Id = educator.Id;
        FirstName = educator.FirstName;    
        MiddleName = educator.MiddleName;
        LastName = educator.LastName;
    }
}