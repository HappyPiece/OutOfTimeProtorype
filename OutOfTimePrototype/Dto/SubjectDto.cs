using OutOfTimePrototype.DAL.Models;

namespace OutOfTimePrototype.DTO;

/// <summary>
/// This dto is needed in order to make it easier to expand in future
/// </summary>
public class SubjectDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }

    public SubjectDto()
    {

    }

    public SubjectDto (Subject subject)
    {
        Id = subject.Id;
        Name = subject.Name;
    }
}