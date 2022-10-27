using Sora;
using Sora.Entities;
using Sora.Entities.Base;
using Sora.EventArgs.SoraEvent;
using Sora.Interfaces;
using Sora.Net.Config;
using Sora.Util;
using YukariToolBox.LightLog;

namespace robot;

public static class Manage
{ 
    static Dictionary<long, GroupSoraRobot> _groupRobots = new Dictionary<long, GroupSoraRobot>();
    public static ISoraService sora;

    public static SoraApi Api => sora.GetApi(sora.ServiceId);

    private static long _adminGroup = 1028750616;
    private static long _testGroup = 645884953;
    public async static ValueTask GetGroupMsg(string _, GroupMessageEventArgs args)
    {
        var id = args.SourceGroup.Id;
        if (!_groupRobots.ContainsKey(id))
        {
            if(id==_adminGroup|| id==_testGroup)
                _groupRobots.Add(id,new AdminGroupRobot(id)); 
            else
                _groupRobots.Add(id,new GroupSoraRobot(id));
        }

        var robot = _groupRobots[id];
        if (id == _adminGroup)
            await robot.GetMsg(_, args);
        else if (robot.Work && robot.Open)
            await robot.GetMsg(_, args);
    }

    public async static Task SendGroupMsg(long group, MessageBody body)
    {
        await Api.SendGroupMessage(group, body);
    }

    public async static Task UploadFile(long group, string file,string upName)
    {
        await Api.UploadGroupFile(group, $"{AppDomain.CurrentDomain.BaseDirectory}{file}" , upName);
    }

    public async static Task SendGroupMsg(long group, SoraMessage body)
    {
        await SendGroupMsg(group, body.GetSendMsg());
    }

    static async void CommandExceptionHandle(Exception exception, BaseMessageEventArgs eventArgs, string log)
    {
        string msg = $"bug了！！！\r\n{log}\r\n{exception.Message}";
        await Api.SendGroupMessage(_adminGroup, msg);
    }

    public async static Task Run()
    {
        //设置log等级
        Log.LogConfiguration
            .EnableConsoleOutput()
            .SetLogLevel(LogLevel.Verbose);

        //实例化Sora服务
        ISoraService service = SoraServiceFactory.CreateService(new ClientConfig
        {
            EnableSocketMessage    = false,
            ThrowCommandException  = false,
            SendCommandErrMsg      = false,
            CommandExceptionHandle = CommandExceptionHandle,
        });
        service.Event.OnGroupMessage += GetGroupMsg;
        service.Event.OnSelfGroupMessage += (_, eventArgs) =>
        {
            Log.Warning("test", $"self group msg {eventArgs.Message.MessageId}[{eventArgs.IsSelfMessage}]");
            return ValueTask.CompletedTask;
        };
        
        sora = service;
        await service.StartService()
            .RunCatch(e => Log.Error("Sora Service", Log.ErrorLogBuilder(e)));
        await Task.Delay(1000);
        var list = await Api.GetGroupList();
        list.groupList.ForEach(id=>_groupRobots.Add(id.GroupId,new GroupSoraRobot(id.GroupId)));
    }
}