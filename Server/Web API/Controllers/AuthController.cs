using Microsoft.AspNetCore.Mvc;
using ApiContracts;
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
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        var user = _userRepository.GetMany()
            .FirstOrDefault(u => string.Equals(u.Username, request.Username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        if (user.Password != request.Password)
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