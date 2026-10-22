using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace Web_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public CommentController(ICommentRepository commentRepo, IUserRepository userRepo, IPostRepository postRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }

    [HttpPost(Name = "AddComment")]
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

        return CreatedAtRoute("GetSingleComment", new { id = dto.Id }, dto);
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

        return Ok(dto);
    }

    [HttpDelete("{id:int}", Name = "DeleteComment")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);

        await commentRepo.DeleteAsync(comment.Id);

        return NoContent();
    }

    [HttpGet(Name = "GetManyComments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetManyAsync(
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

        List<CommentDto> dtos = comments.Select(c => new CommentDto()
        {
            Id = c.Id,
            Body = c.Body,
            PostId = c.PostId,
            UserId = c.UserId
        }).ToList();

        return Ok(dtos);
    }
    private async Task VerifyPostAndUserExistAsync(int postId, int userId)
    {
        await postRepo.GetSingleAsync(postId);
        await userRepo.GetSingleAsync(userId);
    }
}