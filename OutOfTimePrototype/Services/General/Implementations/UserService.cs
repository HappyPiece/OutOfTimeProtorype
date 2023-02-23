using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.Configurations;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Services.Interfaces;
using OutOfTimePrototype.Utilities;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Extensions;
using static OutOfTimePrototype.Utilities.UserUtilities;
using static OutOfTimePrototype.Utilities.UserUtilities.UserOperationResult;
using Role = OutOfTimePrototype.Dal.Models.Role;

namespace OutOfTimePrototype.Services.General.Implementations
{
    public class UserService : IUserService
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;
        private readonly IClusterService _clusterService;

        public UserService(OutOfTimeDbContext outOfTimeDbContext, IClusterService clusterService)
        {
            _outOfTimeDbContext = outOfTimeDbContext;
            _clusterService = clusterService;
        }

        public async Task<UserOperationResult> TryGetUser(Guid id)
        {
            var user = await _outOfTimeDbContext.Users.SingleOrDefaultAsync(x => x.Id == id);
            return user == null
                ? GenerateDefaultOperationResult(OperationStatus.NotFound, id.ToString())
                : GenerateDefaultOperationResult(OperationStatus.Success, user: user);

            throw new InvalidEnumArgumentException(user.AccountType.ToString());
        }
        
        public async Task<UserOperationResult> TryRegisterUser(UserDto userDto)
        {
            if (await _outOfTimeDbContext.Users.AnyAsync(x => (x.Email == userDto.Email)))
            {
                return GenerateDefaultOperationResult(OperationStatus.EmailAlreadyInUse, arg: userDto.Email);
            }

            User user;

            switch (userDto.AccountType)
            {
                case AccountType.Student:
                {
                    Cluster? cluster = null;
                    if (userDto.ClusterNumber is not null)
                    {
                        cluster = await _clusterService.TryGetCluster(userDto.ClusterNumber);
                        if (cluster is null)
                        {
                            return GenerateDefaultOperationResult(OperationStatus.ClusterNotFound,
                                arg: userDto.ClusterNumber);
                        }
                    }

                    user = User.Initialize.Student(userDto, cluster);
                    break;
                }
                case AccountType.Educator:
                    user = User.Initialize.Educator(userDto);
                    break;
                case AccountType.Default:
                case AccountType.Admin:
                case AccountType.ScheduleBureau:
                default:
                    user = User.Initialize.Default(userDto);
                    break;
            }

            await _outOfTimeDbContext.Users.AddAsync(user);
            await _outOfTimeDbContext.SaveChangesAsync();

            return GenerateDefaultOperationResult(OperationStatus.UserRegistered, user.Id.ToString());
        }
    }
}