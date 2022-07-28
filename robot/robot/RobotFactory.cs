using Newtonsoft.Json;

namespace robot;

public static class RobotFactory
{
    private static string Path = "Group";
    private static string pathReplyPath = "pathReply.json";
    private static string completeReplyPath = "completeReply.json";
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
            robot.PathReply = File.Exists(pr) ? 
                JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(pr)) : 
                new Dictionary<string, string>();
            
            var cr=$"{path}/{completeReplyPath}";
            robot.CompleteReply = File.Exists(cr) ? 
                JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(cr)) : 
                new Dictionary<string, string>();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        
    }
}