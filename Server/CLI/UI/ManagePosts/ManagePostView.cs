using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostView
{
    public string Title { get; set; }
    public string Body { get; set; }
    public int Id { get; set; }
    
    public ManagePostView(string Title, string Body, int Id)
    {
        this.Title = Title;
        this.Body = Body;
        this.Id = Id;
    }
}