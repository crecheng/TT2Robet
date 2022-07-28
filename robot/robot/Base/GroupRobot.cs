namespace robot;

public class GroupRobot: IRobot
{
    public long ID { get; set; }
    public long BaseId { get; set; }
    public Dictionary<string, string>? PathReply;
    public Dictionary<string, string>? CompleteReply;
    public List<long>? Admin;
    public Dictionary<string, Func<long, string, object, GroupRobot, string>> Fun;
    public Dictionary<string, Func<long, string, object, GroupRobot, string>> AdminFun;

    public string HardName = ".";
    public string RobotName = "小助手";
    
    public virtual async Task<string> GetMsg(long sendPlayer, string text, object? obj = null)
    {
        await SendMsg(text);
        return "";
    }

    public virtual async Task<bool> SendMsg(string text, object? obj = null)
    {
        return true;
    }
}