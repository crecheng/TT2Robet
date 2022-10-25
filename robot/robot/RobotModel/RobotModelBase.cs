using Newtonsoft.Json;

namespace robot.RobotModel;

public class RobotModelBase
{
    public long Group;
    public virtual bool IsAdmin{ get; } = false;
    public virtual string ModelName { get; } = "base";

    public virtual bool NeedRobotName { get; } = false;

    public string RobotName;

    public bool Work;

    public virtual async Task<SoraMessage> GetMsg(long sender,bool isAdmin, string text, object? obj = null)
    {
        return SoraMessage.Null;
        await Task.CompletedTask;
    }

    public virtual void Init(long group,string robotName)
    {
        Group = group;
        RobotName = robotName;
    }

    public void Save(string path, string text)
    {
        var file = $"{Config.DataPath}\\{Group}\\{ModelName}\\{path}";
        var p= Path.GetDirectoryName(file);
        if (!Directory.Exists(p))
            Directory.CreateDirectory(p);
        File.WriteAllText(file,text);
    }

    public string Load(string path)
    {
        var file = $"{Config.DataPath}\\{Group}\\{ModelName}\\{path}";
        if (File.Exists(file))
            return File.ReadAllText(file);
        return String.Empty;
    }

    public T Load<T>(string path)
    {
        var file = $"{Config.DataPath}\\{Group}\\{ModelName}\\{path}";
        if (!File.Exists(file))
        {
            return Activator.CreateInstance<T>();
        }
        return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));

    }


}