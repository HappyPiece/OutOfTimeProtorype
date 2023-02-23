using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.Dto;
using System.Net;
using static OutOfTimePrototype.Utilities.ClassUtilities;
using static OutOfTimePrototype.Utilities.UserUtilities;

namespace OutOfTimePrototype.Utilities
{
    public static class UserUtilities
    {
        public class UserOperationResult
        {
            public OperationStatus? Status { get; set; }
            public string? Message { get; set; }
            public List<User>? QueryResult { get; set; }

            public User? User { get; set; }

            public HttpStatusCode HttpStatusCode { get; set; }

            public UserOperationResult(OperationStatus operationStatus, HttpStatusCode httpStatusCode,
                string? errorMessage = null, List<User>? queryResult = null, User? user = null)
            {
                Status = operationStatus;
                Message = errorMessage;
                HttpStatusCode = httpStatusCode;
                QueryResult = queryResult;
                User = user;
            }

            public enum OperationStatus
            {
                Success,
                UserRegistered,
                EmailAlreadyInUse,
                UserEdited,
                UserDeleted,
                ClusterNotFound,
                NotFound
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="status"></param>
            /// <param name="arg"></param>
            /// <param name="queryResult"></param>
            /// <param name="user"></param>
            /// <returns></returns>
            public static UserOperationResult GenerateDefaultOperationResult(OperationStatus status, string? arg = null,
                List<User>? queryResult = null, User? user = null)
            {
                var message = "";
                var httpStatusCode = HttpStatusCode.InternalServerError;
                switch (status)
                {
                    case OperationStatus.Success:
                    {
                        message = "Operation successful";
                        httpStatusCode = HttpStatusCode.OK;
                        break;
                    }
                    case OperationStatus.UserRegistered:
                    {
                        message = $"User was successfully created, Id {arg}";
                        httpStatusCode = HttpStatusCode.Created;
                        break;
                    }
                    case OperationStatus.UserEdited:
                    {
                        message = $"User with id {arg} was successfully edited";
                        httpStatusCode = HttpStatusCode.OK;
                        break;
                    }
                    case OperationStatus.UserDeleted:
                    {
                        message = $"User with id {arg} was successfully deleted";
                        httpStatusCode = HttpStatusCode.OK;
                        break;
                    }
                    case OperationStatus.EmailAlreadyInUse:
                    {
                        message = $"User with Email {arg} already exists";
                        httpStatusCode = HttpStatusCode.Conflict;
                        break;
                    }
                    case OperationStatus.ClusterNotFound:
                    {
                        message = $"Cannot assign cluster number {arg} since it doesn't exist";
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;
                    }
                    case OperationStatus.NotFound:
                    {
                        message = $"User with id {arg} does not exist";
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(status), status, null);
                }

                return new UserOperationResult(status, httpStatusCode, errorMessage: message, queryResult: queryResult,
                    user: user);
            }
        }

        public static class RoleUtilities
        {
            public static readonly Dictionary<Role, List<Role>> RoleHierarchy = new()
            {
                { Role.Root, new List<Role> { Role.Admin, Role.ScheduleBureau, Role.Educator, Role.Student } },
                { Role.Admin, new List<Role> { Role.ScheduleBureau, Role.Educator, Role.Student } },
                { Role.ScheduleBureau, new List<Role> {Role.Educator, Role.Student} },
                { Role.Educator, new List<Role>() },
                { Role.Student, new List<Role>() }
            };
        }
    }

    public static class RoleExtensions
    {
        public static bool IsHigherOrEqualPermissions(this Role role, Role otherRole)
        {
            if (role == otherRole) return true;
            
            return RoleUtilities.RoleHierarchy
                .TryGetValue(role, out var canAssign) && canAssign.Contains(otherRole);
        }
        
        public static bool CanAssign(this Role role, Role otherRole)
        {
            return RoleUtilities.RoleHierarchy
                .TryGetValue(role, out var roles) && roles.Contains(otherRole);
        }
    }
}