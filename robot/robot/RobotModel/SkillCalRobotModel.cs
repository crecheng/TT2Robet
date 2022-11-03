// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace robot.RobotModel;

public class SkillCalRobotModel: RobotModelBase
{
    public static SkillCalDataManage Data;
    private Queue<SkillCalCore> _queue = new Queue<SkillCalCore>();
    public override string ModelName => "SkillCalRobotModel";
    
    public override Task<SoraMessage> GetMsg(GroupMsgData data)
    {
        return base.GetMsg(data);
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
        if (Data == null)
        {
            Data = LoadFormData<SkillCalDataManage>("data.json");
        }
        
            
    }
}