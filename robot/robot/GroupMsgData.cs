namespace robot.RobotModel;

public class GroupMsgData
{
    public long Sender;
    public bool IsAdmin;
    public string text;
    public object obj;
    public GroupMsgData(long sender, bool isAdmin, string text, object obj)
    {
        Sender = sender;
        IsAdmin = isAdmin;
        this.text = text;
        this.obj = obj;
    }
}