using Sora.Entities.Segment;

namespace robot;

public static class RobotFunction
{
    public static Dictionary<string, Func<long, string, object,GroupRobot,string>> GetAllFun()
    {
        return new Dictionary<string, Func<long, string, object,GroupRobot, string>>()
        {
            { "时间",GetTime}
        };
    }

    public static string GetTime(long qq, string text, object obj,GroupRobot robot)
    {
        if (text == "现在几点了" || text == "几点了")
        {
        }

        return "";
    }
}

public static class RobotAdminFunction
{
    public static Dictionary<string, Func<long, string, object,GroupRobot,string>> GetAllFun()
    {
        return new Dictionary<string, Func<long, string, object,GroupRobot, string>>()
        {
            { "别说话",RobotOff}
        };
    }

    public static string RobotOff(long qq, string text, object obj,GroupRobot robot)
    {
        return "";
    }
}