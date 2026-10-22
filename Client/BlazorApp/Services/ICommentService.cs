using ApiContracts;

namespace BlazorApp.Services;

public interface ICommentService
{
    public Task<CommentDto> CreateCommentAsync(CommentDto commentDto);
    public Task<CommentDto> UpdateCommentAsync(int id, CommentDto commentDto);
    public Task DeleteCommentAsync(int id);
    public Task<CommentDto> GetCommentAsync(int id);
    public Task<IEnumerable<CommentDto>> GetCommentsAsync();
    public Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId);
}