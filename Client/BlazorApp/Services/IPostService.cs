using ApiContracts;

namespace BlazorApp.Services;

public interface IPostService
{
    public Task<PostDto> CreatePostAsync(PostDto postDto);
    public Task<PostDto> UpdatePostAsync(int id, PostDto postDto);
    public Task DeletePostAsync(int id);
    public Task<IEnumerable<PostDto>> GetPostsAsync();
    public Task<PostDto> GetPostAsync(int id);
}