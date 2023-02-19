using OutOfTimePrototype.DAL.Models;

namespace OutOfTimePrototype.Dal.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public List<Roles> ClaimedRoles { get; set; } = new List<Roles>();
        public List<Roles> VerifiedRoles { get; set; } = new List<Roles>();

        public string? RefreshToken { get; set; } = null;
        public DateTime? RefreshTokenExpiryTime { get; set; } = null;
    }

    public enum Roles
    {
        Student,
        Educator,
        ScheduleBureau,
        Admin,
        Root
    }

    public class Person : User
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
    }

    public class StudentUser : Person
    {
        public string? GradeBookNumber { get; set; }
        public Cluster? Cluster { get; set; }
    }

    public class EducatorUser : Person
    {
        public Educator? ScheduleSelf { get; set; }
    }

    public class ScheduleBureauUser : Person
    {

    }

    public class AdminUser : Person
    {

    }
}
