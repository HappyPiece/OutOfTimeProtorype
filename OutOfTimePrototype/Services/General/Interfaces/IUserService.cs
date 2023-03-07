using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Utilities;
using static OutOfTimePrototype.Utilities.UserUtilities;

namespace OutOfTimePrototype.Services.General.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetUser(Guid id);
        Task<UserOperationResult> EditUser(Guid id, UserDto userDto);
        Task<UserOperationResult> EditUserPassword(Guid id, string password);
        Task<UserOperationResult> DeleteUser(Guid id);
        Task<UserOperationResult> TryRegisterUser(UserDto userDto);
        Task<Result<List<Role>>> GetUnverifiedRoles(Guid id);

        /// <summary>
        /// Method to verify user role by examiner  
        /// </summary>
        /// <param name="examinerRoles">Roles of examiner to check if he can perform verify action</param>
        /// <param name="userId">The user whose role is in the confirmation process</param>
        /// <param name="userRoleToApprove">User's role</param>
        /// <returns><c>Result</c> which represents Fail or Success states of method</returns>
        /// <remarks><c>Result</c> can contain either <see cref="AccessNotAllowedException"/> or
        /// <see cref="RecordNotFoundException"/></remarks>
        Task<Result> VerifyUserRole(List<Role> examinerRoles, Guid userId, Role userRoleToApprove);

        Task<Result> RejectUserRole(List<Role> examinerRoles, Guid userId, Role userRoleToReject);

        Task<List<User>> GetAllUsersWithUnverifiedRoles();
    }
}