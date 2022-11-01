namespace robot.RobotModel;

public class GroupMsgData
{
    public long Sender;
    public bool IsAdmin;
    public bool IsGroupAdmin;
    public string Text;
    public object obj;
    public GroupMsgData(long sender, bool isAdmin,bool isGroupAdmin, string text, object obj)
    {
        Sender = sender;
        IsGroupAdmin = isGroupAdmin;
        IsAdmin = isAdmin;
        Text = text;
        this.obj = obj;
    }
}