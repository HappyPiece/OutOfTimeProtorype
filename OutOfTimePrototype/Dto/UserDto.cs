using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DAL.Models;

namespace OutOfTimePrototype.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class PersonDto : UserDto
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
    }

    public class StudentUserDto : PersonDto
    {
        public string? GradeBookNumber { get; set; }
        public Cluster? Cluster { get; set; }
    }

    public class EducatorUserDto : PersonDto
    {
        public Educator? ScheduleSelf { get; set; }
    }

    public class ScheduleBureauUserDto : PersonDto
    {

    }

    public class AdminUserDto : PersonDto
    {

    }
}
