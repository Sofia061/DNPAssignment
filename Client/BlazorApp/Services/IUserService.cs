using ApiContracts;

namespace BlazorApp.Services;

public interface IUserService
{
    public Task<UserDto> AddUserAsync(UserDto request);
    public Task UpdateUserAsync(int id, UserDto request);
    public Task DeleteUserAsync(int id);
    public Task<UserDto> GetUserAsync(int id);
    public Task<IEnumerable<UserDto>> GetUsersAsync();
}