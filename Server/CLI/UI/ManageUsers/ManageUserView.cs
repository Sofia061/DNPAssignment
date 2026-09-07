namespace CLI.UI.ManageUsers;

public class ManageUserView
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    
    public ManageUserView(string Username, string Password, int Id)
    {
        this.Username = Username;
        this.Password = Password;
        this.Id = Id;
    }
}