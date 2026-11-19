namespace Entities;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    private Comment() { }

    public Comment(string body, int postId, int userId)
    {
        Body = body;
        PostId = postId;
        UserId = userId;
    }

    public Comment(string body, Post post, User user)
    {
        Body = body;
        Post = post;
        User = user;
        PostId = post.Id;
        UserId = user.Id;
    }
}