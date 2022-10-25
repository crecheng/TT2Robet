namespace robot.RobotModel;

public class RaidRobotModel : RobotModelBase
{
    public override string ModelName { get; } = "RaidRobotModel";

    public override async Task<SoraMessage> GetMsg(long sender,bool isAdmin, string text, object? obj = null)
    {
        return await base.GetMsg(sender,isAdmin, text, obj);
    }

    public override void Init(long group,string robotName)
    {
        base.Init(group,robotName);
        
    }
}