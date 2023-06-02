using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServiceApp.Data;
using GrpcServiceApp.Protos;
using Microsoft.EntityFrameworkCore;

namespace GrpcServiceApp.Services
{
    public class UserApiService : UserServie.UserServieBase
    {
        private readonly ApplicationDbContext _dbContext;
        public UserApiService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public override async Task<GetAllUsers> ListUsers(Empty request, ServerCallContext context)
        {
            var allUsers = new GetAllUsers();

            var items = await _dbContext.Users.Select(user => new User { Id = user.Id, Name = user.Name, Age = user.Age }).ToListAsync();

            allUsers.Users.AddRange(items);

            return await Task.FromResult(allUsers);
        }
        public override async Task<User> GetUser(GetUserRequest request, ServerCallContext context)
        {
            var user = await _dbContext.Users.FindAsync(request.Id);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }
            var model = new User() { Id = user.Id, Name = user.Name, Age = user.Age };

            return await Task.FromResult(model);
        }
        public override async Task<User> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var user = new Models.User{Name = request.Name, Age = request.Age };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var model = new User{ Id = user.Id, Name = user.Name, Age = user.Age };

            return await Task.FromResult(model);
        }

        public override async Task<User> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            var user = await _dbContext.Users.FindAsync(request.Id);

            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            var model = new User { Id = user.Id, Name = user.Name, Age = user.Age };
            return await Task.FromResult(model);
        }

        public override async Task<User> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            var user = await _dbContext.Users.FindAsync(request.Id);

            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            user.Name = request.Name;
            user.Age = request.Age;

            await _dbContext.SaveChangesAsync();

            var model = new User{ Id = user.Id, Name = user.Name, Age = user.Age };

            return await Task.FromResult(model);
        }
    }

}
