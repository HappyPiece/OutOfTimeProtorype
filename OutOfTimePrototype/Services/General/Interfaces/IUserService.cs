using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Utilities;
using static OutOfTimePrototype.Utilities.UserUtilities;

namespace OutOfTimePrototype.Services.General.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUser(Guid id);
        Task<UserOperationResult> EditUser(Guid id, UserDto userDto);
        Task<UserOperationResult> DeleteUser(Guid id);
        Task<UserOperationResult> TryRegisterUser(UserDto userDto);
        Task<Result<List<Role>>> GetUnverifiedRoles(Guid id);
    }
}
