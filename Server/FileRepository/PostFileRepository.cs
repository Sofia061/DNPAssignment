using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Post> AddAsync(Post post)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson) ?? new List<Post>();

        int maxId = posts.Count > 0 ? posts.Max(p => p.Id) : 0;
        post.Id = maxId + 1;

        posts.Add(post);
        postsAsJson = JsonSerializer.Serialize(posts, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, postsAsJson);

        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        string updatePostsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> postsUpdate = JsonSerializer.Deserialize<List<Post>>(updatePostsAsJson) ?? new List<Post>();
        int index = postsUpdate.FindIndex(p => p.Id == post.Id);

        if (index != -1)
        {
            postsUpdate[index] = post;
            string updatedJson = JsonSerializer.Serialize(postsUpdate, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
    }

    public async Task DeleteAsync(int id)
    {
        if (!File.Exists(filePath)) return;

        string json = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();
        int removedCount = posts.RemoveAll(p => p.Id == id);

        if (removedCount > 0)
        {
            string updatedJson = JsonSerializer.Serialize(posts, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        if (!File.Exists(filePath))
            throw new KeyNotFoundException($"Post with ID {id} not found.");

        string json = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();

        Post? post = posts.FirstOrDefault(p => p.Id == id);

        return post ?? throw new KeyNotFoundException($"Post with ID {id} not found.");
    }

    public IQueryable<Post> GetMany()
    {
        if (!File.Exists(filePath))
            return Enumerable.Empty<Post>().AsQueryable();

        string json = File.ReadAllText(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();

        return posts.AsQueryable();
    }
}