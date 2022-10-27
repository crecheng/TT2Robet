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
                return "你还没开片数据";
            var f = GetModelDir() + p.Code + "-card.png";
            ClubTool.DrawPlayerCard(p.Card, f, GetModelDir() + "Card-32\\");
            return Tool.Image(f);
        }

        return "你还没绑定";
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
        await UploadGroupFile("allCard.csv", "allCard.csv");
        return SoraMessage.Null;
    }

    /// <summary>
    /// 本轮攻击
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<SoraMessage> GetNearAtkInfoThisRound(GroupMsgData data)
    {
        var last = _club.NextAttackTime - new TimeSpan(20, 0, 0);
        var f = GetNearAtkInfo(last, DateTime.Now - new TimeSpan(8, 0, 0));
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


    #endregion

    #region 带参数的命令

    private async Task<SoraMessage> GetNearAtkInfo(GroupMsgData data, string arg)
    {
        if (_club == null)
            return "还没数据";
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

            var p = _data.Player[c];
            _data.QQLink[sender] = c;
            return $"绑定成功-{p.Code}-{p.Name}";
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

    #endregion
}