using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CommentDto> CreateCommentAsync(CommentDto commentDto)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/comment", commentDto);
        
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create comment: {error}");
        }

        return await response.Content.ReadFromJsonAsync<CommentDto>() 
               ?? throw new InvalidOperationException("Failed to deserialize created comment response.");
    }

    public async Task<CommentDto> UpdateCommentAsync(int id, CommentDto commentDto)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync($"api/comment/{id}", commentDto);
        
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to update comment {id}: {error}");
        }

        return await response.Content.ReadFromJsonAsync<CommentDto>() 
               ?? throw new InvalidOperationException("Failed to deserialize updated comment response.");
    }

    public async Task DeleteCommentAsync(int id)
    {
        HttpResponseMessage response = await client.DeleteAsync($"api/comment/{id}");
        
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete comment {id}: {error}");
        }
    }

    public async Task<CommentDto> GetCommentAsync(int id)
    {
        return await client.GetFromJsonAsync<CommentDto>($"api/comment/{id}") 
               ?? throw new KeyNotFoundException($"Comment with ID {id} was not found.");
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsAsync()
    {
        return await client.GetFromJsonAsync<IEnumerable<CommentDto>>("api/comment") 
               ?? Enumerable.Empty<CommentDto>();
    }
    
    public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId)
    {
        return await client.GetFromJsonAsync<IEnumerable<CommentDto>>($"api/comment?postId={postId}") 
               ?? Enumerable.Empty<CommentDto>();
    }
}