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

        public static void DrawPlayerCard(Dictionary<string,int> card, string path,string imgPath)
        {
            int len = card.Count / 7;
            Bitmap bitmap = new Bitmap(32 * 7, (len + 1) * 64);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            int i = 0;
            foreach (var (key, value) in card)
            {
                int x = i % 7;
                int y = i / 7;
                if (File.Exists(imgPath + key + ".png"))
                {
                    var png = Image.FromFile(imgPath + key + ".png");
                    g.DrawImage(png, new Point(x * 32, y * 64));
                }

                g.DrawString(value.ToString(), font, Brushes.Black, new Point(x * 32, y * 64 + 32));
                i++;
            }
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
                sb.Append(c[i]);
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
            keys.Sort((x,y)=>info[y][0].Data.RaidLevel-info[x][0].Data.RaidLevel);
            
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
                    g.DrawString(attack.Time.ToString("HH:mm:ss"), font, Brushes.Black, 280+c*(32*3+50), i * 96 + 10);
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
        
    }
    

}