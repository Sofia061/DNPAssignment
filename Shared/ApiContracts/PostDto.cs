namespace ApiContracts;

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
    public UserDto? Author { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}