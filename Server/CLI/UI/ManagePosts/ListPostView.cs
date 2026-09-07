using Entities;

namespace CLI.UI.ManagePosts;

public class ListPostView
{
    protected List<Post> Posts = [];

    public ListPostView()
    { }

    public int AllPosts()
    {
        return Posts.Count;
    }

    public void AddToList(Post post)
    {
        Posts.Add(post);
    }
}