using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> CreatePostAsync(PostDto createPostDto)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/post", createPostDto);
    
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create post: {error}");
        }

        return await response.Content.ReadFromJsonAsync<PostDto>() 
               ?? throw new InvalidOperationException("Failed to deserialize created post response.");
    }

    public async Task<PostDto> UpdatePostAsync(int id, PostDto postDto)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync($"api/post/{id}", postDto);
        
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to update post {id}: {error}");
        }

        return await response.Content.ReadFromJsonAsync<PostDto>() 
               ?? throw new InvalidOperationException("Failed to deserialize updated post response.");
    }

    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage response = await client.DeleteAsync($"api/post/{id}");
        
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete post {id}: {error}");
        }
    }

    public async Task<IEnumerable<PostDto>> GetPostsAsync()
    {
        return await client.GetFromJsonAsync<IEnumerable<PostDto>>("api/post") 
               ?? Enumerable.Empty<PostDto>();
    }

    public async Task<PostDto> GetPostAsync(int id)
    {
        return await client.GetFromJsonAsync<PostDto>($"api/post/{id}") 
               ?? throw new KeyNotFoundException($"Post with ID {id} was not found.");
    }
}