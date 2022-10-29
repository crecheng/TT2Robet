using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace testrobot
{
    [SuppressMessage("Interoperability", "CA1416:验证平台兼容性")]
    public static class ClubTool
    {

        public static List<int> CardCost = new List<int>()
        {
            0,
            20, 125, 280, 480, 760,
            1100, 1660, 2350, 3150, 4250,
            5600, 7250, 9150, 11950, 15600,
            19500, 23585, 27775, 32050, 36700,
            41500, 46700, 52050, 57470, 62980,
            68580, 74270, 80100, 86945, 93850,
            100820, 107850, 114940, 122095, 129315,
            136595, 143940, 151405, 159490, 167705,
            175980, 184320, 194820, 205620, 216570,
            227670, 239070, 250620, 262320, 274320,
            286620, 299220, 312120, 325320, 338820,
            352620, 366720, 381120, 395820, 410820,
        };

        public static Dictionary<string, string> CardName = new Dictionary<string, string>()
        {
            {"WhipOfLightning","电鞭"},
            {"FinisherAttack","凯旋"},
            {"TotemFairySkill","仙女"},
            {"PlagueAttack","瘟疫"},
            {"TeamTactics","团队"},
            {"InnerTruth","灵魂"},
            {"SkullBash","头骨"},
            {"RazorWind","风刃"},
            {"Swarm","蜂巢"},
            {"MentalFocus","气体"},
            {"Disease","放射"},
            {"Purify","净化"},
            {"LimbBurst","熊掌"},
            {"SuperheatMetal","菱形"},
            {"MoonBeam","月光"},
            {"RuneAttack","蓝球"},
            {"ImpactAttack","振奋"},
            {"CrushingVoid","紫球"},
            {"Shadow","影子"},
            {"LimbSupport","藤蔓"},
            {"Fuse","核融"},
            {"Fragmentize","破甲"},
            {"FlakShot","防空"},
            {"DecayingAttack","腐败"},
            {"ExecutionersAxe","锤子"},
            {"Haymaker","激光"},
            {"BurstCount","导弹"},
            {"ChainLightning","红链"},
            {"BurningAttack","火海"},
            {"BurstBoost","先祖"},
            {"PoisonAttack","酸雨"},
            {"SpinalTap","骨骼"},
            {"RuinousRust","紫雨"},
        };

        private static Dictionary<string, string> _cardNName;


        public static string NameToIDCard(string name)
        {
            if (_cardNName == null)
            {
                _cardNName = new Dictionary<string, string>();
                foreach (var (key, value) in CardName)
                {
                    _cardNName.Add(value,key);
                }
            }

            if (_cardNName.ContainsKey(name))
                return _cardNName[name];
            return string.Empty;
        }
        public static string ShowAll<T>(this List<T> list,string separator=", ",string head="",Func<T,string> action=null)
        {
            if (list == null || list.Count <= 1)
                return "";
            StringBuilder s = new StringBuilder();
            int c = 0;
            list.ForEach((i) =>
            {
                string tmp = i.ToString();
                if (action != null)
                    tmp = action.Invoke(i);
                s.Append(head).Append(tmp);
                if (c++ < list.Count)
                    s.Append(separator);
            });
            return s.ToString();
        }
        
        public static string ShowAll<T,TE>(this Dictionary<T,TE> dic,string separator=", ",string head="",string m=": ")
        {
            if (dic == null || dic.Count <= 1)
                return "";
            StringBuilder s = new StringBuilder();
            int c = 0;
            foreach (var d in dic)
            {
                s.Append(head).Append(d.Key.ToString()).Append(m)
                    .Append(d.Value.ToString());
                if (c++ < dic.Count)
                    s.Append(separator);
            }
            return s.ToString();
        }

        public static string ShowNum(this double value)
        {
            char[] c = {' ','K', 'M', 'B', 'T'};
            var tmp = value;
            int i = 0;
            while (Math.Abs(tmp)>1000.0&&i<2)
            {
                tmp /= 1000.0;
                i++;
            }
            return tmp.ToString("F") + c[i];
        }
        
        public static string SubstringIncludeChinese(this string originalText, int bytesAfterCut)
        {
            string optimizedText = originalText;
            byte[] val = Encoding.Default.GetBytes(originalText);
            if (val.Length > bytesAfterCut)
            {
                int left = bytesAfterCut / 2;
                int right = bytesAfterCut;
                left = left > originalText.Length ? originalText.Length : left;
                right = right > originalText.Length ? originalText.Length : right;
                while (left < right - 1)
                {
                    int mid = (left + right) / 2;
                    if (Encoding.Default.GetBytes(originalText.Substring(0, mid)).Length > 
                        bytesAfterCut)
                    {
                        right = mid;
                    }
                    else
                    {
                        left = mid;
                    }
                }
                byte[] rightVal = Encoding.Default.GetBytes(originalText.Substring(0, right));
                if (rightVal.Length == bytesAfterCut)
                {
                    optimizedText = originalText.Substring(0, right);
                }
                else
                {
                    optimizedText = originalText.Substring(0, left);
                }
            }
            return optimizedText;
        }
        
        public static DateTime Parse(string s)
        {
            return DateTime.TryParseExact(s,"yyyy-MM-dd HH:mm:ss",null,DateTimeStyles.None, out DateTime time) ? time : default;
        }
        
        public static void DrawPlayerRaidDmg(ClubData club, string path)
        {
            var Players = club.clan_raid.leaderboard;
            int MaxCount = 30;
            int height = 25;
            int width = 400;
            Rectangle rect=new Rectangle(width/2,20,width,height);
            Bitmap bitmap = new Bitmap(width+20*2, 20*2+Players.Count*(height+5));
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            
            Pen pen = new Pen(Color.Aqua);
            Brush brush1 = new SolidBrush(Color.Aqua);
            Brush greenBrush = new SolidBrush(Color.Green);
            Brush whiteBrush = new SolidBrush(Color.White);
            Players.Sort((x,y)=>
            {
                if (y.num_attacks != x.num_attacks)
                {
                    return x.num_attacks - y.num_attacks;
                }
                else
                {
                    return (int) (y.score - x.score);
                }
                
            });
            
            double needAverage = club.titan_lords.GetNeedAllDmg()/Players.Count/MaxCount;


            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                var y = rect.Y + i * (height + 5);
                double average = p.score/p.num_attacks;
                double dec = average - needAverage;
                Rectangle fill=new Rectangle();
                fill.Y = y;
                fill.Height = rect.Height;
                fill.Width=(int)(dec / (needAverage * 2)*rect.Width);
                if (dec > 0)
                {
                    fill.X = rect.X;
                }
                else
                {
                    fill.Width = -fill.Width;
                    fill.X = rect.X - fill.Width;
                }

                g.FillRectangle(brush1,fill);
                g.DrawString(Regex.Unescape(p.name),font,greenBrush,30,y+2);
                g.DrawString(dec.ShowNum(),font,greenBrush,280,y+2);
                g.DrawString(p.num_attacks.ToString(),font,Brushes.HotPink, 380,y+2);
            }
            bitmap.Save(path);
        }

        public static void DrawPlayerCard(string player, Dictionary<string,int> card, string path,string imgPath)
        {
            int rowCount = 10;
            int len = (card.Count-1) / rowCount+1;
            Bitmap bitmap = new Bitmap(32 * rowCount, (len + 1) * 64);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            Font sFont = new Font(FontFamily.GenericMonospace, 12f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            int i = rowCount;
            List<string> name = new List<string>(card.Keys);
            name.Sort((x, y) => card[y] - card[x]);
            int all = 0;
            int cost = 0;
            foreach (var key in name)
            {
                int x = i % rowCount;
                int y = i / rowCount;
                if (File.Exists(imgPath + key + ".png"))
                {
                    var png = Image.FromFile(imgPath + key + ".png");
                    g.DrawImage(png, new Point(x * 32, y * 64));
                }

                int level = card[key];
                all += level;
                cost += CardCost[level];
                g.DrawString(card[key].ToString(), font, Brushes.Black, new Point(x * 32, y * 64 + 32));
                i++;
            }
            
            g.FillRectangle(Brushes.Bisque, 0,32,bitmap.Width,32);
            g.DrawString(player.ToString(), font, Brushes.Black, 10, 10);
            g.DrawString(all.ToString(), sFont, Brushes.Brown, 10, 40);
            g.DrawString(cost.ToString("N0"), sFont, Brushes.Indigo, 100, 40);
            
            bitmap.Save(path);
        }

        public static string GetAllCardString(Dictionary<string, Dictionary<string, int>> cards)
        {
            HashSet<string> cardName = new HashSet<string>();
            foreach (var (key, value) in cards)
            {
                foreach (var (item, level) in value)
                {
                    cardName.Add(item);
                }
            }

            List<string> c = new List<string>(cardName);
            StringBuilder sb = new StringBuilder();
            sb.Append("name").Append(",");
            for (var i = 0; i < c.Count; i++)
            {
                sb.Append(CardName[c[i]]).Append('-').Append(c[i]);
                if (i != c.Count - 1)
                    sb.Append(",");
            }
            
            foreach (var (key, value) in cards)
            {
                sb.Append("\n");
                sb.Append(key).Append(",");
                for (var i = 0; i < c.Count; i++)
                {
                    var name = c[i];
                    if (value.ContainsKey(name))
                        sb.Append(value[name]);
                    else
                        sb.Append(0);
                    if (i != c.Count - 1)
                        sb.Append(",");
                }
            }

            return sb.ToString();
        }
        public static void DrawAllPlayerCard(Dictionary<string, Dictionary<string, int>> cards, string path, string imgPath)
        {
            int max = 0;
            List<string> player = new List<string>();
            Dictionary<string, int> num = new Dictionary<string, int>();
            HashSet<string> cardName = new HashSet<string>();
            foreach (var (key, value) in cards)
            {
                player.Add(key);
                if (value.Count > max)
                    max = value.Count;
                int c = 0;
                foreach (var (item, level) in value)
                {
                    cardName.Add(item);
                    c += level;
                }
                num.Add(key,c);
            }

            player.Sort((x, y) => num[y] - num[x]);
            Bitmap bitmap = new Bitmap(280 + 32 * cardName.Count+50, cards.Count * 64);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            for (var i = 0; i < player.Count; i++)
            {
                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Cornsilk, 0,i * 64,bitmap.Width,64);
                }
                var name = player[i];
                g.DrawString(Regex.Unescape(name), font, Brushes.Black, 30, i * 64 + 10);
                int c = 0;
                foreach (var card in cardName)
                {
                    if (cards[name].ContainsKey(card))
                    {
                        if (File.Exists(imgPath + card + ".png"))
                        {
                            var png = Image.FromFile(imgPath + card + ".png");
                            g.DrawImage(png, new Point(280+c * 32, i * 64));
                        }

                        g.DrawString(cards[name][card].ToString(), font, Brushes.Black,
                            new Point(280 + c * 32, i * 64 + 32));
                    }
                    c++;
                }
            }
            bitmap.Save(path);
        }

        public static void DrawCardName(string path, string imgPath)
        {
            int rowCount = 10;
            int row = (CardName.Count - 1) / rowCount+1;
            Bitmap bitmap = new Bitmap(64*rowCount, row * 96);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            int i = 0;
            foreach (var k in CardName)
            {
                int x = i % rowCount;
                int y = i / rowCount;
                var id = k.Key;
                var name = k.Value;
                if (File.Exists(imgPath + id + ".png"))
                {
                    var png = Image.FromFile(imgPath + id + ".png");
                    g.DrawImage(png, new Point(x * 64, y * 96));
                }

                g.DrawString(name, font, Brushes.Black, x * 64 + 5, y * 96 + 70);
                i++;
            }
            bitmap.Save(path);
            
        }
        
        public static void DrawAtkInfo(Dictionary<string, List<AttackShareInfo>> info, string path, string imgPath)
        {
            int max = 0;
            foreach (var (key, value) in info)
            {
                if (value.Count > max)
                    max = value.Count;
            }
            
            Bitmap bitmap = new Bitmap(280 + (32 *3+50)* max+50, info.Count * 96);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);

            List<string> keys = new List<string>(info.Keys);
            keys.Sort((x,y)=>
            {
                var yy = info[y].Count > 0 ? info[y][0].Data.RaidLevel : 0;
                var xx = info[x].Count > 0 ? info[x][0].Data.RaidLevel : 0;
                return yy - xx;
            });
            
            int i = 0;
            foreach (var k in keys)
            {
                var data = info[k];
                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Cornsilk, 0,i * 96,bitmap.Width,96);
                }
                var name = k;
                g.DrawString(Regex.Unescape(name), font, Brushes.Black, 30, i * 96 + 32);
                int c = 0;
                foreach (var attack in data)
                {
                    g.DrawString((attack.Time+new TimeSpan(8,0,0)).ToString("HH:mm:ss"), font, Brushes.Black, 280+c*(32*3+50), i * 96 + 10);
                    int d = 0;
                    foreach (var (card, value) in attack.Data.Card)
                    {
                        if (File.Exists(imgPath + card + ".png"))
                        {
                            var png = Image.FromFile(imgPath + card + ".png");
                            g.DrawImage(png, new Point(280 + c * (32*3+50) + d * 32, i * 96 + 32));
                        }

                        d++;
                    }
                    g.DrawString(attack.Data.Dmg.ShowNum(), font, Brushes.Black, 280+c*(32*3+50), i * 96 + 70);
                    c++;
                }

                i++;
            }
            bitmap.Save(path);
        }
        
        public static void DrawInfo(Dictionary<string, string> dic, string path,int len=100)
        {
            
            Bitmap bitmap = new Bitmap(280 + len, 20+dic.Count*32);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            int i = 0;
            foreach (var (key, value) in dic)
            {
                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Cornsilk, 0,i * 32,bitmap.Width,32);
                }

                g.DrawString(key, font, Brushes.Black, 10, i * 32 + 3);
                g.DrawString(value, font, Brushes.Brown, 290, i * 32 + 3);

                i++;
            }
            bitmap.Save(path);
        }
    }
    

}