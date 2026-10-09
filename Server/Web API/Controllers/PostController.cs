using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace Web_API.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public PostController(IPostRepository postRepo)
    {
        this.postRepo = postRepo;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
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

        return CreatedAtAction(nameof(GetSingleAsync), new { id = dto.Id }, dto);
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

        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "DeletePost")]
    public async Task<IActionResult> DeletePost(int id)
    {
        Post post = await postRepo.GetSingleAsync(id);

        await postRepo.DeleteAsync(post.Id);

        return NoContent();
    }

    [HttpGet(Name = "GetManyPosts")]
    public async Task<IActionResult> GetManyAsync(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] string? titleContains)
    {
        IQueryable<Post> query = postRepo.GetMany();

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            var matchingUserIds = userRepo.GetMany()
                .Where(u => u.Username.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id);

            query = query.Where(p => matchingUserIds.Contains(p.UserId));
        }
        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            query = query.Where(p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
        }

        List<Post> posts = query.ToList();

        var response = new
        {
            posts = posts.Select(p => new
            {
                p.Id,
                p.Title,
                p.Body,
                p.UserId,
                links = new[]
                {
                    new { href = Url.Link("GetSinglePost", new { id = p.Id }), rel = "self", method = "GET" },
                    new { href = Url.Link("UpdatePost", new { id = p.Id }), rel = "update", method = "PUT" },
                    new { href = Url.Link("DeletePost", new { id = p.Id }), rel = "delete", method = "DELETE" }
                }
            }),
            links = new[]
            {
                new { href = Url.Link("GetManyPosts", null), rel = "self", method = "GET" },
                new { href = Url.Link("AddPost", null), rel = "create", method = "POST" }
            }
        };

        return Ok(response);
    }

    private async Task VerifyUserExistAsync(int userId)
    {
        await userRepo.GetSingleAsync(userId);
    }
}