using Microsoft.VisualBasic;
using Sora.Entities;
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
            Console.WriteLine($"Init {this.GetType().Name} fail !!!!!");
        }
    }
    
    public override async Task<string> GetMsg(long sendPlayer, string text, object? obj = null)
    {
        foreach (var i in CompleteReply)
        {
            if (text == i.Key)
            {
                await SendMsg(i.Value);
                return "";
            }
        }
        foreach (var i in PathReply)
        {
            if (text.IndexOf(i.Key, StringComparison.Ordinal)>=0)
            {
                await SendMsg(i.Value);
                return "";
            }
        }

        return "";
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
    public async Task<bool> SendMsgObj(MessageBody text, object? obj = null)
    {
        if (obj is GroupMessageEventArgs args)
        {
            await SendMsgObj(args, text);
            return true;
        }
        else
            Console.WriteLine($" !!!! Error {ID}-{BaseId}-[ obj==null {obj==null}]");

        return false;
    }
    

    public async ValueTask GetMsg(string _, GroupMessageEventArgs args)
    {
        await GetMsg(args.Sender.Id, args.Message.RawText, args);
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