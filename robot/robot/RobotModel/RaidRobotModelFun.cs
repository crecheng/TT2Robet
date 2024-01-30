using System.Text;
using System.Text.RegularExpressions;
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
            if (!(data.IsAdmin|| data.IsGroupAdmin))
                return "你没权限！";
            if (string.IsNullOrEmpty(PostApi.AppToken) || string.IsNullOrEmpty(PostApi.PlayerToken))
                return "API没有APPToken或PlayerToken";
            await SendGroupMsg("刷新ing");
            var t = await RefreshData();
            await PostApi.CheckReStart();
            if (t != null)
            {
                _club = t;
                _data.current = _club.titan_lords.current;
                _data.isRefresh = true;
                _data.FailCount = 0;
                return "刷新成功！";
            }
            else
            {
                _data.isRefresh = false;
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
    
    /// <summary>
    /// 通配卡
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> ShowWildCard(GroupMsgData data)
    {
        if (data.IsAdmin|| data.IsGroupAdmin)
        {
            List<string> player = new List<string>(_data.Player.Keys);
            player.Sort((x, y) =>
            {
                return _data.Player[y].RaidLevel - _data.Player[x].RaidLevel;
            });
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("名字","通配卡\t突等");
            foreach (var id in player)
            {
                var p = _data.Player[id];
                string b = "未知";
                //if (p.SourceData != null)
                //    b = p.SourceData.raid_wildcard_count.ToString();
                dic.Add(p.Name, $"{b}\t{p.RaidLevel}");
            }
            var f= GetModelDir() + "ShowWildCard.png";
            ClubTool.DrawInfo(dic,f,200);
            return Tool.Image(f);
        }

        return "你没权限";
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
            if (!_club.HaveRaid && _data.current==null)
                return "当前没有突袭";
            var f = GetModelDir() + "kkHp.png";
            _data.current.DrawTitan(_club,f,_data.LastHPTime);
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
            if (DateTime.Now-_data.LastRefreshTime> TimeSpan.FromMinutes(10))
            {
                await SendGroupMsg("获取中。。。");
                var t = await RefreshData();
                if (t != null)
                {
                    _club = t;
                    _data.isRefresh = true;
                    _data.LastRefreshTime = DateTime.Now;
                }
                else
                {
                    _data.isRefresh = false;
                    return "失败！";

                }
            }
            
            var last = _club.NextAttackTime - new TimeSpan(12, 0, 0);
            var atkInfo = GetAtkInfo(last, DateTime.Now ,true);
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
    }

    private async void LookDmgImg()
    {
        SoraMessage msg = default;
        bool send = false;
        if (_club == null)
        {
            return ;
        }
        else
        {
            if (!_club.HaveRaid)
                return ;
            
            var t = await RefreshData();
            if (t != null)
            {
                _club = t;
                _data.isRefresh = true;
                _data.LastRefreshTime = DateTime.Now;
            }
            else
            {
                _data.isRefresh = false;
                return ;

            }

            var last = _club.NextAttackTime - new TimeSpan(12, 0, 0);
            var atkInfo = GetAtkInfo(last, DateTime.Now ,true);
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
            msg= Tool.Image(f);
            send = true;
        }

        if(send) 
            await SendGroupMsg(msg);
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
        // if (_data.QQLink.ContainsKey(data.Sender))
        // {
        //     var player = _data.Player[_data.QQLink[data.Sender]];
        //     if (player.SourceData != null)
        //     {
        //         var info = player.SourceData.GetInfo();
        //         var f = GetModelDir() + $"GetMyInfo{showMyCardCount++ % 5}.png";
        //         ClubTool.DrawInfo(info,f,150);
        //         return Tool.Image(f);
        //     }
        //     else
        //         return player.EasyInfo();
        // }
        //
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
            //if (p.SourceData != null)
            //    dic = p.SourceData.GetInfo();
            ClubTool.DrawPlayerCard(p.Name, p.Card, null, f, Card32Path);
            return Tool.Image(f);
        }

        return "你还没绑定";
        await Task.CompletedTask;
    }
    
    private async Task<SoraMessage> TipDmgIsOut(GroupMsgData data)
    {
        if (data.IsAdmin|| data.IsGroupAdmin)
        {
            _data.TipDmgOut = !_data.TipDmgOut;
            return $"溢伤提醒-{_data.TipDmgOut}";
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
        var last = _club.NextAttackTime - new TimeSpan(12, 0, 0);
        var f = GetNearAtkInfo(last, DateTime.Now,true);
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
            var name = value.Name;
            if (dic.ContainsKey(name))
            {
                name += value.Code;
            }
            dic.Add(name, value.Card);
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
    private async Task<SoraMessage> UploadAtkInfo(GroupMsgData msgArgs)
    {
        if (_club == null)
            return "还没数据";
        if (!(msgArgs.IsAdmin|| msgArgs.IsGroupAdmin))
            return "你没权限！";
        
        var data= GetAtkInfo(DateTime.Now - new TimeSpan(7, 0, 0, 0), DateTime.Now, true);
        Dictionary<string, List<AttackShareInfo>> res = new Dictionary<string, List<AttackShareInfo>>();
        foreach (var info in data)
        {
            if (_data.Player.ContainsKey(info.Key))
            {
                if(!res.ContainsKey(_data.Player[info.Key].Name))
                    res.Add(_data.Player[info.Key].Name,info.Value);
                else
                    res.Add(_data.Player[info.Key].Name+info.Key,info.Value);
            }
            else
                res.Add(info.Key,info.Value);
        }
        var f = GetModelDir() + "AtkInfo.png";
        ClubTool.DrawAtkInfo(res,f,Card32Path,true);
        await UploadGroupFile("AtkInfo.png", $"AtkInfo_{DateTime.Now:MM_dd_HH_mm_ss}.png");
        
        _data.AttackInfos.Sort((x,y)=>DateTime.Compare(x.GetTime(),y.GetTime()));
        StringBuilder sb = new StringBuilder();
        sb.Append("名字,时间,卡1,等级,卡2,等级,卡3,等级,突袭等级,伤害");
        foreach (var info in _data.AttackInfos)
        {
            sb.Append('\n');
            if(_data.Player.ContainsKey(info.Player))
                sb.Append(_data.Player[info.Player].Name).Append(',');
            else
                sb.Append(info.Player).Append(',');
            sb.Append(info.GetTime()+new TimeSpan(8,0,0)).Append(',');
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
            TimeSpan t = new TimeSpan(0, 0, time);
            var start = DateTime.Now - t;
            var end = DateTime.Now ;
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
        if (string.IsNullOrEmpty(id))
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
        string playerName;
        PlayerData player;
        foreach (var (key, value) in dic)
        {
            if (!_data.Player.TryGetValue(key, out player))
                playerName = key;
            else
                playerName = player.Name;
            
            if(res.ContainsKey(playerName))
                res.Add($"{playerName}_{key}",value);
            else
                res.Add(playerName,value);
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
        string tip = $"请正确输入\n列如：\n{_startString}绑定璀璨\n{_startString}绑定abc123";
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
        if (string.IsNullOrEmpty(id))
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

    private async Task<SoraMessage> ShowCardInfo(GroupMsgData data, string card)
    {
        if (string.IsNullOrEmpty(card))
            return "请正确输入，如：查看卡凯旋";
        var id = ClubTool.NameToIDCard(card);
        if (string.IsNullOrEmpty(id))
            return "没找到对于简称，查看简称可以命令：卡名字";
        var cardData = DataManage.GetCardDataDataFirst(id);
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
        var f=GetModelDir() + "ShowCardInfo.png";
        ClubTool.DrawInfo(dic,f,800);
        return Tool.Image(f);
    }

    private async Task<SoraMessage> AddShowCard(GroupMsgData data, string card)
    {
        if (string.IsNullOrEmpty(card))
            return "请正确输入，如：添加卡显示凯旋";
        var id = ClubTool.NameToIDCard(card);
        if (string.IsNullOrEmpty(id))
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

        if (type == "新王")
            i = 99;
        if (i==0)
            return "请正确输入，如：叫我5骨";
        int sing = 1;
        if (end == "蓝" || end == "肉")
            sing = 1;
        else if (end == "骨")
            sing = -1;
        var t = i * sing;
        if (t < -7)
            return "小助手不会叫你的";
        var info = new CallMeInfo()
        {
            qq = data.Sender,
            type = t
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
        if (string.IsNullOrEmpty(id))
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
        // if (time == "全部")
        // {
        //     if (_data.TitanDmgList.Count == 0)
        //         return "还没有数据";
        //     Dictionary<string, string> dic = new Dictionary<string, string>();
        //     dic.Add("时间段","伤害\t\t次数\t序号");
        //     int j = 0;
        //     int max = 0;
        //     for (var i = _data.TitanDmgList.Count - 1; i >= 0; i--)
        //     {
        //         var info = _data.TitanDmgList[i];
        //         double dmg = 0;
        //         string other=String.Empty;
        //         
        //         foreach (var (key, value) in info.dmg)
        //         {
        //             other+=$"{TitanData.PartNameShow(key)}-{value.ShowNum()} ";
        //             dmg += value;
        //         }
        //
        //         max = Math.Max(max, other.Length);
        //
        //         int count = 0;
        //         foreach (var (key, value) in info.AttackCount)
        //         {
        //             count += value;
        //         }
        //
        //         var k = $"{info.Start:HH:mm:ss}-{info.End:HH:mm:ss}";
        //         if(dic.ContainsKey(k))
        //             dic.Add(k+"_1",$"{dmg.ShowNum()}    \t{count}\t{++j}\t{other}");
        //         else
        //             dic.Add(k,$"{dmg.ShowNum()}    \t{count}\t{++j}\t{other}");
        //     }
        //
        //     var f = GetModelDir() + "LookLastHPChangeAll.png";
        //     ClubTool.DrawInfo(dic,f,200+max*19);
        //     return Tool.Image(f);
        // }
        if (int.TryParse(time, out int t))
        {
            if (t <= 0 || t > _data.AttackInfos.Count)
            {
                return "当前只能查询1~" + _data.AttackInfos.Count;
            }
            var d = _data.AttackInfos[_data.AttackInfos.Count - t];
            var f = GetModelDir() + "LookLastHPChange.png";
            ClubTool.DrawAtkInfo(f,d);
            if (DateTime.Now - LastSaveTime > SaveTime)
            {
                SaveRaidData();
                LastSaveTime=DateTime.Now;
            }
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
        if (!(data.IsAdmin || data.IsGroupAdmin))
            return "你没权限！";
        if (string.IsNullOrEmpty(file))
            return "请正确输入，如：解析buff文件1.json";
        var s = await DownloadGroupFile(file);
        switch (s)
        {
            case "-1":
                return "群文件没有找到该文件";
                break;
            case "-2":
                return "获取文件失败";
                break;
        }

        try
        {
            var text = File.ReadAllText(s);
            var list = JsonConvert.DeserializeObject<List<TitanBuffInfo>>(text);
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
                        sb.Append(i.enemy_name).Append(':');
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
                info.spawn_sequence.ForEach(i => sb.Append(i).Append('>'));
            }

            Save("buff.csv", sb.ToString());
            await UploadGroupFile("buff.csv", $"buff{DateTime.Now:yy_MM_dd_HH_mm_ss}.csv");
            return SoraMessage.Null;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await OutException(e);
            return "解析失败";
        }
    }

    private async Task<SoraMessage> SetAppToken(GroupMsgData data, string token)
    {
        if (string.IsNullOrEmpty(token))
            return "无效";
        if (token.StartsWith(" "))
            token= token.Substring(1, token.Length - 1);
        _config.AppToken = token;
        PostApi.AppToken = token;
        Save("config.json",_config.Save());
        return "更新成功\n你可以撤回了";
    }
    
    private async Task<SoraMessage> SetPlayerToken(GroupMsgData data, string token)
    {
        if (string.IsNullOrEmpty(token))
            return "无效";
        if (token.StartsWith(" "))
            token= token.Substring(1, token.Length - 1);
        _config.PlayerToken = token;
        PostApi.PlayerToken = token;
        _config.SupplyQQ = data.Sender;
        Save("config.json",_config.Save());
        return "更新成功\n你可以撤回了";
    }

    private async Task<SoraMessage> ParseConfigInfoFile(GroupMsgData data, string curl)
    {
        if (!(data.IsAdmin || data.IsGroupAdmin))
            return "你没权限！";
        if (string.IsNullOrEmpty(curl))
            return "请正确复制curl";
        try
        {
            var text = curl.Split(' ');
            if (text.Length <= 1)
                return "数据不对，请输入正确的curl";
            List<string> marge = new List<string>();
            for (var i = 0; i < text.Length; i++)
            {
                var a = text[i];
                if (a.StartsWith("-"))
                {
                    marge.Add(a);
                    continue;
                }else if (a.StartsWith("\""))
                {
                    if(a.EndsWith("\""))
                        marge.Add(a);
                    else
                    {
                        var next = String.Empty;
                        while (!next.EndsWith("\""))
                        {
                            next = text[++i];
                            a += " "+ next;
                        }
                        marge.Add(a);
                    }
                }else
                    marge.Add(a);
            }

            text = marge.ToArray();
            var url = text[^1];
            var json = text[text.Length-2];
            //解析content
            Dictionary<string, string> content = new Dictionary<string, string>();
            for (int i = 0; i < text.Length; i++)
            {
                var a = text[i];
                if (a == "-H")
                {
                    var c = text[++i];
                    if(c.StartsWith("\""))
                        c = c.Substring(1, c.Length - 2);
                    if (c.IndexOf(':') > 0)
                    {
                        var p = c.Split(':');
                        content.Add(p[0].ToLower(),p[1]);
                    }
                }
                else if(a=="-d")
                {
                    json = text[++i];
                }
            }

            //解析url
            if (url.StartsWith('\"'))
                url = url.Substring(1, url.Length - 2);
            var urlKey = "tt2.gamehivegames.com";
            int keyIndex = url.IndexOf(urlKey);
            if (keyIndex >= 0)
            {
                url = url.Substring(keyIndex + urlKey.Length);
            }
            var urlHead = url.Substring(0,url.IndexOf('?'));
            var urlPram = url.Substring(url.IndexOf('?') + 1);
            var urlArgs =new Dictionary<string, string>();
            var urlPramArray = urlPram.Split('&');
            foreach (var pram in urlPramArray)
            {
                var p = pram.Split('=');
                
                urlArgs.Add(p[0],p[1]);
            }

            if (json.StartsWith("\'") || json.StartsWith("\""))
                json= json.Substring(1, json.Length - 2);
            _config.Version = urlArgs["v"];
            if (_config.key == null)
                _config.key = new Dictionary<TT2Post.TT2Fun, TT2Post.SendInfo>();
            switch (urlHead)
            {
                case "/raid/current":
                {
                    if (!_config.key.ContainsKey(TT2Post.TT2Fun.RaidCurrent))
                        _config.key.Add(TT2Post.TT2Fun.RaidCurrent,
                            new TT2Post.SendInfo("/raid/current", urlArgs["s"], json));
                    var config= _config.key[TT2Post.TT2Fun.RaidCurrent];
                    config.s = urlArgs["s"];
                    config.json = json;
                } break;
                case "/clan/forum/board":
                {
                    return "弃用，请使用AppToken和PlayerToken";
                    if (!json.Contains("last_message_id"))
                    {
                        return "没有last_message_id，无效请求";
                    }
                    if (!_config.key.ContainsKey(TT2Post.TT2Fun.Forum))
                        _config.key.Add(TT2Post.TT2Fun.Forum,
                            new TT2Post.SendInfo("/clan/forum/board", urlArgs["s"], json));
                    var config= _config.key[TT2Post.TT2Fun.Forum];
                    config.s = urlArgs["s"];
                    config.json = json;
                } break;
                case "/solo_raid/leaderboards":
                {
                    if (!_config.key.ContainsKey(TT2Post.TT2Fun.SoloRaid))
                        _config.key.Add(TT2Post.TT2Fun.SoloRaid,
                            new TT2Post.SendInfo("/solo_raid/leaderboards", urlArgs["s"], json));
                    var config= _config.key[TT2Post.TT2Fun.SoloRaid];
                    config.s = urlArgs["s"];
                    config.json = json;
                } break;
                default:
                    return "不支持的请求curl" + urlHead;
            }

            _config.SessionId = content["X-TT2-session-id".ToLower()];
            _config.vendor = content["X-Tt2-Vendor-Id".ToLower()];
            _config.token = content["Authorization".ToLower()].Split(' ')[1];
            _config.stage = int.Parse(content["X-Tt2-Current-Stage".ToLower()]);
            _config.SupplyQQ = data.Sender;
            Save("config.json",_config.Save());
            
            return "更新成功"+urlHead+"\n你可以撤回了";

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await OutException(e);
            return "解析失败";
        }
    }

    private async Task<SoraMessage> ShowSoloRaid(GroupMsgData data, string other)
    {
        if (!_config.key.ContainsKey(TT2Post.TT2Fun.SoloRaid))
            return "未配置/solo_raid/leaderboards";
        await SendGroupMsg("获取中...");
        var all = await RefreshSoloRaid();
        if(all==null)
            return "获取失败";
        List<SoloRaidData> list=null;
        if (string.IsNullOrEmpty(other))
        {
            list = all.clan;
        }else if (other == "全球")
        {
            list = all.global;
        }
        if (list == null || list.Count == 0)
            return "没有数据";
        list.Sort((x,y)=>x.rank-y.rank);
        Dictionary<string, string> res = new Dictionary<string, string>();
        res.Add("名次-名字","时间\t\t层");
        foreach (var play in list)
        {
            int t = play.completion_time_in_seconds;
            string time = $"{(t / 3600):00}:{(t % 3600 / 60):00}:{(t % 60):00}";
            res.Add($"{play.rank}  {play.display_name}",$"{time}\t{play.world_id}-{play.floors_completed}");
        }

        var f=GetModelDir() + "ShowSoloRaid.png";
        ClubTool.DrawInfo(res,f,300);
        return Tool.Image(f);
        await Task.CompletedTask;
    }

    private async Task<SoraMessage> InputPlayerData(GroupMsgData data, string other)
    {
        // if (string.IsNullOrEmpty(other))
        // {
        //     return "需要解析数据，发送解析数据需要带此命令开头\nwww.crecheng.top/PlayOutput.html";
        // }
        // var pair= other.Split("/");
        // Dictionary<string, string> dic = new Dictionary<string, string>();
        // foreach (var s in pair)
        // {
        //     if(s.Length<=1)
        //         continue;
        //     var ss = s.Split('-');
        //     dic.Add(ss[0], ss[1]);
        // }
        //
        // int raid = int.Parse(dic["RaidLevel"]);
        // int totle = int.Parse(dic["TotalRaidCardLevels"]);
        // int skill = int.Parse(dic["SkillPointsOwned"]);
        // int scroll = int.Parse(dic["HeroScrollUpgrades"]);
        // int weapon = int.Parse(dic["HeroWeaponUpgrades"]);
        // int pet = int.Parse(dic["TotalPetLevels"]);
        // PlayerData playerData = null;
        // foreach (var (key, value) in _data.Player)
        // {
        //     if (value.SourceData == null)
        //     {
        //         continue;
        //     }
        //
        //     if (value.SourceData.player_raid_level == raid)
        //     {
        //         if (value.SourceData.total_card_level==totle
        //             && value.SourceData.total_skill_points==skill
        //             && value.SourceData.total_helper_weapons==weapon
        //             && value.SourceData.total_helper_scrolls==scroll
        //             && value.SourceData.total_pet_levels==pet
        //             )
        //         {
        //             playerData = value;
        //         }
        //     }
        // }
        //
        // if (playerData == null)
        // {
        //     string ret = "没有找到对应玩家";
        //     if (!_club.HaveRaid)
        //         ret += "\n当前没有突袭，请在打突袭时导入";
        //     return ret;
        // }
        //
        // int t = 0;
        // foreach (var (key,value) in ClubTool.CardEName)
        // {
        //     if (dic.ContainsKey(key))
        //     {
        //         t+=Int32.Parse(dic[key]);
        //     }
        //     else
        //     {
        //         Console.WriteLine(key);
        //     }
        // }
        //
        // if (t != totle)
        //     return "请不要自77人";
        // foreach (var (key,value) in ClubTool.CardEName)
        // {
        //     if (dic.ContainsKey(key))
        //     {
        //         if (playerData.Card.ContainsKey(value))
        //             playerData.Card[value]=Int32.Parse(dic[key]);
        //         else
        //             playerData.Card.Add(value, Int32.Parse(dic[key]));
        //     }
        // }
        //
        // return "更新成功，建议撤回";

        return SoraMessage.Null;
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

    private static List<float> LoyaltyLevel = new List<float>()
    {
        1, 1.1f, 1.14f, 1.16f, 1.18f, 1.2f, 1.22f, 1.24f, 1.26f, 1.28f, 1.3f, 1.32f, 1.34f, 1.34f
    };
    // private async Task<SoraMessage> RaidCardCal(GroupMsgData data, string other)
    // {
    //     if (!_data.QQLink.ContainsKey(data.Sender))
    //         return "没有你的卡片数据";
    //     if (string.IsNullOrEmpty(other))
    //         return "请正确输入,例如\n突袭模拟 胸 月光 风刃 风刃\n突袭模拟 胸肉 月光 灵魂 风刃";
    //
    //     if (other.StartsWith(' '))
    //         other = other.Substring(1);
    //     var args = other.Split(' ');
    //     if(args.Length<=1)
    //         return "请正确输入,例如\n突袭模拟 胸 月光 风刃 风刃\n突袭模拟 胸肉 月光 灵魂 风刃";
    //     int p = -1;
    //     int pa = -1;
    //     if (partCName.ContainsKey(args[0]))
    //         p = partCName[args[0]];
    //     
    //     if (!_club.HaveRaid && p != -1)
    //         return "当前没有突袭，请输入具体部位，如头蓝条，头白条";
    //     if (p == -1)
    //     {
    //         foreach (var (name,i) in partCName)
    //         {
    //             if (args[0].StartsWith(name))
    //                 p = i;
    //         }
    //
    //         if (args[0].EndsWith("蓝条"))
    //             pa = 0;
    //         else if (args[0].EndsWith("白条"))
    //             pa = 1;
    //         if (pa == -1 || p == -1)
    //             return "请输入正确部位，如头蓝条，头白条";
    //     }
    //
    //     List<CalPart> parts = new List<CalPart>();
    //     for (int i = 0; i < 8; i++)
    //     {
    //         parts.Add(null);
    //     }
    //     CalPart target = null;
    //     if (pa == -1 && _club.HaveRaid)
    //     {
    //         var titan= _club.GetCurrentTitanData();
    //         CalPart part = null;
    //         
    //         for (var j = 0; j < titan.parts.Count; j++)
    //         {
    //             var i = titan.parts[j];
    //             int pd = (int) i.part_id;
    //             int index = pd / 2;
    //             if (parts[index] == null)
    //                 parts[index] = new CalPart(i);
    //             else
    //                 parts[index].Add(i);
    //             if (index==p)
    //                 part = parts[index];
    //         }
    //
    //         if (part.CurrentHp > 0)
    //             target = part;
    //         else
    //             return "当前部分为骨架，请选其他部位\n或选择正确部位，如头蓝条，头白条";
    //     }
    //     
    //     var player = _data.Player[_data.QQLink[data.Sender]];
    //     string s = "突等："+player.RaidLevel+"\n";
    //     Dictionary<string, int> card = new Dictionary<string, int>();
    //     for (var i = 1; i < Math.Min(args.Length,4) ; i++)
    //     {
    //         var c = args[i];
    //         var id = ClubTool.NameToIDCard(c);
    //         if (string.IsNullOrEmpty(id))
    //             return "没找到对于简称，查看简称可以命令：卡名字";
    //
    //         if (!player.Card.ContainsKey(id))
    //             return "你还没有对应卡的数据，可以使用导入个人数据导入";
    //         if(card.ContainsKey(id))
    //             return SoraMessage.Null;
    //         card.Add(id,player.Card[id]);
    //         s += c + ":" + card[id]+"\n";
    //     }
    //     RaidCal cal = new RaidCal();
    //     if (target==null)
    //     {
    //         target = new CalPart(p, pa);
    //     }
    //     cal.TargetPart = target;
    //     RaidCal.RaidAdd add = new RaidCal.RaidAdd();
    //
    //     add.AllAdd *= LoyaltyLevel[player.SourceData.loyalty_level];
    //     double td = 0;
    //     if (_club.HaveRaid)
    //     {
    //         var raid = _club.clan_raid;
    //         if (raid.boost_bonus != null)
    //         {
    //             if (raid.boost_bonus.BonusType == "AllRaidDamage")
    //                 td += raid.boost_bonus.BonusAmount;
    //         }
    //
    //         if (raid.special_card_info != null && raid.special_card_info.Count > 0)
    //         {
    //             raid.special_card_info.ForEach(i =>
    //             {
    //                 if (i.BonusType == "TeamTacticsClanMoraleBoost")
    //                     td += i.BonusAmount;
    //             });
    //         }
    //
    //         add.AllAdd *= (float)(1 + td);
    //     }
    //     
    //     double all = 0;
    //     int count = 100;
    //     // double max = Double.MinValue;
    //     // double min =Double.MaxValue;
    //     // for (int i = 0; i < count; i++)
    //     // {
    //     //     var d = cal.Cal(card, player.RaidLevel, DataManage, parts, add);
    //     //     if (d > max)
    //     //         max = d;
    //     //     if (d < min)
    //     //         min = d;
    //     //     all += d;
    //     // }
    //     //
    //     // s += $"模拟{count}次\n最高:{max.ShowNum()}\n最低:{min.ShowNum()}\n平均:{(all / count).ShowNum()}";
    //     List<RaidDmgData> list = new List<RaidDmgData>();
    //     for (int i = 0; i < count; i++)
    //     {
    //         var d = cal.Cal(card, player.RaidLevel, DataManage, parts, add);
    //         list.Add(d);
    //     }
    //     
    //     var f = GetModelDir() + "RaidCardCal.png";
    //     RaidDmgDataTool.DrawDmgDataList(list,f,player.SourceData,(float)td);
    //     return Tool.Image(f);
    // }

    
    /// <summary>
    /// 清理不在的人
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> ClearMember(GroupMsgData data)
    {
        if (_config.CanUse())
        {
            if (!(data.IsAdmin|| data.IsGroupAdmin))
                return "你没权限！";
            if (_data == null || _club==null || !_club.HaveRaid)
                return "没有数据！";
            var bak = new Dictionary<string, PlayerData>(_data.Player);
            _data.Player.Clear();
            foreach (var (name, id) in _data.PlayerId)
            {
                _data.Player.Add(id,bak[id]);
            }
            SaveRaidData();
            return $"成功清理{bak.Count - _data.Player.Count}个成员数据";
        }
        else
        {
            return "还没配置文件";
        }
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// 重连api
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> ReStartClient(GroupMsgData data)
    {
        if (_config.CanUse())
        {
            if (!(data.IsAdmin|| data.IsGroupAdmin))
                return "你没权限！";
            await PostApi.Stop();
            await Task.Delay(10000);
            await PostApi.CheckReStart();
            
            return SoraMessage.Null;
        }
        else
        {
            return "还没配置文件";
        }
        await Task.CompletedTask;
    }

    #endregion
}