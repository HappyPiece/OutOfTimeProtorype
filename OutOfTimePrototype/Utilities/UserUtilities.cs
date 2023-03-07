using System.Net;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.Dto;
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
            /// Builder method to easily create different instances of <c>UserOperationResult</c> class
            /// </summary>
            /// <param name="status"><c>OperationStatus</c> representing state of operation with user</param>
            /// <param name="arg">Argument to pass to the message</param>
            /// <param name="queryResult"></param>
            /// <param name="user"><c>User</c> instance</param>
            /// <returns><c>UserOperationResult</c> instance with different <c>message</c> and <c>httpStatusCode</c> depending on
            /// arguments passed to the function</returns>
            public static UserOperationResult GenerateDefaultOperationResult(OperationStatus status, string? arg = null,
                List<User>? queryResult = null, User? user = null)
            {
                string message;
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

            public static IActionResult ToIActionResult(UserOperationResult userOperationResult)
            {
                switch (userOperationResult.Status)
                {
                    case OperationStatus.Success:
                        if (userOperationResult.User is not null)
                            return new OkObjectResult(new UserDto(userOperationResult.User));
                        return new OkResult();
                    case OperationStatus.UserRegistered:
                        if (userOperationResult.User is not null)
                            return new CreatedResult($"api/users/{userOperationResult.User.Id}",
                                userOperationResult.User);
                        return new NoContentResult();
                    case OperationStatus.UserEdited:
                    case OperationStatus.UserDeleted:
                        return new NoContentResult();
                    case OperationStatus.NotFound:
                        if (userOperationResult.Message is not null)
                            return new NotFoundObjectResult(userOperationResult.Message);
                        return new NotFoundResult();
                    case OperationStatus.ClusterNotFound:
                        if (userOperationResult.Message is not null)
                            return new BadRequestObjectResult(userOperationResult.Message);
                        return new BadRequestResult();
                    case OperationStatus.EmailAlreadyInUse:
                        if (userOperationResult.User is not null)
                            return new ConflictObjectResult(userOperationResult.User.Email);
                        return new ConflictResult();
                    default:
                        return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public static class RoleUtilities
        {
            public static readonly Dictionary<Role, List<Role>> RoleHierarchy = new()
            {
                { Role.Root, new List<Role> { Role.Admin, Role.ScheduleBureau, Role.Educator, Role.Student } },
                { Role.Admin, new List<Role> { Role.ScheduleBureau, Role.Educator, Role.Student } },
                { Role.ScheduleBureau, new List<Role> { Role.Educator, Role.Student } },
                { Role.Educator, new List<Role>() },
                { Role.Student, new List<Role>() }
            };
        }
    }
}