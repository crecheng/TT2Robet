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

    public virtual async Task<SoraMessage> GetMsg(GroupMsgData data)
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

    public void CreateModelPath()
    {
        var file = $"{Config.DataPath}\\{Group}\\{ModelName}\\";
        var p= Path.GetDirectoryName(file);
        if (!Directory.Exists(p))
            Directory.CreateDirectory(p);
    }
    

    public string Load(string path)
    {
        var file = $"{Config.DataPath}\\{Group}\\{ModelName}\\{path}";
        if (File.Exists(file))
            return File.ReadAllText(file);
        return String.Empty;
    }

    public string GetModelDir()
    {
        return $"{Config.DataPath}\\{Group}\\{ModelName}\\";
    }

    public async Task SendGroupMsg(SoraMessage message)
    {
       await Manage.SendGroupMsg(Group, message);
    }

    public async Task UploadGroupFile(string file,string name)
    {
        await Manage.UploadFile(Group, GetModelDir() + file, name);
    }

    public async Task<string> DownloadGroupFile(string file)
    {
        return await Manage.DownLoadGroupFile(Group, file);
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

    public T LoadFormData<T>(string path)
    {
        var file = $"Data\\{ModelName}\\{path}";
        if (!File.Exists(file))
        {
            return default;
        }
        return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
    }
    
    public void SaveToData(string path,string json)
    {
        var p = $"Data\\{ModelName}\\";
        var file = $"Data\\{ModelName}\\{path}";
        if (!Directory.Exists(p))
            Directory.CreateDirectory(p);
        File.WriteAllText(file,json);
    }
    
    public T LoadCanBeNull<T>(string path)
    {
        var file = $"{Config.DataPath}\\{Group}\\{ModelName}\\{path}";
        if (!File.Exists(file))
        {
            return default;
        }
        return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));

    }

    public async Task OutException(string s)
    {
        await Manage.OutException(s);
    }
    
    public async Task OutException(Exception s)
    {
        await Manage.OutException($"{Group}-{ModelName}\n"+ s.ToString());
    }

}