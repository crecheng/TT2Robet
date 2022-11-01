﻿using System.Text;
using Newtonsoft.Json;
using testrobot;
// ReSharper disable All
#pragma warning disable CS8600

namespace robot.RobotModel;

public partial class RaidRobotModel
{
    #region 无参的命令

    /// <summary>
    /// 刷新
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> RefreshData(GroupMsgData data)
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
            {
                return "失败！";

            }
        }
        else
        {
            return "还没配置文件";
        }

        await Task.CompletedTask;
    }

    private async Task<SoraMessage> ShowCardName(GroupMsgData data)
    {
        var f = GetModelDir() + "ShowCardName.png";
        ClubTool.DrawCardName(f,Card64Path);
        return Tool.Image(f);
        await Task.CompletedTask;
    }
    
    private async Task<SoraMessage> GetShowCard(GroupMsgData data)
    {
        string s=String.Empty;
        _data.ShowCard.ForEach(i=>s+=ClubTool.CardName[i]+",");
        return "当前卡显示：\n" + s;
        await Task.CompletedTask;
    }

    private async Task<SoraMessage> ResetAtkInfo(GroupMsgData data)
    {
        if(_club==null||_data.AttackInfos.Count==0)
            return "没有数据！";
        if (data.IsAdmin|| data.IsGroupAdmin)
        {
            await UploadAtkInfo(data);
            _data.AttackInfos.Clear();
            return SoraMessage.Null;
        }

        return "你没权限";
    }

    /// <summary>
    /// 查看血量
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> LookHp(GroupMsgData data)
    {
        if (_club == null)
        {
            return "没有数据！";
        }
        else
        {
            if (!_club.HaveRaid)
                return "当前没有突袭";
            var f = GetModelDir() + "kkHp.png";
            _club.GetCurrentTitanData().DrawTitan(f);
            return Tool.Image(f);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 查看伤害
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> LookDmg(GroupMsgData data)
    {
        if (_club == null)
        {
            return "没有数据！";
        }
        else
        {
            if (!_club.HaveRaid)
                return "当前没有突袭";

            var last = _club.NextAttackTime - new TimeSpan(20, 0, 0);
            var atkInfo = GetAtkInfo(last, DateTime.Now - new TimeSpan(8, 0, 0),true);
            List<string> set = new List<string>(_data.ShowCard);
            Dictionary<string, List<string>> showCard = new Dictionary<string, List<string>>();
            foreach (var (key, list) in atkInfo)
            {
                set.Clear();
                set.AddRange(_data.ShowCard);
                foreach (var info in list)
                {
                    foreach (var (card, value) in info.Data.Card)
                    {
                        set.Remove(card);
                    }
                }

                if (set.Count > 0)
                    showCard.Add(key,new List<string>(set));
                
            }


            var f = GetModelDir() + "kkDmg.png";
            ClubTool.DrawPlayerRaidDmg(_club, f, _data.AtkCount, showCard, _data.ShowCard.Count,
                Card32Path);
            return Tool.Image(f);
        }

        await Task.CompletedTask;
    }

    private async Task<SoraMessage> GetCmd(GroupMsgData data)
    {
        List<string> s = new List<string>();
        foreach (var key in _argFun.Keys)
        {
            s.Add(key);
        }

        foreach (var key in _argLenFun.Keys)
        {
             s.Add(key + "{参数}");
        }

        var f = GetModelDir() + "cmd.png";
        ClubTool.DrawInfo(s, f);
        return Tool.Image(f);
        await Task.CompletedTask;
    }


    /// <summary>
    /// 本轮时间
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> GetThisRoundTime(GroupMsgData data)
    {
        if (_club == null)
            return "还没数据";
        if (!_club.HaveRaid)
            return "当前没有突袭";
        return $"{(_club.NextAttackTime - new TimeSpan(12, 0, 0)):dd-HH:mm:ss}\n{_club.NextAttackTime:dd-HH:mm:ss}";
        await Task.CompletedTask;
    }

    /// <summary>
    /// 我的数据
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> GetMyInfo(GroupMsgData data)
    {
        if (_data.QQLink.ContainsKey(data.Sender))
        {
            var player = _data.Player[_data.QQLink[data.Sender]];
            if (player.SourceData != null)
            {
                var info = player.SourceData.GetInfo();
                var f = GetModelDir() + $"GetMyInfo{showMyCardCount++ % 5}.png";
                ClubTool.DrawInfo(info,f,150);
                return Tool.Image(f);
            }
            else
                return player.EasyInfo();
        }

        return "你还没绑定";
        await Task.CompletedTask;
    }

    /// <summary>
    /// 我的卡片
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> ShowMyCard(GroupMsgData data)
    {
        if (_data.QQLink.ContainsKey(data.Sender))
        {
            var p = _data.Player[_data.QQLink[data.Sender]];
            if (p.Card.Count == 0)
                return "你还没卡片数据";
            var f = GetModelDir() + $"ShowMyCard{showMyCardCount++ % 5}.png";
            Dictionary<string, string> dic = null;
            if (p.SourceData != null)
                dic = p.SourceData.GetInfo();
            ClubTool.DrawPlayerCard(p.Name, p.Card, dic, f, Card32Path);
            return Tool.Image(f);
        }

        return "你还没绑定";
        await Task.CompletedTask;
    }

    private async Task<SoraMessage> TipNotShareSwitch(GroupMsgData data)
    {
        if (data.IsAdmin|| data.IsGroupAdmin)
        {
            _data.TipNotShare = !_data.TipNotShare;
            return $"分享警告-{_data.TipNotShare}";
        }

        return "你没权限！";
        await Task.CompletedTask;

    }

    /// <summary>
    /// 上传全员卡
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> UploadAllCard(GroupMsgData data)
    {
        if (_club == null)
            return "还没数据";
        if (!(data.IsAdmin|| data.IsGroupAdmin))
            return "你没权限！";

        Dictionary<string, Dictionary<string, int>> dic = new Dictionary<string, Dictionary<string, int>>();
        foreach (var (key, value) in _data.Player)
        {
            dic.Add(value.Name, value.Card);
        }

        foreach (var (key, value) in _data.PlayerId)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, new Dictionary<string, int>());
        }

        var text = ClubTool.GetAllCardString(dic);
        Save("allCard.csv", text);
        await Task.Delay(500);
        await UploadGroupFile("allCard.csv", $"allCard_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.csv");
        return SoraMessage.Null;
    }

    /// <summary>
    /// 本轮攻击
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> GetNearAtkInfoThisRound(GroupMsgData data)
    {
        if (!_club.HaveRaid)
            return "当前没有突袭";
        var last = _club.NextAttackTime - new TimeSpan(20, 0, 0);
        var f = GetNearAtkInfo(last, DateTime.Now - new TimeSpan(8, 0, 0),true);
        if (!string.IsNullOrEmpty(f))
            return Tool.Image(f);
        return "没有数据";
        await Task.CompletedTask;
    }

    /// <summary>
    /// 全员卡
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> GetAllCard(GroupMsgData data)
    {
        if (_club == null)
            return "还没数据";
        Dictionary<string, Dictionary<string, int>> dic = new Dictionary<string, Dictionary<string, int>>();
        foreach (var (key, value) in _data.Player)
        {
            dic.Add(value.Name, value.Card);
        }

        foreach (var (key, value) in _data.PlayerId)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, new Dictionary<string, int>());
        }

        var f = GetModelDir() + "allCard.png";
        ClubTool.DrawAllPlayerCard(dic, f, Card32Path);
        return Tool.Image(f);
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// 导出攻击
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> UploadAtkInfo(GroupMsgData data)
    {
        if (_club == null)
            return "还没数据";
        if (!(data.IsAdmin|| data.IsGroupAdmin))
            return "你没权限！";
        _data.AttackInfos.Sort((x,y)=>x.Id-y.Id);

        StringBuilder sb = new StringBuilder();
        sb.Append("名字,时间,卡1,等级,卡2,等级,卡3,等级,突袭等级,伤害");
        foreach (var info in _data.AttackInfos)
        {
            sb.Append('\n');
            sb.Append(_data.Player[info.Player].Name).Append(',');
            sb.Append(info.Time+new TimeSpan(8,0,0)).Append(',');
            int c = 0;
            foreach (var i in info.Data.Card)
            {
                c++;
                sb.Append($"{ClubTool.CardName[i.Key]}-{i.Key},{i.Value},");
            }

            for (; c < 3; c++)
            {
                sb.Append($",0,");
            }

            sb.Append(info.Data.RaidLevel).Append(',');
            sb.Append(info.Data.Dmg);
        }
        Save("AtkInfo.csv",sb.ToString());
        await UploadGroupFile("AtkInfo.csv", $"AtkInfo_{DateTime.Now:MM_dd_HH_mm_ss}.csv");
        return SoraMessage.Null;
    }

    private async Task<SoraMessage> ShareCountShow(GroupMsgData data)
    {
        List<string> player = new List<string>(_data.Player.Keys);
        player.Sort((x,y)=>_data.Player[y].NotShareCount-_data.Player[x].NotShareCount);
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("名字",$"次数\t分享\t没分享");
        foreach (var id in player)
        {
            var p = _data.Player[id];
            dic.Add(p.Name,$"{p.AtkCount}\t{p.ShareCount}\t{p.NotShareCount}");
        }
        
        var f = GetModelDir() + "ShareCountShow.png";
        ClubTool.DrawInfo(dic, f, 300);
        return Tool.Image(f);
        await Task.CompletedTask;
    }


    #endregion

    #region 带参数的命令

    private async Task<SoraMessage> GetNearAtkInfo(GroupMsgData data, string arg)
    {
        if (_club == null)
            return "还没数据";
        if (!_club.HaveRaid)
            return "当前没有突袭";
        if (int.TryParse(arg, out int time))
        {
            TimeSpan t = new TimeSpan(8, 0, time);
            var start = DateTime.Now - t;
            var end = DateTime.Now - new TimeSpan(8, 0, 0);
            var f = GetNearAtkInfo(start, end);
            if (!string.IsNullOrEmpty(f))
                return Tool.Image(f);
            else
                return "没有数据";
        }
        else
        {
            return "格式不对";
        }
        await Task.CompletedTask;
    }
    
    private async Task<SoraMessage> GetAtkInfoByCard(GroupMsgData data, string card)
    {
        if (_club == null)
            return "还没数据";
        if (!_club.HaveRaid)
            return "当前没有突袭";
        if (string.IsNullOrEmpty(card))
            return "请正确输入，如：查询攻击卡片凯旋";
        var id = ClubTool.NameToIDCard(card);
        if (string.IsNullOrEmpty(card))
            return "没找到对于简称，查看简称可以命令：卡名字";
        Dictionary<string, List<AttackShareInfo>> dic = new Dictionary<string, List<AttackShareInfo>>();
        int c = 0;
        foreach (var info in _data.AttackInfos)
        {
            if (info.Data.Card.ContainsKey(id))
            {
                if(!dic.ContainsKey(info.Player))
                    dic.Add(info.Player,new List<AttackShareInfo>());
                dic[info.Player].Add(info);
                c++;
            }
        }
        if(c==0)
            return "没有数据";
        
        Dictionary<string, List<AttackShareInfo>> res = new Dictionary<string, List<AttackShareInfo>>();
        foreach (var (key, value) in dic)
        {
            res.Add(_data.Player[key].Name,value);
        }
        var f = GetModelDir() + "GetAtkInfoByCard.png";
        ClubTool.DrawAtkInfo(res,f,Card32Path);
        return Tool.Image(f);
        await Task.CompletedTask;
    }
    private async Task<SoraMessage> SetAtkCount(GroupMsgData data, string count)
    {
        if (!(data.IsAdmin|| data.IsGroupAdmin))
            return "你没权限！";
        if (int.TryParse(count, out int time))
        {
            _data.AtkCount = time;
            return "设置次数-" + _data.AtkCount;
        }
        else
        {
            return "格式不对";
        }

        return SoraMessage.Null;
        await Task.CompletedTask;
    }
    private async Task<SoraMessage> QQLinkGame(GroupMsgData data, string code)
    {
        var sender = data.Sender;
        string tip = $"请正确输入\n列如：\n{_startString}游戏绑定璀璨\n{_startString}游戏绑定abc123";
        if (string.IsNullOrEmpty(code))
        {
            return tip;
        }

        if (_data.PlayerId.ContainsKey(code))
        {
            var c = _data.PlayerId[code];
            if (!_data.QQLink.ContainsKey(sender))
            {
                _data.QQLink.Add(sender, c);
            }

            _data.QQLink[sender] = c;
            if (_data.Player.ContainsKey(c))
            {
                var p = _data.Player[c];

                return $"绑定成功-{p.Code}-{p.Name}";
            }
            else
            {
                return $"绑定成功-{code}";
            }

        }
        else if (_data.Player.ContainsKey(code))
        {
            if (!_data.QQLink.ContainsKey(sender))
            {
                _data.QQLink.Add(sender, code);
            }

            var p = _data.Player[code];
            _data.QQLink[sender] = code;
            return $"绑定成功-{p.Code}-{p.Name}";
        }
        else
        {
            return "没有找到\n" + tip;
        }
        await Task.CompletedTask;
    }
    private async Task<SoraMessage> GetWhoUesCard(GroupMsgData data, string card)
    {
        if (string.IsNullOrEmpty(card))
            return "请正确输入，如：谁用了凯旋";
        var id = ClubTool.NameToIDCard(card);
        if (string.IsNullOrEmpty(card))
            return "没找到对于简称，查看简称可以命令：卡名字";
        Dictionary<string, int> res = new Dictionary<string, int>();
        res.Add("总次数",0);
        foreach (var d in _data.AttackInfos)
        {
            if (d.Data.Card.ContainsKey(id))
            {
                if(!res.ContainsKey(d.Player))
                    res.Add(d.Player,0);
                res[d.Player] += 1;
                res["总次数"] += 1;
            }
        }

        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("总次数",res["总次数"].ToString());
        res.Remove("总次数");
        
        List<string> player = new List<string>(_data.PlayerId.Values);
        player.Sort((x, y) =>
        {
            var xx = res.ContainsKey(x) ? res[x] : 0;
            var yy = res.ContainsKey(y) ? res[y] : 0;
            return yy - xx;
        });
        
        foreach (var key in player)
        {
            var value=res.ContainsKey(key) ? res[key] : 0;
            dic.Add(_data.Player[key].Name,value.ToString());
        }
        var f=GetModelDir() + "GetWhoUesCard.png";
        ClubTool.DrawInfo(dic,f);
        return Tool.Image(f);
        await Task.CompletedTask;
    }
    
    private async Task<SoraMessage> AddShowCard(GroupMsgData data, string card)
    {
        if (string.IsNullOrEmpty(card))
            return "请正确输入，如：添加卡显示凯旋";
        var id = ClubTool.NameToIDCard(card);
        if (string.IsNullOrEmpty(card))
            return "没找到对于简称，查看简称可以命令：卡名字";
        _data.ShowCard.Remove(id);
        _data.ShowCard.Add(id);
        string s=String.Empty;
        
        _data.ShowCard.ForEach(i=>s+=ClubTool.CardName[i]+",");
        return "添加成功\n当前卡显示：\n" + s;
        await Task.CompletedTask;
    }
    
    private async Task<SoraMessage> CallMeWhile(GroupMsgData data, string type)
    {
        if (string.IsNullOrEmpty(type))
            return "请正确输入，如：叫我5骨";
        int i = 0;
        var first = type.Substring(0, 1);
        var end = type.Substring(1);
        switch (first)
        {
            case "1": i = 1; break;
            case "2": i = 2; break;
            case "3": i = 3; break;
            case "4": i = 4; break;
            case "5": i = 5; break;
            case "6": i = 6; break;
            case "7": i = 7; break;
            case "8": i = 8; break;
        }
        
        
        if (i == 0)
        {
            first = type.Substring(0, 2);
            end = type.Substring(2);
            switch (first)
            {
                case "一": i = 1; break;
                case "二": i = 2; break;
                case "三": i = 3; break;
                case "四": i = 4; break;
                case "五": i = 5; break;
                case "六": i = 6; break;
                case "七": i = 7; break;
                case "八": i = 8; break;
            }
        }
        if (i==0)
            return "请正确输入，如：叫我5骨";
        int sing = 1;
        if (end == "蓝" || end == "肉")
            sing = 1;
        else if (end == "骨")
            sing = -1;
        var info = new CallMeInfo()
        {
            qq = data.Sender,
            type = i * sing
        };
        _data.CallMe.Add(info);

        return $"小助手知道了，{type}会叫你的";
        await Task.CompletedTask;
    }
    private async Task<SoraMessage> RemoveShowCard(GroupMsgData data, string card)
    {
        if (string.IsNullOrEmpty(card))
            return "请正确输入，如：移除卡显示凯旋";
        var id = ClubTool.NameToIDCard(card);
        if (string.IsNullOrEmpty(card))
            return "没找到对于简称，查看简称可以命令：卡名字";
        _data.ShowCard.Remove(id);
        string s=String.Empty;
        
        _data.ShowCard.ForEach(i=>s+=ClubTool.CardName[i]+",");
        return "移除成功\n当前卡显示：\n" + s;
        await Task.CompletedTask;
    }

    private async Task<SoraMessage> LookLastHPChange(GroupMsgData data, string time)
    {
        if (string.IsNullOrEmpty(time))
            return "请正确输入，如：查询血量变动1";
        if (int.TryParse(time, out int t))
        {
            if (t <= 0 || t > _data.TitanDmgList.Count)
            {
                return "当前只能查询1~" + _data.TitanDmgList.Count;
            }
            var d = _data.TitanDmgList[_data.TitanDmgList.Count - t];
            Dictionary<string, List<AttackShareInfo>> atk = new Dictionary<string, List<AttackShareInfo>>();
            HashSet<int> set = new HashSet<int>(d.AttackInfoIdList);
            for (var i = _data.AttackInfos.Count - 1; i >= 0; i--)
            {
                var info = _data.AttackInfos[i];
                if (set.Contains(info.Id))
                {
                    if (!atk.ContainsKey(info.Player))
                        atk.Add(info.Player, new List<AttackShareInfo>());
                    atk[info.Player].Add(info);
                    set.Remove(info.Id);
                    if (set.Count <= 0)
                        break;
                }
            }

            Dictionary<string, List<AttackShareInfo>> atkRes = new Dictionary<string, List<AttackShareInfo>>();
            foreach (var (key, value) in atk)
            {
                atkRes.Add(_data.Player[key].Name,value);
            }

            Dictionary<string, int> atkCount = new Dictionary<string, int>();
            foreach (var (key, value)  in d.AttackCount)
            {
                atkCount.Add(_data.Player[key].Name,value);
            }
            var f = GetModelDir() + "LookLastHPChange.png";
            ClubTool.DrawTitanHPChangeInfo(d.dmg,atkCount,atkRes,d.Start,d.End,f,Card32Path);
            return Tool.Image(f);
        }
        else 
        {
            return "请正确输入，如：查询血量变动1";
        }
        
        return SoraMessage.Null;
    }

    private async Task<SoraMessage> ParseBuffInfoFile(GroupMsgData data, string file)
    {
        if (!(data.IsAdmin|| data.IsGroupAdmin))
            return "你没权限！";
        if(string.IsNullOrEmpty(file))
            return "请正确输入，如：解析buff文件1.json";
        var s= await DownloadGroupFile(file);
        switch (s)
        {
            case "-1":
                return "群文件没有找到改文件"; break;
            case "-2":
                return "获取文件失败";
            break;
        }

        try
        {
            var text = File.ReadAllText(s);
            var list= JsonConvert.DeserializeObject<List<TitanBuffInfo>>(text);
            HashSet<string> area = new HashSet<string>();
            HashSet<string> boss = new HashSet<string>();
            StringBuilder sb = new StringBuilder();
            sb.Append("区域,等级,Buff加持,减益,诅咒,数量,顺序");
            foreach (var info in list)
            {
                sb.Append($"\n{info.tier},{info.level},");
                if (info.area_buffs != null)
                    sb.Append($"{info.area_buffs[0]}");
                sb.Append(',');
                info.titans?.ForEach(i =>
                {
                    if (i.area_debuffs != null)
                    {
                        sb.Append($"{i.enemy_name}:{i.area_debuffs[0]} | ");
                    }
                });
                
                sb.Append(',');
                info.titans?.ForEach(i =>
                {
                    if (i.cursed_debuffs != null)
                    {
                        sb.Append($"{i.enemy_name}:");
                        i.parts.ForEach(p =>
                        {
                            if (p.cursed)
                                sb.Append(TitanData.PartNameShowNo(p.part_id)).Append('-');
                        });
                        sb.Append($"({i.cursed_debuffs[0]}) | ");
                    }
                });
                
                sb.Append(',');
                sb.Append(info.spawn_sequence.Count);
                
                sb.Append(',');
                info.spawn_sequence.ForEach(i=>sb.Append(i).Append('>'));
            }
            Save("buff.csv",sb.ToString());
            await UploadGroupFile("buff.csv", $"buff{DateTime.Now:yy_MM_dd_HH_mm_ss}.csv");
            return SoraMessage.Null;
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "解析失败";
        }
    }

    #endregion
}