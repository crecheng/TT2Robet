using System.Text;
using System.Text.RegularExpressions;
using ConfigData;
using Newtonsoft.Json;
using robot.SocketTool;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;
using testrobot;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace robot.RobotModel;

public partial class RaidRobotModel : RobotModelBase
{
    public override string ModelName { get; } = "RaidRobotModel";
    public static string Card32Path = "Data\\RaidRobotModel\\Card-32\\";
    public static string Card64Path = "Data\\RaidRobotModel\\Card-64\\";
    public static List<RaidRobotModel> AllInstance = new List<RaidRobotModel>();
    public static ConfigDataManage DataManage;
    private ClubData _club;
    private TT2Post _post;
    private TT2PostConfig _config;
    
    private static Task _refreshDataTask;
    private TT2PostAPI PostApi;
    
    private static string ConfigFile = "config.json";
    private static string DataFile = "data.json";
    private RaidData _data;
    private string _startString = "#";
    private int showMyCardCount;
    

    private static int _lenCd => _len++;
    private static int _len;


    private Dictionary<string, Func<GroupMsgData, Task<SoraMessage>>> _argFun;

    private Dictionary<string, Func<GroupMsgData,string, Task<SoraMessage>>> _argLenFun;
    public override async Task<SoraMessage> GetMsg(GroupMsgData data)
    {
        var oldText = data.Text;
        if (data.obj is GroupMessageEventArgs soraArgs)
        {
            data.Text = soraArgs.Message.GetText();
        }

        if (data.Text.StartsWith(_startString))
        {
            try
            {
                var arg = data.Text.Substring(_startString.Length);
                SetIsAutoRefresh(data, arg);
                await ShowUesClub(data, arg);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                await OutException(e);
            }
        }

        data.Text = oldText;
        return await base.GetMsg(data);
    }

    private void SetIsAutoRefresh(GroupMsgData data, string arg)
    {
        if (data.IsAdmin || data.IsGroupAdmin)
        {
            if (arg == "*关")
            {
                _data.isRefresh = false;
                SaveRaidData();
            }else if (arg == "*开")
            {
                _data.isRefresh = true;
                SaveRaidData();
            }
        }
    }
    
    private async Task ShowUesClub(GroupMsgData data, string arg)
    {
        if (data.IsAdmin )
        {
            if (arg == "使用部落")
            {
                string s = string.Empty;
                foreach (var raidRobotModel in AllInstance)
                {
                    if (raidRobotModel._data.isRefresh)
                    {
                        s += $"{raidRobotModel._data.ClubCode}-{raidRobotModel._data.ClubName}\n";
                    }
                }
                await SendGroupMsg(s);
            }

        }
    }
    
    private string GetNearAtkInfo(DateTime start, DateTime end,bool allPlayer=false)
    {
        int c = 0;
        var all = GetAtkInfo(start, end,allPlayer);
        foreach (var d in all)
        {
            c += d.Value.Count;
        }

        if (c == 0)
            return string.Empty;
        Dictionary<string, List<AttackShareInfo>> res = new Dictionary<string, List<AttackShareInfo>>();
        foreach (var info in all)
        {
            var name = _data.Player[info.Key].Name;
            if (res.ContainsKey(name))
            {
                name += _data.Player[info.Key].Code;
            }
            res.Add(name,info.Value);
        }
        var f = GetModelDir() + "AtkInfo.png";
        ClubTool.DrawAtkInfo(res,f,Card32Path);
        return f;
    }
    

    private Dictionary<string, List<AttackShareInfo>> GetAtkInfo(DateTime start, DateTime end,bool allPlayer=false)
    {
        Dictionary<string, List<AttackShareInfo>> all = new Dictionary<string, List<AttackShareInfo>>();
        foreach (var info in _data.AttackInfos)
        {
            if (info.Log!=null && info.GetTime() > start && info.GetTime()< end)
            {
                if(!all.ContainsKey(info.Player))
                    all.Add(info.Player,new List<AttackShareInfo>());
                all[info.Player].Add(info);
            }
        }
        if (allPlayer)
        {
            foreach (var (key, value) in _data.PlayerId)
            {
                if(!all.ContainsKey(value))
                    all.Add(value,new List<AttackShareInfo>());
            }
        }

        return all;
    }

