using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> AddUserAsync(UserDto request)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/user", request);
        
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to add user: {error}");
        }

        return await response.Content.ReadFromJsonAsync<UserDto>() 
               ?? throw new InvalidOperationException("Failed to deserialize user response.");
    }
    
    public async Task UpdateUserAsync(int id, UserDto userDto)
    {
        var response = await client.PutAsJsonAsync($"api/user/{id}", userDto);
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to update user: {error}");
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        var response = await client.DeleteAsync($"api/user/{id}");
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete user: {error}");
        }
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        return await client.GetFromJsonAsync<UserDto>($"api/user/{id}") 
               ?? throw new KeyNotFoundException($"User with ID {id} was not found.");
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        return await client.GetFromJsonAsync<IEnumerable<UserDto>>("api/user") 
               ?? Enumerable.Empty<UserDto>();
    }
}