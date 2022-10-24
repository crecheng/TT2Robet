using robot.RobotModel;

namespace robot;

public class GroupRobot: IRobot
{
    public long ID { get; set; }
    public long BaseId { get; set; }
    public List<long> Admin=new List<long>();
    public Dictionary<string, RobotModelBase> RobotModel=new Dictionary<string, RobotModelBase>();
    public RobotConfig Config;
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