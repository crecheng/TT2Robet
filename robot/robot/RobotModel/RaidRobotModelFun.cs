using System.Text;
using testrobot;

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
        ClubTool.DrawCardName(f,GetModelDir() + "Card-64\\");
        return Tool.Image(f);
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
            var f = GetModelDir() + "kkDmg.png";
            ClubTool.DrawPlayerRaidDmg(_club, f);
            return Tool.Image(f);
        }

        await Task.CompletedTask;
    }

    private async Task<SoraMessage> GetCmd(GroupMsgData data)
    {
        string s = String.Empty;
        foreach (var key in _argFun.Keys)
        {
            s += key + "\n";
        }

        foreach (var key in _argLenFun.Keys)
        {
            s += key + "{参数}\n";
        }

        return s;
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
            return _data.Player[_data.QQLink[data.Sender]].EasyInfo();
        }

        return "你还没绑定";
    }

    /// <summary>
    /// 我的卡片
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> GetMyCard(GroupMsgData data)
    {
        if (_data.QQLink.ContainsKey(data.Sender))
        {
            var p = _data.Player[_data.QQLink[data.Sender]];
            if (p.Card.Count == 0)
                return "你还没卡片数据";
            var f = GetModelDir() + p.Code + "-card.png";
            ClubTool.DrawPlayerCard(p.Name, p.Card, f, GetModelDir() + "Card-32\\");
            return Tool.Image(f);
        }

        return "你还没绑定";
    }

    private async Task<SoraMessage> TipNotShareSwitch(GroupMsgData data)
    {
        if (data.IsAdmin|| data.IsGroupAdmin)
        {
            _data.TipNotShare = !_data.TipNotShare;
            return $"分享警告-{_data.TipNotShare}";
        }

        return "你没权限！";

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
        ClubTool.DrawAllPlayerCard(dic, f, GetModelDir() + "Card-32\\");
        return Tool.Image(f);
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
                sb.Append($"{i.Key},{i.Value},");
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

        return SoraMessage.Null;
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
        foreach (var (key, value) in res)
        {
            dic.Add(_data.Player[key].Name,value.ToString());
        }
        var f=GetModelDir() + "GetWhoUesCard.png";
        ClubTool.DrawInfo(dic,f);
        return Tool.Image(f);
    }

    #endregion
}