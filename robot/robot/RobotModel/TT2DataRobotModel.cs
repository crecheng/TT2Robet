using Newtonsoft.Json;
using testrobot;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace robot.RobotModel;

public class TT2DataRobotModel: RobotModelBase
{
    public override string ModelName { get; } = "TT2DataRobotModel";
    public static Dictionary<long, List<string>> QQLink;
    public static string DataFile = "Data\\TT2DataRobotModel\\QQLink.json";
    public static string DataPath = "Data\\TT2DataRobotModel\\";
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
        Group = group;
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
                { "卡名字", GetCardName },
            };
            _argLenFun = new Dictionary<string, Func<GroupMsgData, string, Task<SoraMessage>>>()
            {
                { "绑定", QQLinkGame },
                { "解绑", QQUnLinkGame },
                { "切换", SwitchLinkGame },
                { "查看卡", ShowCardInfo },
                { "突袭模拟", RaidCardCal },
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

    private async Task<SoraMessage> GetCardName(GroupMsgData data)
    {
        return Tool.Image(DataPath + "ShowCardName.png");
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

    private async Task<SoraMessage> ShowCardInfo(GroupMsgData data, string card)
    {
        if (string.IsNullOrEmpty(card))
            return "";
        var id = ClubTool.NameToIDCard(card);
        if (string.IsNullOrEmpty(id))
        {
            SoraMessage msg = "小助手没有找到对应的卡\n小助手目前只知道这些卡哦\n";
            msg.Add(Tool.Image(DataPath+"ShowCardName.png"));
            return msg;
        }
        var cardData = RaidRobotModel.DataManage.GetCardDataDataFirst(id);
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("ID",cardData.ID);
        dic.Add("名字",cardData.Name);
        dic.Add("描述",cardData.Note);
        dic.Add("类别",cardData.Category);
        dic.Add("Tier",cardData.Tier.ToString());
        dic.Add("推荐部位",cardData.BestAgainst);
        dic.Add("最大叠层",cardData.MaxStacks.ToString());
        dic.Add("时间",cardData.Duration.ToString("F"));
        dic.Add("几率",cardData.Chance.ToString("F"));
        if (cardData.BonusTypeC != "None")
        {
            dic.Add("加成C",$"{ClubTool.CardDmageType.TryGet(cardData.BonusTypeC) } : {cardData.BonusCValue.ToString("F3")}" );
        }
        if (cardData.BonusTypeD != "None")
        {
            dic.Add("加成D",$"{ClubTool.CardDmageType.TryGet(cardData.BonusTypeD)} : {cardData.BonusDValue.ToString("F3")}" );
        }
        if (cardData.BonusTypeE != "None")
        {
            dic.Add("加成E",$"{ClubTool.CardDmageType.TryGet(cardData.BonusTypeE)} : {cardData.BonusEValue.ToString("F3")}" );
        }
        if (cardData.BonusTypeF != "None")
        {
            dic.Add("加成F",$"{ClubTool.CardDmageType.TryGet(cardData.BonusTypeF)} : {cardData.BonusFValue.ToString("F3")}" );
        }

        string st=String.Empty;
        string sa=String.Empty;
        string sb=String.Empty;
        int rowCount = 5;
        for (int i = 1; i <= 60; i++)
        {
            st += $"{i}\t\t";
            sa += $"{cardData.BonusAValue[i - 1]:F3}\t\t";
            sb += $"{cardData.BonusBValue[i - 1]:F3}\t\t";
            if (i % rowCount == 0)
            {
                dic.Add($"等级{i-rowCount+1}-{i}",st);
                if(cardData.BonusTypeA!="None")
                    dic.Add($"{ClubTool.CardDmageType.TryGet(cardData.BonusTypeA)}:{i-rowCount+1}-{i}",sa);
                if(cardData.BonusTypeB!="None")
                    dic.Add($"{ClubTool.CardDmageType.TryGet(cardData.BonusTypeB)}:{i-rowCount+1}-{i}",sb);
                st=String.Empty;
                sa=String.Empty;
                sb=String.Empty;
            }
        }
        var f=DataPath + $"ShowCardInfo{(++_drawCount) / 20}.png";
        ClubTool.DrawInfo(dic, f, 800);
        return Tool.Image(f);
    }
    
    private Dictionary<string, int> partCName = new Dictionary<string, int>()
    {
        {"头", 0},
        {"胸", 1},
        {"左肩", 2},
        {"右肩", 3},
        {"左腿", 4},
        {"右腿", 5},
        {"左手", 6},
        {"右手", 7},
    };
    private async Task<SoraMessage> RaidCardCal(GroupMsgData data, string other)
    {
        if (string.IsNullOrEmpty(other))
            return "格式不对哦，格式应该是这样的\n突袭模拟 200 胸白条 月光10 风刃10 风刃10\n突袭模拟 200 胸蓝条 月光10 灵魂10 风刃10";

        if (other.StartsWith(' '))
            other = other.Substring(1);
        var args = other.Split(' ');
        if(args.Length<=1)
            return "格式不对哦，格式应该是这样的\n突袭模拟 200 胸白条 月光10 风刃10 风刃10\n突袭模拟 200 胸蓝条 月光10 灵魂10 风刃10";
        int p = -1;
        int pa = -1;
        if (partCName.ContainsKey(args[0]))
            p = partCName[args[0]];
        
        if (p == -1)
        {
            foreach (var (name,i) in partCName)
            {
                if (args[1].StartsWith(name))
                    p = i;
            }

            if (args[1].EndsWith("蓝条"))
                pa = 0;
            else if (args[1].EndsWith("白条"))
                pa = 1;
            if (pa == -1 || p == -1)
                return "没有这个部位哦，如头蓝条，头白条";
        }

        if (!int.TryParse(args[0], out int raidLevel) || raidLevel<1)
            return "你还告诉小助手突袭等级呐";

        List<CalPart> parts = new List<CalPart>();
        Dictionary<string, int> card = new Dictionary<string, int>();
        for (var i = 2; i < Math.Min(args.Length,5) ; i++)
        {
            if(args[i].Length<2)
                return "没找到对于简称，查看简称可以命令：卡名字";
            var c = args[i].Substring(0,2);
            var id = ClubTool.NameToIDCard(c);
            if (string.IsNullOrEmpty(id))
                return "没找到对于简称，查看简称可以命令：卡名字";

            if (!int.TryParse(args[i].Substring(2),out int level)|| level>60|| level<=0)
                return "你还告诉小助手卡等级呐";
            if(card.ContainsKey(id))
                return SoraMessage.Null;
            card.Add(id,level);
        }
        RaidCal cal = new RaidCal();


        CalPart target = new CalPart(p, pa);

        parts.Add(target);
        cal.TargetPart = target;
        RaidCal.RaidAdd add = new RaidCal.RaidAdd();
        
        double all = 0;
        int count = 100;
        List<RaidDmgData> list = new List<RaidDmgData>();
        for (int i = 0; i < count; i++)
        {
            var d = cal.Cal(card, raidLevel, RaidRobotModel.DataManage, parts, add);
            list.Add(d);
        }
        
        var f=DataPath + $"RaidCardCal{(++_drawCount) / 20}.png";
        RaidDmgDataTool.DrawDmgDataList(list,f);
        return Tool.Image(f);
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