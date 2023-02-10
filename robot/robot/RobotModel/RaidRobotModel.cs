using System.Text;
using System.Text.RegularExpressions;
using ConfigData;
using Newtonsoft.Json;
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
    private static Queue<Func<Task>> _refreshQueue=new Queue<Func<Task>>();
    private static Queue<RaidRobotModel> _refreshDataQueue=new Queue<RaidRobotModel>();
    
    private static string ConfigFile = "config.json";
    private static string DataFile = "data.json";
    private TimeSpan _refreshCd = new TimeSpan(0, 0, 5, 0);
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
        var all = GetAtkInfo(start, end,true);
        foreach (var d in all)
        {
            c += d.Value.Count;
        }

        if (c == 0)
            return string.Empty;
        Dictionary<string, List<AttackShareInfo>> res = new Dictionary<string, List<AttackShareInfo>>();
        foreach (var info in all)
        {
            res.Add(_data.Player[info.Key].Name,info.Value);
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
            if (info.Time > start && info.Time< end)
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

    private async Task CheckEndRaid()
    {
        if(!_club.HaveRaid)
            return;
        var t = _club.GetCurrentTitanData();
        if (t.current_hp > 0)
            return;
        if (_data.AttackInfos.Count > 0)
        {
            var data= GetAtkInfo(DateTime.Now - new TimeSpan(7, 0, 0, 0), DateTime.Now, true);
            Dictionary<string, List<AttackShareInfo>> res = new Dictionary<string, List<AttackShareInfo>>();
            foreach (var info in data)
            {
                res.Add(_data.Player[info.Key].Name,info.Value);
            }
            var f = GetModelDir() + "AtkInfo.png";
            ClubTool.DrawAtkInfo(res,f,Card32Path,true);
            _data.AttackInfos.Clear();
            _data.TitanDmgList.Clear();
            await SendGroupMsg("突袭结束了");
            await UploadGroupFile("AtkInfo.png", $"AtkInfo_{DateTime.Now:MM_dd_HH_mm_ss}.png");
        }
    }

    private async Task RefreshCallMe(ClubData last)
    {
        if(_data.CallMe.Count<=0)
            return;
        if (!_club.HaveRaid)
        {
            _data.CallMe.Clear();
            return;
        }

        bool isNew = last.titan_lords.currentIndex != _club.titan_lords.currentIndex;
        var titan = _club.GetCurrentTitanData().GetBodyInfo();
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
    
    private async Task RefreshRaidCardDataSave(List<AttackShareInfo> cards)
    {
        int max = _data.LastFromId;
        Dictionary<string, int> share = new Dictionary<string, int>();
        foreach (var atk in cards)
        {
            if (atk.Id > max)
            {
                _data.AttackInfos.Add(atk);
                if(!share.ContainsKey(atk.Player))
                    share.Add(atk.Player,0);
                share[atk.Player] += 1;
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

        if (_club.HaveRaid)
        {
            var d = RefreshAtkAndShareCount(share);
            if (d.Count > 0)
            {
                await SendNotSharePlayer(d);
            }
        }
        _data.LastFromId = max;
        
    }

    private Dictionary<string, int> RefreshAtkAndShareCount(Dictionary<string, int> share)
    {
        CheckNewRaid();
        Dictionary<string, int> dic = new Dictionary<string, int>();
        foreach (var data in _club.clan_raid.leaderboard)
        {
            var id = data.player_code;
            var player = _data.Player[id];
            var count = data.num_attacks;
            var last = player.AtkCount;
            var lastNotShare = player.LastNotShareCount;
            
            share.TryGetValue(id, out int shareCount);
            var atk = count - last;
            var notShareThis = atk - shareCount;
            var notShare = lastNotShare + notShareThis;
            if (atk > 0)
            {
                player.LastNotShareCount = notShare;
                
                player.AtkCount = data.num_attacks;
                player.ShareCount += shareCount;
                player.NotShareCount += notShareThis;
                if(player.LastNotShareCount>0)
                    Console.WriteLine($"{Regex.Unescape(data.name)}\tA:{atk}\tS:{shareCount}\tL:{player.LastNotShareCount}");
            }
            else
            {
                player.LastNotShareCount = notShare;
                if (player.LastNotShareCount > 0)
                {
                    dic.Add(player.Code,player.LastNotShareCount);
                    player.LastNotShareCount = 0;
                }
            }

        }

        return dic;
    }

    private async Task SendNotSharePlayer(Dictionary<string, int> data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("以下成员打了没分享");
        foreach (var (key, value) in data)
        {
            sb.Append($"\n[{_data.Player[key].Name}]-[{value}]");
        }
        Console.WriteLine(sb);
        if (_data.TipNotShare)
            await SendGroupMsg(sb.ToString());
    }

    private void CheckNewRaid()
    {
        int saveCount = 0;
        int allCount = 0;
        foreach (var data in _club.clan_raid.leaderboard)
        {
            var player = _data.Player[data.player_code];
            saveCount += player.AtkCount;
            allCount += data.num_attacks;
        }

        if (saveCount > allCount)
        {
            NewRaid();
        }
    }

    private void NewRaid()
    {
        foreach (var data in _data.Player)
        {
            data.Value.AtkCount = 0;
            data.Value.ShareCount = 0;
            data.Value.NotShareCount = 0;
        }
    }



    private async Task<SoraMessage> GetRaidConfig(GroupMsgData data)
    {
        return _config.GetEasyData();
    }

    private async Task RefreshClubData()
    {
        try
        {
            Console.WriteLine($"{DateTime.Now}-{Group}-部落刷新数据");
            _data.LastRefreshTime=DateTime.Now;
            if (_config.CanUse() && _data.isRefresh)
            {
                ClubData tmp = _post.RaidCurrent(GetModelDir() + "RaidCurrent.json");
                var msg = _post.GetForum(GetModelDir() + "GetForum.json");
                var all = AttackShareInfo.GetAllAttackShareInfo(msg);
                tmp.Init();
                var last = _club;
                if (tmp != null)
                {
                    Console.WriteLine("OK");
                    int res = await RefreshTitanDmg(last, tmp, all);
                    Console.WriteLine(res);
                    _club = tmp;
                }

                await RefreshRaidDataSave();
                await RefreshRaidCardDataSave(all);
                await RefreshCallMe(last);
                await CheckEndRaid();
                SaveRaidData();
                Console.WriteLine($"{DateTime.Now}-{Group}-部落刷新数据保存");
                _data.FailCount = 0;
            }
        }
        catch (Exception e)
        {
            _data.FailCount++;
            if (_data.FailCount > 5)
            {
                _data.isRefresh = false;
                SoraMessage send = new SoraMessage("账号数据失效，请重新设置！！！\n");
                if(_config.SupplyQQ!=0)
                    send.Add(SoraSegment.At(_config.SupplyQQ));
                await SendGroupMsg(send);
                await OutException(e);
            }

            Console.WriteLine(e);
        }
    }

    private async Task StartRefreshClubDataQueue()
    {
        while (_refreshQueue.Count>0)
        {
            var t = _refreshQueue.Dequeue();
            var data = _refreshDataQueue.Dequeue();
            if ((DateTime.Now - data._data.LastRefreshTime) >= _refreshCd)
            {
                await t.Invoke();
            }
            _refreshQueue.Enqueue(t);
            _refreshDataQueue.Enqueue(data);
            await Task.Delay(10000);
        }
    }

    private async Task<int> RefreshTitanDmg(ClubData lastClub, ClubData thisData, List<AttackShareInfo> atk)
    {
        try
        {
            if (lastClub == null)
                return -1;
            if (!thisData.HaveRaid)
                return -1;
            Dictionary<string, int> atkCount = new Dictionary<string, int>();
            double thisDmg=0;
            double lastDmg=0;
            var dmgAll = thisDmg - lastDmg;
            double titanDmg = 0;
            foreach (var playerData in thisData.clan_raid.leaderboard)
            {
                thisDmg += playerData.score;
                if(_data.Player.ContainsKey(playerData.player_code))
                {
                    var a = playerData.num_attacks - _data.Player[playerData.player_code].AtkCount;
                    if (a > 0)
                        atkCount.Add(playerData.player_code, a);
                }
            }
            foreach (var playerData in lastClub.clan_raid.leaderboard)
            {
                lastDmg += playerData.score;
            }

            List<int> atkIdList = new List<int>();
            foreach (var info in atk)
            {
                if (info.Id > _data.LastFromId)
                {
                    atkIdList.Add(info.Id);
                }
            }

            Console.WriteLine($"atkCount:{atkCount.Count}");
            if (atkCount.Count > 0)
            {
                Dictionary<TitanData.PartName, double> dmg = new Dictionary<TitanData.PartName, double>();
                if (thisData.titan_lords.currentIndex != lastClub.titan_lords.currentIndex)
                {
                    dmg.Add(TitanData.PartName.Last, lastClub.GetCurrentTitanData().current_hp);
                    titanDmg += lastClub.GetCurrentTitanData().current_hp;
                    var current = thisData.GetCurrentTitanData();
                    foreach (var part in current.parts)
                    {
                        if (Math.Abs(part.total_hp - part.current_hp) > 0.01)
                        {
                            var d = part.total_hp - part.current_hp;
                            titanDmg += d;
                            dmg.Add(part.part_id, d);
                        }
                    }
                }
                else
                {
                    var current = thisData.GetCurrentTitanData();
                    Dictionary<TitanData.PartName, double> last = new Dictionary<TitanData.PartName, double>();
                    foreach (var part in lastClub.GetCurrentTitanData().parts)
                    {
                        last.Add(part.part_id, part.current_hp);
                    }

                    foreach (var part in current.parts)
                    {
                        var d = last[part.part_id] - part.current_hp;
                        if (Math.Abs(d) > 0.01)
                        {
                            titanDmg += d;
                            dmg.Add(part.part_id, d);
                        }

                    }
                }

                double outDmg = dmgAll - titanDmg;
                if (outDmg < 100)
                    outDmg = 0;
                _data.TitanDmgList.Add(new TitanDmg()
                {
                    AttackInfoIdList = atkIdList,
                    AttackCount = atkCount,
                    OutDmg = outDmg,
                    dmg = dmg,
                    End = DateTime.Now,
                    Start = _data.LastTime
                });
                if (outDmg > 0 && _data.TipDmgOut)
                {
                    await SendGroupMsg($"伤害溢出 {outDmg.ShowNum()}");
                }
            }
            else if (atkCount.Count == 0 && atkIdList.Count > 0)
            {
                if (_data.TitanDmgList.Count > 0)
                {
                    var d = _data.TitanDmgList[^1];
                    atkIdList.ForEach(i => d.AttackInfoIdList.Add(i));
                }
            }

            _data.LastTime = DateTime.Now;
            return atkCount.Count;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -99;
        }
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
                    var msg = _post.GetForum(GetModelDir() + "GetForum.json");
                    tmp.Init();
                    if (tmp != null)
                    {
                        _club = tmp;
                    }
                    var all= AttackShareInfo.GetAllAttackShareInfo(msg);
                    await RefreshRaidDataSave();
                    await RefreshRaidCardDataSave(all);
                
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
        try
        {
            if(_club!=null)
                _club.Init();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        InitStatic();
        RegisterFun();
        CheckAtkList();
        AllInstance.Add(this);
    }

    private void InitStatic()
    {
        if (DataManage == null)
        {
            DataManage = new ConfigDataManage();
            DataManage.InitJson(GetTextFromData("RaidCardInfo.json"));
        }

        //刷新线程
        _refreshQueue.Enqueue(this.RefreshClubData);
        _refreshDataQueue.Enqueue(this);
        if (_refreshDataTask == null)
        {
            _refreshDataTask = Task.Run(StartRefreshClubDataQueue);
        }
    }

    private void CheckAtkList()
    {
        HashSet<int> idSet = new HashSet<int>();
        for (var i = _data.AttackInfos.Count - 1; i >= 0; i--)
        {
            var d = _data.AttackInfos[i];
            if (idSet.Contains(d.Id))
            {
                _data.AttackInfos.RemoveAt(i);
            }
            else
            {
                idSet.Add(d.Id);
            }
        }
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
            { "分享警告", TipNotShareSwitch },
            { "溢伤警告", TipDmgIsOut },
            { "分享统计", ShareCountShow },
            { "重置并导出攻击", ResetAtkInfo }
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
        public List<TitanDmg> TitanDmgList = new List<TitanDmg>();
        public int AtkCount=30;
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

    public class TitanDmg
    {
        public double OutDmg;
        public Dictionary<TitanData.PartName, double> dmg=new Dictionary<TitanData.PartName, double>();
        public List<int> AttackInfoIdList = new List<int>();
        public Dictionary<string, int> AttackCount = new Dictionary<string, int>();
        public DateTime End;
        public DateTime Start;
    }

    public class PlayerData
    {
        public Dictionary<string, int> Card = new Dictionary<string, int>();

        public string Code;
        public string Name;
        public int RaidLevel;
        public int AtkCount;
        public int ShareCount;
        public int NotShareCount;
        public int LastNotShareCount;
        public testrobot.PlayerData SourceData;

        public string EasyInfo()
        {
            return $"{Name}\n{Code}\n" +
                   $"突等：{RaidLevel}";
        }
    }
}