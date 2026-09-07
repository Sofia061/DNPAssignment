using Entities;
using RepositoryContracts;
namespace CLI.UI.ManagePosts;

public class SinglePostView : ListPostView
{
    public SinglePostView()
    { }

    public int FindSinglePost(Post post)
    {
        return Posts.IndexOf(post);
    }
    
    public void Display(Post post)
    {
        Console.WriteLine($"\nID: {post.Id}\nTitle: {post.Title}\nBody: {post.Body}\nAuthor ID: {post.UserId}");
    }
}