    private async Task RefreshRaidDataSave()
    {
        if(!_club.HaveRaid)
            return;

        _data.ClubCode = _club.clan_raid.clan_code;
        _data.PlayerId.Clear();
        foreach (var playerData in _club.clan_raid.leaderboard)
        {
            var id = playerData.player_code;
            var name = Regex.Unescape(playerData.name);
            if(!_data.PlayerId.ContainsKey(name))
                _data.PlayerId.Add(name,id);
            if(!_data.Player.ContainsKey(id))
                _data.Player.Add(id,new PlayerData()
                {
                    Code = id,
                    Name = name
                });
            _data.ClubName = Regex.Unescape(playerData.clan_name);
            _data.Player[id].Name = name;
            _data.Player[id].RaidLevel = playerData.player_raid_level;
            _data.Player[id].SourceData = playerData;
        }

    }

    private async Task EndRaid()
    {
        if (_data.AttackInfos.Count > 0)
        {
            try
            {
                var data= GetAtkInfo(DateTime.Now - new TimeSpan(7, 0, 0, 0), DateTime.Now, true);
                Dictionary<string, List<AttackShareInfo>> res = new Dictionary<string, List<AttackShareInfo>>();
                foreach (var info in data)
                {
                    var name = _data.Player[info.Key].Name;
                    if (res.ContainsKey(name))
                    {
                        name += _data.Player[info.Key].Code;
                    }
                    res.Add(name,info.Value);
                }
                var f = GetModelDir() + "AtkInfo.png";
                ClubTool.DrawAtkInfo(res,f,Card32Path,true);
                _data.AttackInfos.Clear();
                _data.CallMe.Clear();
                _data.current = null;
                _data.currentIndex = 0;
                await SendGroupMsg("突袭结束了");
                await UploadGroupFile("AtkInfo.png", $"AtkInfo_{DateTime.Now:MM_dd_HH_mm_ss}.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await OutException(e);
            }

        }
    }
    
    private async Task ClearRaid()
    {
        _data.AttackInfos.Clear();
        _data.CallMe.Clear();
        _club = await RefreshData();
    }
    private void RefreshHp(AttackAPIInfo apiInfo)
    {
        foreach (TitanData.Part p in _data.current.parts)
        {
            var t = apiInfo.raid_state.current.parts.Find((i) => p.part_id == i.part_id);
            if (t != null)
            {
                p.current_hp = t.current_hp;
            }
        }
        _data.current.current_hp = apiInfo.raid_state.current.current_hp;
        _data.LastHPTime = apiInfo.attack_log.attackTime;
    }

    private async Task RefreshCallMe(int currentIndex, AttackAPIInfo apiInfo)
    {
        bool isNew = currentIndex != _data.currentIndex;
        if (isNew)
            _club = await RefreshData();
        else
            RefreshHp(apiInfo);
        _data.currentIndex = currentIndex;
        if(_data.CallMe.Count<=0)
            return;
        var titan = _data.current.GetBodyInfo();
        SoraMessage msg = "小助手提醒你，打突袭了！\n";
        int c = 0;
        for (var i = _data.CallMe.Count - 1; i >= 0; i--)
        {
            var info = _data.CallMe[i];
            if (info.type > 0)
            {
                if (info.type == 99 && isNew)
                {
                    c++;
                    msg.Add(SoraSegment.At(info.qq));
                    _data.CallMe.RemoveAt(i);
                }
                else if (titan.blue >= info.type)
                {
                    c++;
                    msg.Add(SoraSegment.At(info.qq));
                    _data.CallMe.RemoveAt(i);
                }
            }
            else
            {
                if (titan.bone <= info.type)
                {
                    c++;
                    msg.Add(SoraSegment.At(info.qq));
                    _data.CallMe.RemoveAt(i);
                }
            }
        }

        if (c > 0)
        {
            await SendGroupMsg(msg);
        }
    }

    public DateTime LastSaveTime;
    public TimeSpan SaveTime = new TimeSpan(0, 5, 0);
    public bool ShowAtk = false;
    private async void RefreshRaidCardDataSave(AttackAPIInfo attackInfo)
    {
        try
        {
            var atk = new AttackShareInfo();
            atk.Init(attackInfo);
            if(!_data.Player.ContainsKey(atk.Player))
                _data.Player.Add(atk.Player,new PlayerData()
                {
                    Code = atk.Player,
                    Name = atk.PlayerName,
                });
            var p = _data.Player[atk.Player];
            p.RaidLevel = atk.Data.RaidLevel;
            foreach (var (key, value) in atk.Data.Card)
            {
                p.Card[key] = value;
            }
            _data.AttackInfos.Add(atk);
            if (DateTime.Now - LastSaveTime > SaveTime)
            {
                SaveRaidData();
                LastSaveTime = DateTime.Now;
            }
            if(ShowAtk)
            {
                var f = GetModelDir() + $"ShowAtkApi{showMyCardCount++ % 5}.png";
                ClubTool.DrawAtkInfo(f, atk);
                await SendGroupMsg(Tool.Image(f));
            }
            await RefreshCallMe(attackInfo.raid_state.titan_index, attackInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await OutException(e);
        }
    }
    
    private async Task<SoraMessage> GetRaidConfig(GroupMsgData data)
    {
        return _config.GetEasyData();
    }
    

    private async Task<ClubData> RefreshData()
    {
        ClubData tmp = null;
        await Task.Run(async () =>
        {
            try
            {
                if (_config.CanUse())
                {
                    tmp = _post.RaidCurrent(GetModelDir()+"RaidCurrent.json");
                    //var msg = _post.GetForum(GetModelDir() + "GetForum.json");
                    tmp.Init();
                    if (tmp != null)
                    {
                        _club = tmp;
                        _data.current = _club.titan_lords.current;
                        _data.LastHPTime = DateTime.Now;
                    }
                    //var all= AttackShareInfo.GetAllAttackShareInfo(msg);
                    await RefreshRaidDataSave();
                    //await RefreshRaidCardDataSave(all);
                
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                tmp = null;
                SoraMessage send = new SoraMessage("/raid/current数据失效，请重新设置！！！\n");
                if(_config.SupplyQQ!=0)
                    send.Add(SoraSegment.At(_config.SupplyQQ));
                await SendGroupMsg(send);
            }
            
        });
        return tmp;
    }

    private async void RefreshClubData()
    {
        try
        {
            if (_config.CanUse())
            {
                var tmp = _post.RaidCurrent(GetModelDir() + "RaidCurrent.json");
                //var msg = _post.GetForum(GetModelDir() + "GetForum.json");
                tmp.Init();
                if (tmp != null)
                {
                    _club = tmp;
                    _data.current = _club.titan_lords.current;
                    _data.LastHPTime = DateTime.Now;
                }
                await RefreshRaidDataSave();
                SaveRaidData();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task<AllSoloRaidData> RefreshSoloRaid()
    {
        AllSoloRaidData tmp = null;
        await Task.Run(async () =>
        {
            try
            {
                if (_config.CanUse())
                {
                    tmp = _post.SoloRaid(GetModelDir()+"SoloRaid.json");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                tmp = null;
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
        _club = LoadCanBeNull<ClubData>("RaidCurrent.json");
        var config = LoadFormData<Dictionary<string, String>>("Config.json");
        try
        {
            if(_club!=null)
                _club.Init();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        for (var i = _data.AttackInfos.Count - 1; i >= 0; i--)
        {
            if(_data.AttackInfos[i].Log==null)
                _data.AttackInfos.RemoveAt(i);
        }
        InitStatic();
        RegisterFun();
        AllInstance.Add(this);
    }

    private void InitStatic()
    {
        if (DataManage == null)
        {
            DataManage = new ConfigDataManage();
            DataManage.InitJson(GetTextFromData("RaidCardInfo.json"));
        }
        InitPostApi();
    }

    private async void InitPostApi()
    {
        await Task.Delay(_lenCd * 5000);
        PostApi = new TT2PostAPI();
        PostApi.Group = Group;
        PostApi.AppToken = _config.AppToken;
        PostApi.PlayerToken = _config.PlayerToken;
        PostApi.OnAttack = RefreshRaidCardDataSave;
        PostApi.OnEx = (s) => OutException(s).Wait();
        PostApi.OnStart= () =>
        {
            ClearRaid().Wait();
        };
        PostApi.OnEnd = () =>
        {
            EndRaid().Wait();
        };
        PostApi.OnCannotConnect = (s) =>
        {
            SendGroupMsg("api连接失败" + s).Wait();
        };
        PostApi.OnConnect = (s) =>
        {
            
            SendGroupMsg("API连接成功，部落："+s.Split('"')[5]).Wait();
        };
        PostApi.OnDisconnect = () =>
        {
            SendGroupMsg("api连接断开").Wait();
        };

        PostApi.OnCycleReset = (date) =>
        {
            if (!date.next_reset_at.EndsWith('Z'))
                date.next_reset_at += "Z";
            var time=DateTime.Parse(date.next_reset_at);
            try
            {
                _club.NextAttackTime = DateTime.Parse(date.next_reset_at);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
            
            RefreshClubData();
            SendGroupMsg("次数刷新\n下次刷新时间：" + time).Wait();
        };

        PostApi.OnLog = (s) =>
        {
            File.AppendAllText(GetModelDir() + "RaidLog.log", s);
        };
        PostApi.OnConnectFaile = () =>
        {
            SendGroupMsg("api连接失败！").Wait();
        };
        PostApi.StartSocket();
    }

    public void RegisterFun()
    {
        _argFun = new Dictionary<string, Func<GroupMsgData, Task<SoraMessage>>>()
        {
            { "帮助", GetCmd },
            { "血", LookHp },
            { "伤害", LookDmg },
            { "卡名字", ShowCardName },
            { "卡显示", GetShowCard },
            { "通配卡", ShowWildCard },
            { "突袭刷新", RefreshData },
            { "查看突袭config", GetRaidConfig },
            { "我的数据", GetMyInfo },
            { "卡片数据", ShowMyCard },
            { "全员卡", GetAllCard },
            { "导出全员卡", UploadAllCard },
            { "导出攻击", UploadAtkInfo },
            { "查询攻击本轮", GetNearAtkInfoThisRound },
            { "本轮时间", GetThisRoundTime },
            { "溢伤警告", TipDmgIsOut },
            { "重置并导出攻击", ResetAtkInfo },
            { "清理", ClearMember },
            { "StopClient", StopClient }
        };

        _argLenFun = new Dictionary<string, Func<GroupMsgData, string, Task<SoraMessage>>>()
        {
            { "绑定", QQLinkGame },
            { "设置次数", SetAtkCount },
            { "叫我", CallMeWhile },
            { "个突", ShowSoloRaid },
            { "突袭模拟", RaidCardCal },
            { "导入个人数据", InputPlayerData },
            { "查询攻击时间", GetNearAtkInfo },
            { "查询攻击卡片", GetAtkInfoByCard },
            { "查询血量变动", LookLastHPChange },
            { "谁用了", GetWhoUesCard },
            { "查看卡", ShowCardInfo },
            { "添加卡显示", AddShowCard },
            { "移除卡显示", RemoveShowCard },
            { "解析buff文件", ParseBuffInfoFile },
            { "curl", ParseConfigInfoFile },
            { "AT", SetAppToken },
            { "PT", SetPlayerToken },
        };
    }
    
    private void SaveRaidData()
    {
        Save(DataFile,JsonConvert.SerializeObject(_data));
    }

    public Dictionary<string, PlayerData> GetAllPlayer()
    {
        return _data.Player;
    }

    class RaidData
    {
        public Dictionary<string, string> PlayerId = new Dictionary<string, string>();
        public Dictionary<long, string> QQLink = new Dictionary<long, string>();
        public Dictionary<string, PlayerData> Player = new Dictionary<string, PlayerData>();
        public List<AttackShareInfo> AttackInfos = new List<AttackShareInfo>();
        public List<string> ShowCard = new List<string>();
        public List<CallMeInfo> CallMe = new List<CallMeInfo> ();
        public int AtkCount=30;
        public TitanData current;
        public DateTime LastHPTime;
        public int currentIndex;
        public int LastFromId;
        public bool TipNotShare;
        public bool TipDmgOut;
        public bool isRefresh = false;
        public int FailCount;
        public DateTime LastTime;
        public DateTime LastRefreshTime;
        public string ClubCode;
        public string ClubName;
    }

    class CallMeInfo
    {
        public long qq;
        public int type;
    }
    

    public class PlayerData
    {
        public Dictionary<string, int> Card = new Dictionary<string, int>();

        public string Code;
        public string Name;
        public int RaidLevel;
        public int AtkCount;
        public testrobot.PlayerData SourceData;

        public string EasyInfo()
        {
            return $"{Name}\n{Code}\n" +
                   $"突等：{RaidLevel}";
        }
    }
}