namespace CLI.UI.ManageComments;

public class ManageCommentView
{
    public string Body { get; set; }
    public int Id { get; set; }
    
    public ManageCommentView(string Body, int Id)
    {
        this.Body = Body;
        this.Id = Id;
    }
}