using Entities;
namespace CLI.UI.ManageUsers;

public class ListUserView
{
    protected List<User> Users = [];

    public ListUserView()
    { }

    public int AllUsers()
    {
        return Users.Count;
    }

    public void AddToList(User user)
    {
        Users.Add(user);
    }
}