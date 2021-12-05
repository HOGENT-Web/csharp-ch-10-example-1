using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStation.Shared;

namespace WeatherStation.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Administrator")]

    public class UserController : ControllerBase
    {
        private readonly ManagementApiClient _managementApiClient;

        public UserController(ManagementApiClient managementApiClient)
        {
            _managementApiClient = managementApiClient;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto.Index>> GetUsers()
        {
            var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());
            return users.Select(x => new UserDto.Index
            {
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Blocked = x.Blocked ?? false,
            });
        }

        [HttpPost]
        public async Task CreateUser(UserDto.Create user)
        {
            var createRequest = new UserCreateRequest
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = user.Password,
                Connection = "Username-Password-Authentication",
            };
            var createdUser = await _managementApiClient.Users.CreateAsync(createRequest);

            var allRoles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest());
            var adminRole = allRoles.First(x => x.Name == "Administrator");

            var assignRoleRequest = new AssignRolesRequest
            {
                Roles = new string[] { adminRole.Id }
            };
            await _managementApiClient.Users.AssignRolesAsync(createdUser?.UserId, assignRoleRequest);
        }
    }
}
