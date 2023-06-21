using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using robot.SocketTool;

namespace testrobot
{
    public class AttackShareInfo
    {
        public string Player;
        public string PlayerName;
        public DmgData Data;
        public AttackLog Log;
        public double Tap;
        public int index;

        public DateTime GetTime()=>Log == null ? new DateTime(1970, 1, 1) : Log.attackTime;

        public Dictionary<string, string> GetDmgInfo()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(PlayerName,"");
            dic.Add("突等",Data.RaidLevel.ToString());
            foreach (var cardInfo in Log.cards_level)
            {
                if(ClubTool.CardName.TryGetValue(cardInfo.id, out var value))
                    dic.Add(value,cardInfo.value.ToString());
                else
                    dic.Add(cardInfo.id,cardInfo.value.ToString());
            }
            dic.Add("泰坦",index.ToString());
            Dictionary<string, double> parts = new Dictionary<string, double>();
            foreach (var damageInfo in Log.cards_damage)
            {
                foreach (var dmg in damageInfo.damage_log)
                {
                    if (parts.ContainsKey(dmg.id))
                        parts[dmg.id] += dmg.value;
                    else
                        parts[dmg.id] = dmg.value;
                }
            }
            foreach (var part in parts)
            {
                if (Enum.TryParse(part.Key, out TitanData.PartName p))
                {
                    dic.Add(TitanData.PartNameShow(p),part.Value.ShowNum());
                }
                else
                {
                    dic.Add(part.Key,part.Value.ShowNum());
                }
            }

            return dic;
        }

        public Dictionary<TitanData.PartName, double> GetPartDmg()
        {
            Dictionary<TitanData.PartName, double> parts = new Dictionary<TitanData.PartName, double>();
            foreach (var damageInfo in Log.cards_damage)
            {
                foreach (var dmg in damageInfo.damage_log)
                {
                    if (Enum.TryParse(dmg.id, out TitanData.PartName p))
                    {
                        if (parts.ContainsKey(p))
                            parts[p] += dmg.value;
                        else
                            parts[p] = dmg.value;
                    }
                }
            }

            return parts;
        }

        public void CalTap()
        {
            foreach (var damageInfo in Log.cards_damage)
            {
                bool addTap = string.IsNullOrEmpty(damageInfo.id);
                
                foreach (var dmg in damageInfo.damage_log)
                {
                    Data.Dmg += dmg.value;
                    if (addTap)
                        Tap += dmg.value;
                }
            }
        }

        public class DmgData
        {
            public Dictionary<string, int> Card=new Dictionary<string, int>();
            public double Dmg;
            public int RaidLevel;
        }

        public void Init(AttackAPIInfo info)
        {
            Log = info.attack_log;
            Log.attackTime=DateTime.Parse(Log.attack_datetime);
            Player = info.player.player_code;
            PlayerName = Regex.Unescape(info.player.name);
            Data = new DmgData();
            Data.RaidLevel = info.player.raid_level;
            index = info.raid_state.titan_index;
            
            foreach (var cardInfo in info.attack_log.cards_level)
            {
                Data.Card[cardInfo.id] = cardInfo.value;
            }
            CalTap();
        }

        // public static DmgData GetDmg(string data)
        // {
        //     DmgData dmg = new DmgData();
        //     //4,6070668.21232824,600,1300985.8512,4,33,nam,295,Clan,AvatarVI,FrameDungeo,TotemFairy:2.53333:16:0,Purify:0:14:0,PoisonAttack:153670.856681233:15:0
        //     //4,3291288.50046542,599,269849.45208,3,11,cre,212,Clan,Avatar11,FrameAbyss2,BurstCount:1770192:14,BurstBoost:0:13,RazorWind:1251246.33221806:11
        //     var s = data.Split(',');
        //     double.TryParse(s[1], out dmg.Dmg);
        //     int.TryParse(s[7], out dmg.RaidLevel);
        //     if (s.Length >= 12)
        //     {
        //         dmg.Card = new Dictionary<string, int>();
        //         var c = s[11].Split(':');
        //         int.TryParse(c[2], out int num1);
        //         dmg.Card.Add(c[0],num1);
        //         if (s.Length >= 13)
        //         {
        //              c = s[12].Split(':');
        //             int.TryParse(c[2], out num1);
        //             dmg.Card.Add(c[0],num1);
        //         }
        //         
        //         if (s.Length >= 14)
        //         {
        //             c = s[13].Split(':');
        //             int.TryParse(c[2], out num1);
        //             dmg.Card.Add(c[0],num1);
        //         }
        //     }
        //
        //     return dmg;
        // }


    }
}