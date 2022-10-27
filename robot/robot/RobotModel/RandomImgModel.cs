namespace robot.RobotModel;

public class RandomImgModel : RobotModelBase
{
    public override string ModelName => "RandomImgModel";
    public string Key = "嘤嘤嘤";
    public override async Task<SoraMessage> GetMsg(GroupMsgData data)
    {
        if(data.text==Key)
            return Tool.RandomImage("Img");
        return SoraMessage.Null;
        await Task.CompletedTask;
    }
}