using Newtonsoft.Json;

namespace robot;

public static class RobotFactory
{
    private static string Path = "Group";
    private static string pathReplyPath = "pathReply.json";
    private static string completeReplyPath = "completeReply.json";
    private static string functionPath = "fun.json";
    private static string adminFunctionPath = "adminFun.json";
    private static string adminQQPath = "adminQQ.json";
    private static Dictionary<string, Func<long, string, object,GroupRobot,string>> fun=RobotFunction.GetAllFun();
    private static Dictionary<string, Func<long, string, object,GroupRobot ,string>> adminFun=RobotAdminFunction.GetAllFun();
    public static bool InitGroupRobot(long id, GroupSoraRobot robot)
    {
        var path = $"{Path}/{id}";
        if (Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        try
        {
            var pr = $"{path}/{pathReplyPath}";
            robot.PathReply = GetJsonOrNew<Dictionary<string, string>>(File.ReadAllText(pr)) ;
            
            var cr=$"{path}/{completeReplyPath}";
            robot.CompleteReply = GetJsonOrNew<Dictionary<string, string>>(File.ReadAllText(cr)) ;
            
            var path1 = $"{path}/{adminQQPath}";
            robot.Admin = GetJsonOrNew<List<long>>(path1);

            var p2 = $"{path}/{functionPath}";
            List<string>? gFun = GetJsonOrNew<List<string>>(p2);
            if (gFun != null)
                foreach (var i in gFun)
                {
                    fun.TryGetValue(i, out Func<long, string, object, GroupRobot, string> f);
                    if (f != null)
                    {
                        robot.Fun.Add(i,f);
                    }

                }

            var p3=$"{path}/{adminFunctionPath}";
            List<string>? gAdminFun = GetJsonOrNew<List<string>>(p3);
            if (gAdminFun != null)
                foreach (var i in gAdminFun)
                {
                    fun.TryGetValue(i, out Func<long, string, object, GroupRobot, string> f);
                    if (f != null)
                    {
                        robot.AdminFun.Add(i,f);
                    }

                }
            
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public static T? GetJsonOrNew<T>(string path) where T: new()
    {
        return File.Exists(path) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(path)) : new T();
    }
}