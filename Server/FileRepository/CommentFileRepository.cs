using System.Text.Json;
using Entities;
using RepositoryContracts;
namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 1;
        comment.Id = maxId + 1;
        comments.Add(comment);
        commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        string updateCommentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> commentsUpdate = JsonSerializer.Deserialize<List<Comment>>(updateCommentsAsJson) ?? new List<Comment>();
        int index = commentsUpdate.FindIndex(c => c.Id == comment.Id);

        if (index != -1)
        {
            commentsUpdate[index] = comment;
            string updatedJson = JsonSerializer.Serialize(commentsUpdate, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
    }

    public async Task DeleteAsync(int id)
    {
        if (!File.Exists(filePath)) return;

        string json = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(json) ?? new List<Comment>();
        int removedCount = comments.RemoveAll(c => c.Id == id);
        if (removedCount > 0)
        {
            string updatedJson = JsonSerializer.Serialize(comments, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        if (!File.Exists(filePath))
            throw new KeyNotFoundException($"Comment with ID {id} not found.");

        string json = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(json) ?? new List<Comment>();

        Comment? comment = comments.FirstOrDefault(c => c.Id == id);

        return comment ?? throw new KeyNotFoundException($"Comment with ID {id} not found.");
    }

    public IQueryable<Comment> GetMany()
    {
        if (!File.Exists(filePath))
            return Enumerable.Empty<Comment>().AsQueryable();

        string json = File.ReadAllText(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(json) ?? new List<Comment>();

        return comments.AsQueryable();
    }
}