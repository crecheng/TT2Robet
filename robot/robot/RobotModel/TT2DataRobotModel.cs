using Newtonsoft.Json;
using testrobot;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace robot.RobotModel;

public class TT2DataRobotModel: RobotModelBase
{
    public override string ModelName { get; } = "TT2DataRobotModel";
    public static Dictionary<long, List<string>> QQLink;
    public static string DataFile = "Data\\TT2DataRobotModle\\QQLink.json";
    public static string DataPath = "Data\\TT2DataRobotModle\\";
    private Dictionary<string, Func<GroupMsgData, Task<SoraMessage>>> _argFun;
    private Dictionary<string, Func<GroupMsgData,string, Task<SoraMessage>>> _argLenFun;
    private int _drawCount;
    public string RobotName;
    
    public override async Task<SoraMessage> GetMsg(GroupMsgData data)
    {
        if (data.Text.StartsWith(RobotName))
        {
            var arg = data.Text.Substring(RobotName.Length);
            if (_argFun.ContainsKey(arg))
                return await _argFun[arg].Invoke(data);
            
            foreach (var func in _argLenFun)
            {
                if (arg.StartsWith(func.Key))
                {
                    var arg1 = arg.Substring(func.Key.Length);
                    return await func.Value.Invoke(data, arg1);
                }
            }
        }
        return await base.GetMsg(data);
    }
    public override void Init(long group, string robotName)
    {
        if (!Directory.Exists(DataPath))
            Directory.CreateDirectory(DataPath);
        RobotName = robotName;
        if (QQLink == null)
        {
            if (File.Exists(DataFile))
            {
                QQLink = JsonConvert.DeserializeObject<Dictionary<long, List<string>>>(File.ReadAllText(DataFile));
            }
            else
            {
                QQLink = new Dictionary<long, List<string>>();
            }
        }

        if (_argFun == null)
        {
            _argFun = new Dictionary<string, Func<GroupMsgData, Task<SoraMessage>>>()
            {
                { "卡片数据", GetMyCard },
            };
            _argLenFun = new Dictionary<string, Func<GroupMsgData, string, Task<SoraMessage>>>()
            {
                { "绑定", QQLinkGame },
                { "解绑", QQUnLinkGame },
                { "切换", SwitchLinkGame }
            };
        }
    }

    private async Task<SoraMessage> GetMyCard(GroupMsgData data)
    {
        if (!QQLink.ContainsKey(data.Sender))
            return "你还没绑定呐";
        var code = QQLink[data.Sender][0];
        var p = SearchPlayer(code);

        if(p==null)
            return $"{RobotName}没找到你的数据呐，你需要输入对的代码才能绑定呐";

        Dictionary<string, string> dic = null;
        if (p.SourceData != null)
            dic = p.SourceData.GetInfo();
        var f = DataPath + $"GetMyCard{(++_drawCount) / 20}.png";
        ClubTool.DrawPlayerCard(p.Name, p.Card, dic, f, RaidRobotModel.Card32Path);
        return Tool.Image(f);
    }

    private async Task<SoraMessage> QQLinkGame(GroupMsgData data, string code)
    {
        if (QQLink.ContainsKey(data.Sender) && QQLink[data.Sender].Contains(code))
            return $"你已经绑定了{code}，不能重复绑定哦";

        var p = SearchPlayer(code);

        if(p==null)
            return $"{RobotName}没找到你的数据呐，你需要输入对的代码才能绑定呐";

        if(!QQLink.ContainsKey(data.Sender))
            QQLink.Add(data.Sender,new List<string>());
        QQLink[data.Sender].Insert(0,p.Code);
        SaveLink();
        return $"{RobotName}知道了，你是{p.Name}-{p.Code}";
    }
    
    private async Task<SoraMessage> QQUnLinkGame(GroupMsgData data, string code)
    {
        if (!QQLink.ContainsKey(data.Sender))
            return "你都还没绑定呐";
        if (QQLink[data.Sender].Remove(code))
        {
            if (QQLink[data.Sender].Count == 0)
                QQLink.Remove(data.Sender);
            SaveLink();
            return "解绑成功了";
        }
        else
            return "好像失败了";
    }
    private async Task<SoraMessage> SwitchLinkGame(GroupMsgData data, string index)
    {
        if (!QQLink.ContainsKey(data.Sender))
            return "你都还没绑定呐";
        if (string.IsNullOrEmpty(index))
            return "你错了，应该是切换2";
        if (int.TryParse(index, out int i))
        {
            var list = QQLink[data.Sender];
            var c = i - 1;
            if (c < 0 || c >= list.Count)
            {
                string s = string.Empty;
                for (var j = 0; j < list.Count; j++)
                {
                    s += $"{j + 0}：{list[j]}\n";
                }

                return "不对!你当前的绑定\n" + s;
            }

            (list[c], list[0]) = (list[0], list[c]);
            SaveLink();
            return $"成功了，{list[c]}";
        }
        else
        {
            return "你错了，应该是切换2";
        }
    }

    private RaidRobotModel.PlayerData SearchPlayer(string code)
    {
        foreach (var model in RaidRobotModel.AllInstance)
        {
            var club = model.GetAllPlayer();
            if (club.ContainsKey(code))
            {
                return club[code];
                
            }
        }
        return null;
    }
    

    private void SaveLink()
    {
        var path = "Data\\TT2DataRobotModle\\";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        File.WriteAllText(DataFile,JsonConvert.SerializeObject(QQLink));
    }
}