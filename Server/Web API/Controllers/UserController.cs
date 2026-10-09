using ApiContracts;
using Entities;
using FileRepositories;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace Web_API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository userRepo;
    
    public UserController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }
    
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        await VerifyUserNameIsAvailableAsync(request.UserName);

        User user = new(request.UserName, request.Password);
        User created = await userRepo.AddAsync(user);
        UserDto dto = new()
        {
            Id = created.Id,
            UserName = created.Username
        };
        return Created($"/Users/{dto.Id}", created);
    }

    [HttpGet("{id:int}", Name = "GetSingleUser")]
    public async Task<IActionResult> GetSingleAsync(int id)
    {
        User user = await userRepo.GetSingleAsync(id);

        UserDto dto = new()
        {
            Id = user.Id,
            UserName = user.Username
        };

        return Ok(dto);
    }

    [HttpPut("{id:int}", Name = "UpdateUser")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { error = "Route ID does not match body ID." });
        }

        User existingUser = await userRepo.GetSingleAsync(id);

        existingUser.Username = dto.UserName;

        await userRepo.UpdateAsync(existingUser);

        return NoContent();
    }
    
    [HttpDelete("{id:int}", Name = "DeleteUser")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        User user = await userRepo.GetSingleAsync(id);

        await userRepo.DeleteAsync(user.Id);

        return NoContent();
    }
    
    [HttpGet(Name = "GetManyUsers")]
    public async Task<IActionResult> GetManyAsync([FromQuery] string? usernameContains)
    {
        IQueryable<User> query = userRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(usernameContains))
        {
            query = query.Where(u => u.Username.Contains(usernameContains, StringComparison.OrdinalIgnoreCase));
        }

        List<User> users = query.ToList();

        var response = new
        {
            users = users.Select(u => new
            {
                u.Id,
                u.Username,
                links = new[]
                {
                    new { href = Url.Link("GetSingleUser", new { id = u.Id }), rel = "self", method = "GET" },
                    new { href = Url.Link("UpdateUser", new { id = u.Id }), rel = "update", method = "PUT" },
                    new { href = Url.Link("DeleteUser", new { id = u.Id }), rel = "delete", method = "DELETE" },
                    new { href = Url.Link("GetManyPosts", new { userId = u.Id }), rel = "posts", method = "GET" },
                    new { href = Url.Link("GetManyComments", new { userId = u.Id }), rel = "comments", method = "GET" }
                }
            }),
            links = new[]
            {
                new { href = Url.Link("GetManyUsers", null), rel = "self", method = "GET" },
                new { href = Url.Link("AddUser", null), rel = "create", method = "POST" }
            }
        };

        return Ok(response);
    }

    private async Task VerifyUserNameIsAvailableAsync(string userName)
    {
        bool exists = userRepo.GetMany()
            .Any(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            throw new InvalidOperationException($"Username '{userName}' is already taken.");
        }

        await Task.CompletedTask;
    }
}