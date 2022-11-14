using Microsoft.VisualBasic;
using robot.RobotModel;
using Sora.Entities;
using Sora.Entities.Info;
using Sora.Enumeration.EventParamsType;
using Sora.EventArgs.SoraEvent;
using Sora.Interfaces;
using Sora.Util;

namespace robot;

public class GroupSoraRobot : GroupRobot
{
    public bool Work;
    public bool Open;
    public Group GroupData;
    
    public GroupSoraRobot(long id)
    {
        ID = id;
        Work = true;
        Open = true;
        Init();
    }
    

    public void Init()
    {
        var success= RobotFactory.InitGroupRobot(ID, this);
        if (!success)
        {
            Work = false;
            Console.WriteLine($"Init {ID}-{this.GetType().Name} fail !!!!!");
        }
    }
    
    protected override async Task<string> GetMsgInline(long sendPlayer, string text,int role, object? obj = null)
    {
        foreach (var (key, value) in RobotModel)
        {
            var res = await value.GetMsg(new GroupMsgData(sendPlayer, IsAdmin(sendPlayer),role>=2, text, obj));
            if (res.HaveData())
            {
                await SendMsgObj(res.GetSendMsg(), obj);
            }
        }
        await Task.CompletedTask;
        return "";
    }

    public async Task<GroupMemberInfo> GetMemberInfo(long qq)
    {
        var m = await Manage.Api.GetGroupMemberInfo(ID,qq);
        return m.memberInfo;
    }
    public override async Task<bool> SendMsg(string text, object? obj = null)
    {
        if (obj is GroupMessageEventArgs args)
        {
            await SendMsg(args, text);
            return true;
        }
        else
            Console.WriteLine($" !!!! Error {ID}-{BaseId}-[ obj==null {obj==null}]");

        return false;
    }
    public async Task<bool> SendMsgObj(MessageBody body, object? obj = null)
    {
        if (obj is GroupMessageEventArgs args)
        {
            await SendMsgObj(args, body);
            return true;
        }
        else
            Console.WriteLine($" !!!! Error {ID}-{BaseId}-[ obj==null {obj==null}]");

        return false;
    }
    

    public async ValueTask GetMsg(string _, GroupMessageEventArgs args)
    {
        
        await GetMsg(args.Sender.Id, args.Message.RawText,(int)args.SenderInfo.Role, args);
    }

    public async ValueTask SendMsg(GroupMessageEventArgs args, string text)
    {
        await args.Reply(CQCodeUtil.DeserializeMessage(text));
    }
    
    public async ValueTask SendMsgObj(GroupMessageEventArgs args, MessageBody text)
    {
        await args.Reply(text);
    }


}