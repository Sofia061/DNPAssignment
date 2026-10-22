using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace Web_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public PostController(IPostRepository postRepo, IUserRepository userRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] PostDto request)
    {
        await VerifyUserExistAsync(request.UserId);

        Post post = new(request.Title, request.Body, request.UserId);
        Post created = await postRepo.AddAsync(post);

        PostDto dto = new()
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId
        };

        return CreatedAtRoute("GetSinglePost", new { id = dto.Id }, dto);
    }
    
    [HttpGet("{id:int}", Name = "GetSinglePost")]
    public async Task<IActionResult> GetSingleAsync(int id)
    {
        Post post = await postRepo.GetSingleAsync(id);

        PostDto dto = new()
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        };

        return Ok(dto);
    }

    [HttpPut("{id:int}", Name = "UpdatePost")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] PostDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { error = "Route ID does not match body ID." });
        }
        Post existingPost = await postRepo.GetSingleAsync(id);

        existingPost.Title = dto.Title;
        existingPost.Body = dto.Body;

        await postRepo.UpdateAsync(existingPost);

        return Ok(dto);
    }

    [HttpDelete("{id:int}", Name = "DeletePost")]
    public async Task<IActionResult> DeletePost(int id)
    {
        Post post = await postRepo.GetSingleAsync(id);

        await postRepo.DeleteAsync(post.Id);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetManyAsync([FromQuery] int? userId)
    {
        IQueryable<Post> query = postRepo.GetMany();

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        var postDtos = query.ToList()
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId
            })
            .ToList();

        return Ok(postDtos);
    }

    private async Task VerifyUserExistAsync(int userId)
    {
        await userRepo.GetSingleAsync(userId);
    }
}