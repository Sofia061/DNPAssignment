using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
}