using Entities;

namespace CLI.UI.ManageComments;

public class ListCommentView
{
    protected List<Comment> Comments = [];

    public ListCommentView()
    { }

    public int AllComments()
    {
        return Comments.Count;
    }

    public void AddToList(Comment comment)
    {
        Comments.Add(comment);
    }
    
    public void DisplayComments(IEnumerable<Comment> comments)
    {
        foreach (var comment in comments)
        {
            Console.WriteLine($"[ID: {comment.Id}] Post: {comment.PostId} | User: {comment.UserId} -> {comment.Body}");
        }
    }
}