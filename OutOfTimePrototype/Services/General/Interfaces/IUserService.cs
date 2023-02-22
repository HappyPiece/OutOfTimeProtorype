using OutOfTimePrototype.Dto;
using static OutOfTimePrototype.Utilities.UserUtilities;

namespace OutOfTimePrototype.Services.General.Interfaces
{
    public interface IUserService
    {
        Task<UserOperationResult> TryGetUser(Guid id);
        Task<UserOperationResult> TryRegisterUser(UserDto userDto);
    }
}
