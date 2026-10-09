using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace Web_API.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public CommentController(ICommentRepository commentRepo)
    {
        this.commentRepo = commentRepo;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] CreateCommentDto request)
    {
        await VerifyPostAndUserExistAsync(request.PostId, request.UserId);

        Comment comment = new(request.Body, request.PostId, request.UserId);
        Comment created = await commentRepo.AddAsync(comment);

        CommentDto dto = new()
        {
            Id = created.Id,
            Body = created.Body,
            PostId = created.PostId,
            UserId = created.UserId
        };

        return Created($"/Comments/{dto.Id}", dto);
    }

    [HttpGet("{id:int}", Name = "GetSingleComment")]
    public async Task<IActionResult> GetSingleAsync(int id)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);

        CommentDto dto = new()
        {
            Id = comment.Id,
            Body = comment.Body,
            PostId = comment.PostId,
            UserId = comment.UserId
        };

        return Ok(dto);
    }

    [HttpPut("{id:int}", Name = "UpdateComment")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { error = "Route ID does not match body ID." });
        }

        Comment existingComment = await commentRepo.GetSingleAsync(id);
        existingComment.Body = dto.Body;
        await commentRepo.UpdateAsync(existingComment);

        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "DeleteComment")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);

        await commentRepo.DeleteAsync(comment.Id);

        return NoContent();
    }

    [HttpGet(Name = "GetManyComments")]
    public async Task<IActionResult> GetManyAsync(
        [FromQuery] int? postId,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Comment> query = commentRepo.GetMany();

        if (postId.HasValue)
        {
            query = query.Where(c => c.PostId == postId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(c => c.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            var matchingUserIds = userRepo.GetMany()
                .Where(u => u.Username.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id);

            query = query.Where(c => matchingUserIds.Contains(c.UserId));
        }

        List<Comment> comments = query.ToList();

        var response = new
        {
            comments = comments.Select(c => new
            {
                c.Id,
                c.Body,
                c.PostId,
                c.UserId,
                links = new[]
                {
                    new { href = Url.Link("GetSingleComment", new { id = c.Id }), rel = "self", method = "GET" },
                    new { href = Url.Link("UpdateComment", new { id = c.Id }), rel = "update", method = "PUT" },
                    new { href = Url.Link("DeleteComment", new { id = c.Id }), rel = "delete", method = "DELETE" }
                }
            }),
            links = new[]
            {
                new { href = Url.Link("GetManyComments", null), rel = "self", method = "GET" },
                new { href = Url.Link("AddComment", null), rel = "create", method = "POST" }
            }
        };

        return Ok(response);
    }

    private async Task VerifyPostAndUserExistAsync(int postId, int userId)
    {
        await postRepo.GetSingleAsync(postId);
        await userRepo.GetSingleAsync(userId);
    }
}