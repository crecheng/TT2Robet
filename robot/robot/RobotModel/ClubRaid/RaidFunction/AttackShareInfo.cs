using System.Collections.Generic;

namespace testrobot
{
    public class AttackShareInfo
    {
        public int Id;
        public string Player;
        public string PlayerName;
        public DmgData Data;


        public static List<AttackShareInfo> GetAllAttackShareInfo(MsgDataList dataList)
        {
            var list = new List<AttackShareInfo>();
            foreach (var msg in dataList.messages)
            {
                if (msg.message_type == "RaidAttackSummaryShare")
                {
                    var d = new AttackShareInfo();
                    d.Id = msg.id;
                    d.Player = msg.player_from;
                    d.PlayerName = msg.name;
                    d.Data = GetDmg(msg.message);
                    list.Add(d);
                }
            }

            return list;
        }
        
        public class DmgData
        {
            public Dictionary<string, int> Card;
            public double Dmg;
            public int RaidLevel;
        }

        public static DmgData GetDmg(string data)
        {
            DmgData dmg = new DmgData();
            //4,3291288.50046542,599,269849.45208,3,11,cre,212,Clan,Avatar11,FrameAbyss2,BurstCount:1770192.71616738:14,BurstBoost:0:13,RazorWind:1251246.33221806:11
            var s = data.Split(',');
            double.TryParse(s[1], out dmg.Dmg);
            int.TryParse(s[7], out dmg.RaidLevel);
            if (s.Length >= 12)
            {
                dmg.Card = new Dictionary<string, int>();
                var c = s[11].Split(':');
                int.TryParse(c[2], out int num1);
                dmg.Card.Add(c[0],num1);
                if (s.Length >= 13)
                {
                     c = s[12].Split(':');
                    int.TryParse(c[2], out num1);
                    dmg.Card.Add(c[0],num1);
                }
                
                if (s.Length >= 14)
                {
                    c = s[13].Split(':');
                    int.TryParse(c[2], out num1);
                    dmg.Card.Add(c[0],num1);
                }
            }

            return dmg;
        }


    }
}