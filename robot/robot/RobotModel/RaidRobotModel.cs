using System.Text.RegularExpressions;
using Newtonsoft.Json;
using testrobot;

namespace robot.RobotModel;

public partial class RaidRobotModel : RobotModelBase
{
    public override string ModelName { get; } = "RaidRobotModel";

    private ClubData _club;
    private TT2Post _post;
    private TT2PostConfig _config;
    private Task _refreshDataTask;
    private static string ConfigFile = "config.json";
    private static string DataFile = "data.json";
    private TimeSpan _refreshCd = new TimeSpan(0, 0, 5, 0);
    private TimeSpan _refreshFromCd = new TimeSpan(0, 0, 10, 0);
    private RaidData _data;
    private string _startString = "#";

    private static int _lenCd => _len++;
    private static int _len;


    private Dictionary<string, Func<GroupMsgData, Task<SoraMessage>>> _argFun;

    private Dictionary<string, Func<GroupMsgData,string, Task<SoraMessage>>> _argLenFun;
    public override async Task<SoraMessage> GetMsg(GroupMsgData data)
    {
        if (data.text.StartsWith(_startString))
        {
            var arg = data.text.Substring(_startString.Length);
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
    
    private string GetNearAtkInfo(DateTime start, DateTime end)
    {
        int c = 0;
        Dictionary<string, List<AttackShareInfo>> all = new Dictionary<string, List<AttackShareInfo>>();
        foreach (var info in _data.AttackInfos)
        {
            if (info.Time > start && info.Time< end)
            {
                c++;
                if(!all.ContainsKey(info.Player))
                    all.Add(info.Player,new List<AttackShareInfo>());
                all[info.Player].Add(info);
            }
        }

        if (c == 0)
            return string.Empty;
        Dictionary<string, List<AttackShareInfo>> res = new Dictionary<string, List<AttackShareInfo>>();
        foreach (var info in all)
        {
            res.Add(_data.Player[info.Key].Name,info.Value);
        }
        var f = GetModelDir() + "AtkInfo.png";
        ClubTool.DrawAtkInfo(res,f,GetModelDir()+"Card-32\\");
        return f;
    }

    private void RefreshRaidDataSave()
    {
        _data.PlayerId.Clear();
        foreach (var playerData in _club.clan_raid.leaderboard)
        {
            var id = playerData.player_code;
            var name = Regex.Unescape(playerData.name);
            _data.PlayerId.Add(name,id);
        }
    }
    
    private void RefreshRaidCardDataSave(List<AttackShareInfo> cards)
    {
        int max = _data.LastFromId;
        
        foreach (var atk in cards)
        {
            if (atk.Id > max)
            {
                _data.AttackInfos.Add(atk);
                max = atk.Id;
                if(!_data.Player.ContainsKey(atk.Player))
                    _data.Player.Add(atk.Player,new PlayerData()
                    {
                        Code = atk.Player,
                        Name = Regex.Unescape(atk.PlayerName)
                    });
                var p = _data.Player[atk.Player];
                p.RaidLevel = atk.Data.RaidLevel;
                foreach (var (key, value) in atk.Data.Card)
                {
                    if (!p.Card.ContainsKey(key))
                        p.Card.Add(key, value);
                    else
                        p.Card[key] = value;
                }
            }
        }

        _data.LastFromId = max;
        SaveRaidData();
    }
    


    private async Task<SoraMessage> GetRaidConfig(GroupMsgData data)
    {
        return _config.GetEasyData();
    }
    private void StartRefreshData()
    {
        _refreshDataTask=Task.Run(async () =>
        {
            await Task.Delay(_lenCd * 10000);
            while (true)
            {
                Console.WriteLine($"{Group}-部落刷新数据");
                if (_config.CanUse())
                {
                    ClubData tmp = _post.RaidCurrent(GetModelDir()+"RaidCurrent.json");
                    var msg = _post.GetForum(GetModelDir() + "GetForum.json");
                    var all= AttackShareInfo.GetAllAttackShareInfo(msg);
                    tmp.Init();
                    if (tmp != null)
                    {
                        _club = tmp;
                    }
                    RefreshRaidDataSave();
                    RefreshRaidCardDataSave(all);
                }
                
                await Task.Delay(_refreshCd);
            }
        });
    }

    private async Task<ClubData> RefreshData()
    {
        ClubData tmp = null;
        await Task.Run(() =>
        {
            if (_config.CanUse())
            {
                tmp = _post.RaidCurrent(GetModelDir()+"RaidCurrent.json");
                tmp.Init();
            }
        });
        return tmp;
    }

    public override void Init(long group,string robotName)
    {
        base.Init(group,robotName);
        _config=Load<TT2PostConfig>(ConfigFile);
        _post = new TT2Post();
        _post.Tt2Post = _config;
        _data = Load<RaidData>(DataFile);
        StartRefreshData();
        RegisterFun();
    }

    public void RegisterFun()
    {
        _argFun = new Dictionary<string, Func<GroupMsgData, Task<SoraMessage>>>()
        {
            { "突袭命令", GetCmd },
            { "突袭血量", LookHp },
            { "突袭刷新", RefreshData },
            { "查看突袭config", GetRaidConfig },
            { "突袭伤害", LookDmg },
            { "我的数据", GetMyInfo },
            { "卡片数据", GetMyCard },
            { "全员卡", GetAllCard },
            { "导出全员卡", UploadAllCard },
            { "查询攻击本轮", GetNearAtkInfoThisRound },
            { "本轮时间", GetThisRoundTime },
        };

        _argLenFun = new Dictionary<string, Func<GroupMsgData, string, Task<SoraMessage>>>()
        {
            { "游戏绑定", QQLinkGame },
            { "查询攻击时间", GetNearAtkInfo },
        };

    }
    
    private void SaveRaidData()
    {
        Save(DataFile,JsonConvert.SerializeObject(_data));
    }

    class RaidData
    {
        public Dictionary<string, string> PlayerId = new Dictionary<string, string>();
        public Dictionary<long, string> QQLink = new Dictionary<long, string>();
        public Dictionary<string, PlayerData> Player = new Dictionary<string, PlayerData>();
        public List<AttackShareInfo> AttackInfos = new List<AttackShareInfo>();

        public int LastFromId;
    }

    class PlayerData
    {
        public Dictionary<string, int> Card = new Dictionary<string, int>();

        public string Code;
        public string Name;
        public int RaidLevel;

        public string EasyInfo()
        {
            return $"{Name}-{Code}\n" +
                   $"突等：{RaidLevel}";
        }
    }
}