using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Dal.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Email { get; set; } = "Undefined";

        public string Password { get; set; } = "Undefined";

        public List<Role> ClaimedRoles { get; set; } = new List<Role>();
        public List<Role> VerifiedRoles { get; set; } = new List<Role>();

        public AccountType AccountType { get; set; } = AccountType.Default;

        public string? RefreshToken { get; set; } = null;
        public DateTime? RefreshTokenExpiryTime { get; set; } = null;

        // Person properties >>>
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        // <<< Person properties

        // Student properties >>>
        public string? GradeBookNumber { get; set; }
        public Cluster? Cluster { get; set; }

        // <<< Student properties

        // Student properties >>>
        public Educator? ScheduleSelf { get; set; }

        // <<< Student properties

        public User() { }

        public User(UserDto userDto, Cluster? cluster = null, Educator? scheduleSelf = null)
        {
            Id = userDto.Id;
            Email = userDto.Email;
            Password = userDto.Password ??
                       throw new ArgumentNullException("userDto", "Attempted to initialize User without a password");
            ClaimedRoles = userDto.ClaimedRoles;
            VerifiedRoles = userDto.VerifiedRoles;
            AccountType = userDto.AccountType;
            FirstName = userDto.FirstName;
            MiddleName = userDto.MiddleName;
            LastName = userDto.LastName;
            GradeBookNumber = userDto.GradeBookNumber;
            Cluster = cluster;
            ScheduleSelf = scheduleSelf;
        }

        public static class Initialize
        {
            public static User Default(UserDto userDto)
            {
                var newUser = new User
                {
                    Email = userDto.Email,
                    Password = HashingHelper.ComputeSha256Hash(userDto.Password)
                };

                return newUser;
            }

            public static User Person(UserDto userDto)
            {
                var newUser = Default(userDto);

                newUser.FirstName = userDto.FirstName;
                newUser.MiddleName = userDto.MiddleName;
                newUser.LastName = userDto.LastName;

                return newUser;
            }

            public static User Student(UserDto userDto, Cluster? cluster = null)
            {
                var newUser = Person(userDto);

                newUser.VerifiedRoles = new List<Role> { Role.Student };
                newUser.AccountType = AccountType.Student;

                newUser.GradeBookNumber = userDto.GradeBookNumber;
                newUser.Cluster = cluster;

                return newUser;
            }

            public static User Educator(UserDto userDto, Cluster? cluster = null)
            {
                var newUser = Person(userDto);

                newUser.ClaimedRoles = new List<Role> { Role.Educator };
                newUser.AccountType = AccountType.Educator;

                return newUser;
            }

            public static User ScheduleBureau(UserDto userDto)
            {
                var newUser = Person(userDto);

                newUser.ClaimedRoles = new List<Role> { Role.ScheduleBureau };
                newUser.AccountType = AccountType.ScheduleBureau;

                return newUser;
            }
            
            public static User Admin(UserDto userDto)
            {
                var newUser = Person(userDto);
                
                newUser.ClaimedRoles = new List<Role> { Role.Admin };
                newUser.AccountType = AccountType.Admin;

                return newUser;
            }
        }
    }

    public enum AccountType
    {
        Default,
        Admin,
        ScheduleBureau,
        Educator,
        Student
    }

    public enum Role
    {
        Root,
        Admin,
        ScheduleBureau,
        Educator,
        Student
    }
}