// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace robot.RobotModel;

public class SkillCalRobotModel: RobotModelBase
{
    private static SkillCalDataManage Data;
    private Queue<SkillCalCore> _queue = new Queue<SkillCalCore>();
    public override string ModelName => "SkillCalRobotModel";
    private string start;
    private static string SkillImgPath="Data\\SkillCalRobotModel\\Img\\";
    private int Count = 0;
    public override async Task<SoraMessage> GetMsg(GroupMsgData data)
    {
        if (data.Text.StartsWith(start))
        {
            var arg = data.Text.Substring(start.Length);
            if (arg.StartsWith(' '))
                arg = arg.Substring(1);
            if (arg.Length > 0)
            {
                var args = arg.Split(' ');
                if (args.Length >= 2)
                {
                    if (int.TryParse(args[0], out int skill))
                    {
                        if(skill<=0)
                            return SoraMessage.Null;
                        List<string> build = new List<string>();
                        for (var i = 1; i < args.Length; i++)
                        {
                            var b = args[i];
                            if (!Data.AdditionConvert.ContainsKey(b))
                            {
                                return $"没有{b}流派呐，目前流派有这些诶\n{String.Join(",", Data.AdditionConvert.Keys)}";
                            }
                            build.Add(b);
                        }

                        var c = GetCore();
                        try
                        {
                            await c.CalRun(skill, build);
                            var f = GetModelDir() + $"{++Count % 10}.png";
                            c.DrawSkill(f,SkillImgPath);
                            InCore(c);
                            return Tool.Image(f);
                        }
                        catch (Exception e)
                        {
                            await OutException(e);
                        }
                        
                    }
                }
            }
        }

        return SoraMessage.Null;
    }

    private SkillCalCore GetCore()
    {
        if (_queue.Count <= 0)
            return new SkillCalCore();
        return _queue.Dequeue();
    }

    private void InCore(SkillCalCore core)
    {
        _queue.Enqueue(core);
    }

    public override void Init(long group, string robotName)
    {
        RobotName = robotName;
        start = RobotName + "加点";
        Group = group;
        CreateModelPath();
        if (Data == null)
        {
            Data = LoadFormData<SkillCalDataManage>("data.json");
            SkillCalDataManage.Init(Data);
        }
        
            
    }
}