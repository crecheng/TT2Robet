namespace robot.RobotModel;

public class RandomImgModel : RobotModelBase
{
    public override string ModelName => "RandomImgModel";
    public string Key = "嘤嘤嘤";
    public override async Task<SoraMessage> GetMsg(long sender, bool isAdmin, string text, object? obj = null)
    {
        if(text==Key)
            return Tool.RandomImage("Img");
        return SoraMessage.Null;
        await Task.CompletedTask;
    }
}