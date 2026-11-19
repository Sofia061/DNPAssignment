using Microsoft.AspNetCore.Mvc;
using ApiContracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Web_API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest request)
    {
        User? user = await _userRepository.GetMany()
            .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

        if (user == null || user.Password != request.Password)
        {
            return Unauthorized("Invalid username or password.");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.Username
        };

        return Ok(userDto);
    }
}