using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }
    public async Task<User> AddAsync(User user)
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>();

        int maxId = users.Count > 0 ? users.Max(u => u.Id) : 0;
        user.Id = maxId + 1;

        users.Add(user);
        usersAsJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, usersAsJson);

        return user;
    }

    public async Task UpdateAsync(User user)
    {
        string updateUsersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> usersUpdate = JsonSerializer.Deserialize<List<User>>(updateUsersAsJson) ?? new List<User>();
        int index = usersUpdate.FindIndex(u => u.Id == user.Id);

        if (index != -1)
        {
            usersUpdate[index] = user;
            string updatedJson = JsonSerializer.Serialize(usersUpdate, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
    }

    public async Task DeleteAsync(int id)
    {
        if (!File.Exists(filePath)) return;

        string json = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        int removedCount = users.RemoveAll(u => u.Id == id);

        if (removedCount > 0)
        {
            string updatedJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
    }

    public async Task<User> GetSingleAsync(int id)
    {
        if (!File.Exists(filePath))
            throw new KeyNotFoundException($"User with ID {id} not found.");

        string json = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();

        User? user = users.FirstOrDefault(u => u.Id == id);

        return user ?? throw new KeyNotFoundException($"User with ID {id} not found.");
    }

    public IQueryable<User> GetMany()
    {
        if (!File.Exists(filePath))
            return Enumerable.Empty<User>().AsQueryable();

        string json = File.ReadAllText(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();

        return users.AsQueryable();
    }
}