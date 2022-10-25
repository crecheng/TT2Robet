using robot.RobotModel;

namespace robot;

public class GroupRobot: IRobot
{
    public long ID { get; set; }
    public long BaseId { get; set; }
    public Dictionary<string, RobotModelBase> RobotModel=new Dictionary<string, RobotModelBase>();
    public RobotConfig Config;
    public string HardName = ".";
    public string RobotName = "小助手";
    public bool IsAdmin(long qq) => Config.Admin.Contains(qq);
    
    public async Task<string> GetMsg(long sendPlayer, string text, object? obj = null)
    {
        if(Config.Admin.Contains(sendPlayer))
            await AdminFun(sendPlayer, text, obj);
        if(Config.Use)
            return await GetMsgInline(sendPlayer, text, obj);
        return "";
    }
    
    protected virtual async Task<string> GetMsgInline(long sendPlayer, string text, object? obj = null)
    {
        return "";
    }
    
    protected async Task AdminFun(long sendPlayer, string text, object? obj = null)
    {
        if (text.StartsWith(RobotName))
        {
            await RobotSwitch(sendPlayer, text, obj);
            await RobotModelManage(sendPlayer, text, obj);

        }
    }

    private async Task RobotSwitch(long sendPlayer, string text, object? obj = null)
    {
        var arg = text.Substring(RobotName.Length);
        if (arg == "闭嘴")
        {
            Config.Use = false;
            await SendMsg(RobotName + "不说话了",obj);
        }else if (arg == "说话")
        {
            Config.Use = true;
            await SendMsg(RobotName + "又活了",obj);
        }
    }

    private async Task RobotModelManage(long sendPlayer, string text, object? obj = null)
    {
        var arg = text.Substring(RobotName.Length);
        if (arg == "会什么")
        {
            string s = RobotName + "会这些哦：\n";
            foreach (var modelBase in RobotModel)
            {
                s += modelBase.Key + "\n";
            }

            await SendMsg(s, obj);
        }

        if (arg.StartsWith(".学会"))
        {
            
            string arg1 = arg.Substring(".学会".Length);
            if (RobotModel.ContainsKey(arg1))
            {
                await SendMsg(RobotName + "已经学过了",obj) ;
                return;
            }

            var model = RobotModelFactory.GetModel(arg1, this);
            if (model != null)
            {
                RobotModel.Add(arg1, model);
                await SendMsg(RobotName + "学会了", obj);
                Config.RobotModel.Add(arg1);
                RobotFactory.SaveGroupRobot(this);
                return;
            }
            else
            {
                await SendMsg(RobotName + "失败了！", obj);
                return;
            }
        }
        
        if (arg.StartsWith(".忘记"))
        {
            
            string arg1 = arg.Substring(".忘记".Length);
            if (!RobotModel.ContainsKey(arg1))
            {
                await SendMsg(RobotName + "还没学过呐",obj) ;
                return;
            }

            RobotModel.Remove(arg1);
            Config.RobotModel.Remove(arg1);
            await SendMsg(RobotName + "忘了",obj) ;
            RobotFactory.SaveGroupRobot(this);
            return;
            
        }
    }

    public virtual async Task<bool> SendMsg(string text, object? obj = null)
    {
        return true;
        await Task.CompletedTask;
    }
    
}