using RepositoryContracts;
namespace CLI.UI.ManageComments;

public class CreateCommentView
{
    private ICommentRepository CommentRepository;

    public CreateCommentView(ICommentRepository CommentRepository)
    {
        this.CommentRepository = CommentRepository;
    }
}