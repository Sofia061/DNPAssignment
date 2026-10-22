using ApiContracts;
using Entities;
using FileRepositories;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace Web_API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await userRepo.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch.");
        }

        User existingUser = await userRepo.GetSingleAsync(id);
        existingUser.Username = dto.UserName;

        await userRepo.UpdateAsync(existingUser);
        return Ok(dto);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetManyAsync([FromQuery] string? usernameContains)
    {
        IQueryable<User> query = userRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(usernameContains))
        {
            query = query.Where(u => u.Username.Contains(usernameContains, StringComparison.OrdinalIgnoreCase));
        }

        var userDtos = query.ToList()
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.Username
            })
            .ToList();

        return Ok(userDtos);
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