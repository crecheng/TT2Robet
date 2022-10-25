using System.Collections.Generic;

namespace testrobot
{
    public class MsgDmgData
    {
        public int Index;
        public double AllDmg;
        public int TapCount;
        public double TapDmg;
        public int Raid;
        public int RaidS;
        public string name;
        public int RaidLevel;
        public string club;
        public string PlayerHead;
        public string None;
        public List<CardBoard> cards;

        public static MsgDmgData DeserializeObject(string s,string player="")
        {
            if (string.IsNullOrEmpty(s))
                return null;
            string[] m = s.Split(',');
            var data=new MsgDmgData();
            int i = 0;
            if (m.Length > 11)
            {
                int.TryParse(m[i++], out data.Index);
                double.TryParse(m[i++], out data.AllDmg);
                int.TryParse(m[i++], out data.TapCount);
                double.TryParse(m[i++], out data.TapDmg);
                int.TryParse(m[i++], out data.Raid);
                int.TryParse(m[i++], out data.RaidS);
                data.name = m[i++];
                int.TryParse(m[i++], out data.RaidLevel);
                data.club = m[i++];
                data.PlayerHead = m[i++];
                data.None = m[i++];
            }
            data.cards=new List<CardBoard>();

            for (; i < m.Length; i++)
            {
                var c = CardBoard.DeserializeObject(m[i]);
                if (c != null)
                {
                    c.player = player;
                    data.cards.Add(c);
                }
            }

            return data;
        }
        
        public class CardBoard
        {
            public string type;
            public double Dmg;
            public int Level;
            public string player;

            public static CardBoard DeserializeObject(string s)
            {
                if (string.IsNullOrEmpty(s))
                    return null;
                string[] m = s.Split(':');
                var d=new CardBoard();
                if (m.Length > 2)
                {
                    d.type = m[0];
                    double.TryParse(m[1], out d.Dmg);
                    int.TryParse(m[2], out d.Level);
                    return d;
                }

                return null;
            }

            public override string ToString()
            {
                return $"{nameof(player)}: {player}, {nameof(type)}: {type}, {nameof(Level)}: {Level}";
            }
        }
    }
}