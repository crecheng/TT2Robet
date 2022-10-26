using System.Text.RegularExpressions;
using Newtonsoft.Json;
using testrobot;

namespace robot.RobotModel;

public class RaidRobotModel : RobotModelBase
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

    private List<string> _cmd = new List<string>()
    {
        "突袭命令",
        "突袭血量",
        "突袭刷新",
        "查看突袭配置",
        "突袭伤害",
        "游戏绑定{代码或名字}",
        "我的数据",
        "卡片数据"
    };
    public override async Task<SoraMessage> GetMsg(long sender,bool isAdmin, string text, object? obj = null)
    {
        if (text.StartsWith(RobotName))
        {
            var arg = text.Substring(RobotName.Length);
            if (arg == "突袭命令")
            {
                return string.Join("\n", _cmd);
            }
            else if (arg == "突袭血量")
            {
                return await LookHp();
            }else if (arg == "突袭刷新")
            {
                return await RefreshData(sender, isAdmin, text, obj);
            }
            else if (arg == "查看突袭配置")
            {
                return _config.GetEasyData();
            }else if (arg == "突袭伤害")
            {
                return await LookDmg();
            }
            else if (arg.StartsWith("游戏绑定"))
            {
                var arg1 = arg.Substring("游戏绑定".Length);
                return await QQLinkGame(arg1, sender);
            }else if (arg == "我的数据")
            {
                return await GetMyInfo(sender);
            }
            else if (arg == "卡片数据")
            {
                return await GetMyCard(sender);
            }
        }
        return await base.GetMsg(sender,isAdmin, text, obj);
    }

    private async Task<SoraMessage> GetMyInfo(long sender)
    {
        if (_data.QQLink.ContainsKey(sender))
        {
            return _data.Player[_data.QQLink[sender]].EasyInfo();
        }
        return "你还没绑定";
    }

    private async Task<SoraMessage> GetMyCard(long sender)
    {
        if (_data.QQLink.ContainsKey(sender))
        {
            var p = _data.Player[_data.QQLink[sender]];
            if (p.Card.Count == 0)
                return "你还没开片数据";
            var f = GetModelDir() + p.Code + "-card.png";
            ClubTool.DrawPlayerCard(p.Card,f,GetModelDir()+"Card-32\\");
            return Tool.Image(f);
        }
        return "你还没绑定";
    }


    private async Task<SoraMessage> QQLinkGame(string code, long sender)
    {
        string tip = "请正确输入，\n列如：\n小助手游戏绑定璀璨\n小助手游戏绑定abc123";
        if (string.IsNullOrEmpty(code))
        {
            return tip;
        }

        if (_data.PlayerId.ContainsKey(code))
        {
            var c = _data.PlayerId[code];
            if (!_data.QQLink.ContainsKey(sender))
            {
                _data.QQLink.Add(sender,c);
            }
            var p = _data.Player[c];
            _data.QQLink[sender] = c;
            return $"绑定成功-{p.Code}-{p.Name}";
        }
        else if(_data.Player.ContainsKey(code))
        {
            if (!_data.QQLink.ContainsKey(sender))
            {
                _data.QQLink.Add(sender,code);
            }

            var p = _data.Player[code];
            _data.QQLink[sender] = code;
            return $"绑定成功-{p.Code}-{p.Name}";
        }
        else
        {
            return "没有找到\n" + tip;
        }
    }

    private async Task<SoraMessage> LookHp()
    {
        if (_club == null)
        {
            return "没有数据！";
        }
        else
        {
            var f = GetModelDir() + "kkHp.png";
            _club.GetCurrentTitanData().DrawTitan(f);
            return Tool.Image(f);
        }

        await Task.CompletedTask;
    }

    private async Task<SoraMessage> LookDmg()
    {
        if (_club == null)
        {
            return "没有数据！";
        }
        else
        {
            var f = GetModelDir() + "kkDmg.png";
            ClubTool.DrawPlayerRaidDmg(_club, f);
            return Tool.Image(f);
        }

        await Task.CompletedTask;
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


    
    private async Task<SoraMessage> RefreshData(long sender, bool isAdmin, string text, object? obj = null)
    {
        if (_config.CanUse())
        {
            await SendGroupMsg("刷新ing");
            var t = await RefreshData();
            if (t != null)
            {
                _club = t;
                return "刷新成功！";
            }
            else
            {return "失败！";
                
            }
        }
        else
        {
            return "还没配置文件";
        }

        await Task.CompletedTask;
    }
    private void StartRefreshData()
    {
        _refreshDataTask=Task.Run(() =>
        {
            while (true)
            {
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
                Console.WriteLine($"{Group}-部落刷新数据");
                Thread.Sleep(_refreshCd);
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