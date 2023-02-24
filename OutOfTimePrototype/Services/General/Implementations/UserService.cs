using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Services.Interfaces;
using OutOfTimePrototype.Utilities;
using static OutOfTimePrototype.Utilities.UserUtilities;
using static OutOfTimePrototype.Utilities.UserUtilities.UserOperationResult;

namespace OutOfTimePrototype.Services.General.Implementations
{
    public class UserService : IUserService
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;
        private readonly IClusterService _clusterService;
        private readonly IMapper _mapper;

        public UserService(OutOfTimeDbContext outOfTimeDbContext, IClusterService clusterService, IMapper mapper)
        {
            _outOfTimeDbContext = outOfTimeDbContext;
            _clusterService = clusterService;
            _mapper = mapper;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _outOfTimeDbContext.Users.ToListAsync();
        }

        public async Task<User?> GetUser(Guid id)
        {
            return await _outOfTimeDbContext.Users.FindAsync(id);
        }

        public async Task<UserOperationResult> EditUser(Guid id, UserDto userDto)
        {
            var dbUser = await _outOfTimeDbContext.Users.FindAsync(id);

            if (dbUser is null)
            {
                return GenerateDefaultOperationResult(OperationStatus.NotFound, arg: id.ToString());
            }

            if (await _outOfTimeDbContext.Users.AnyAsync(user => user.Email == userDto.Email))
            {
                return GenerateDefaultOperationResult(OperationStatus.EmailAlreadyInUse, userDto.Email);
            }

            if (!await _outOfTimeDbContext.Clusters.AnyAsync(cluster => cluster.Number == userDto.ClusterNumber))
            {
                return GenerateDefaultOperationResult(OperationStatus.ClusterNotFound, userDto.ClusterNumber);
            }

            var updatedUser = _mapper.Map(userDto, dbUser);
            _outOfTimeDbContext.Users.Update(updatedUser);
            await _outOfTimeDbContext.SaveChangesAsync();

            return GenerateDefaultOperationResult(OperationStatus.UserEdited, id.ToString());
        }

        public async Task<UserOperationResult> DeleteUser(Guid id)
        {
            if (!await _outOfTimeDbContext.Users.AnyAsync(user => user.Id == id))
            {
                return GenerateDefaultOperationResult(OperationStatus.NotFound, id.ToString());
            }

            var userToDelete = new User { Id = id };
            _outOfTimeDbContext.Users.Remove(userToDelete);
            await _outOfTimeDbContext.SaveChangesAsync();

            return GenerateDefaultOperationResult(OperationStatus.UserDeleted, id.ToString());
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

            _outOfTimeDbContext.Users.Add(user);
            await _outOfTimeDbContext.SaveChangesAsync();

            return GenerateDefaultOperationResult(OperationStatus.UserRegistered, user.Id.ToString());
        }
        
        public async Task<Result> VerifyUserRole(List<Role> examinerRoles, Guid userToVerifyId, Role userRole)
        {
            if (!examinerRoles.Any(x => x.CanAssign(userRole)))
                return new AccessNotAllowedException($"User with roles '{examinerRoles}' cannot perform this action");

            var userToApprove = await GetUser(userToVerifyId);
            if (userToApprove is null)
                return new RecordNotFoundException($"User with id '{userToVerifyId.ToString()}' not found");
            
            userToApprove.VerifiedRoles.Add(userRole);
            userToApprove.ClaimedRoles.Remove(userRole);
            _outOfTimeDbContext.Users.Update(userToApprove);
            await _outOfTimeDbContext.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<List<Role>>> GetUnverifiedRoles(Guid id)
        {
            var user = await _outOfTimeDbContext.Users.FindAsync(id);

            if (user is null)
            {
                return new RecordNotFoundException($"User with id '{id.ToString()}' does not exists");
            }

            return user.ClaimedRoles;
        }
    }
}