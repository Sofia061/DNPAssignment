using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private IPostRepository postRepository;

    public CreatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }
}