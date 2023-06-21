using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using robot.RobotModel;
using robot.SocketTool;

// ReSharper disable All
#pragma warning disable CS8600

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

        private static Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();

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
            {"CelestialStatic","天体"},
            {"MirrorForce","镜像"},
            {"AstralEcho","回声"},
            {"PowerBubble","气泡"},
        };
        
        public static Dictionary<string, string> CardEName = new Dictionary<string, string>()
        {
            {"WhipofLightning","WhipOfLightning"},
            {"VictoryMarch","FinisherAttack"},
            {"TotemofPower","TotemFairySkill"},
            {"ThrivingPlague","PlagueAttack"},
            {"TeamTactics","TeamTactics"},
            {"SoulFire","InnerTruth"},
            {"SkullBash","SkullBash"},
            {"RazorWind","RazorWind"},
            {"RavenousSwarm","Swarm"},
            {"RancidGas","MentalFocus"},
            {"Radioactivity","Disease"},
            {"PurifyingBlast","Purify"},
            {"PsychicShackles","LimbBurst"},
            {"PrismaticRift","SuperheatMetal"},
            {"MoonBeam","MoonBeam"},
            {"Maelstrom","RuneAttack"},
            {"InspiringForce","ImpactAttack"},
            {"InsanityVoid","CrushingVoid"},
            {"GrimShadow","Shadow"},
            {"GraspingVines","LimbSupport"},
            {"FusionBomb","Fuse"},
            {"Fragmentize","Fragmentize"},
            {"FlakShot","FlakShot"},
            {"DecayingStrike","DecayingAttack"},
            {"CrushingInstinct","ExecutionersAxe"},
            {"CosmicHaymaker","Haymaker"},
            {"ClanshipBarrage","BurstCount"},
            {"ChainofVengeance","ChainLightning"},
            {"BlazingInferno","BurningAttack"},
            {"AncestralFavor","BurstBoost"},
            {"AcidDrench","PoisonAttack"},
            {"SkeletalSmash","SpinalTap"},
            {"RuinousRain","RuinousRust"},
        };

        public static Dictionary<string, string> CardDmageType = new Dictionary<string, string>()
        {
            { "AfflictedChanceSupport", "毒系几率加成" },
            { "AfflictedDamageSupport", "毒系伤害加成" },
            { "AllRaidDamageSupport", "全部伤害加成" },
            { "ArmorDamageSupport", "白条伤害加成" },
            { "BodyDamageSupport", "蓝条伤害加成" },
            { "BurningDamageRate", "火海伤害" },
            { "BurningPartChance", "触发火海提供的毒系几率加成" },
            { "BurstChanceSupport", "爆发几率加成" },
            { "BurstCountBonus", "爆发次数奖励" },
            { "BurstCountDamage", "爆发次数伤害" },
            { "BurstDamageSupport", "爆发伤害加成" },
            { "CelestialStaticBurst", "天体静态爆发" },
            { "CelestialStaticChargePerBurst", "天体静态爆发时消耗" },
            { "CelestialStaticChargePerTap", "天体静态每次点击时加分" },
            { "CelestialStaticMaxCharges", "天体静态最大积分数" },
            { "ChainLightningBurst", "红链伤害" },
            { "ChainLightningMaxTargets", "红链最高支持部位" },
            { "ChestDamageSupport", "胸部伤害加成" },
            { "DecayDamageExpo", "腐败伤害" },
            { "DecayHealthCap", "腐败血量限制" },
            { "DecayPercentRate", "腐败百分比增伤" },
            { "DiseaseBonusRate", "放射性随时间提高的伤害" },
            { "DiseaseDamageRate", "放射性伤害" },
            { "ExposedBodyBoostSupport", "暴露的蓝条数量加成" },
            { "ExposedBodyBoostSupportMaxParts", "暴露的蓝条数量加成上限" },
            { "ExposedSkeletonBoostSupport", "暴露的骨架数量加成" },
            { "ExposedSkeletonBoostSupportMaxParts", "暴露的骨架数量加成上限" },
            { "FairyTotemDamage", "仙女加成" },
            { "FairyTotemDuration", "仙女持续时间" },
            { "FairyTotemRandomChance", "仙女出现几率" },
            { "FairyTotemRate", "仙女飞行速度" },
            { "FlakBurst", "防空伤害" },
            { "FragmentizeArmorMult", "破甲对白条额外加成" },
            { "FragmentizeBurst", "破甲伤害" },
            { "FragmentizeEnchantedArmorMult", "破甲对诅咒额外加成" },
            { "FuseExplosionMult", "核融触发的等待时间" },
            { "FuseRepeatChance", "核融触发几率" },
            { "HaymakerBurst", "激光伤害" },
            { "HaymakerTapsNeeded", "每次触发需要的点击数" },
            { "HeadDamageSupport", "头部伤害加成" },
            { "LimbBurstDamage", "四肢爆发伤害" },
            { "LimbBurstMult", "四肢部位额外加成数" },
            { "LimbDamageSupport", "四肢伤害加成" },
            { "MirrorForceBoost", "镜像的队友加成" },
            { "MirrorForceBoostMax", "镜像队友加成上限" },
            { "MirrorForceBurst", "镜像伤害" },
            { "MoonBeamBurst", "月光伤害" },
            { "MoonBeamChestMult", "月光对胸部的额外加成" },
            { "PlagueAttackDamageMult", "瘟疫额外毒系部位的伤害加成" },
            { "PlagueAttackDamageRate", "瘟疫基础伤害" },
            { "PoisonDamageRate", "酸雨伤害" },
            { "PurifyBonus", "净化清毒加成" },
            { "PurifyDamage", "净化伤害" },
            { "RazorWindBodyMult", "风刃蓝条额外加成" },
            { "RazorWindBurst", "风刃伤害" },
            { "RicochetDamageMult", "防空蓝条加成（疑似）" },
            { "RuinousRustDamageRate", "紫雨伤害" },
            { "RuinousRustEnchantedArmorMult", "紫雨对诅咒时的额外加成" },
            { "RuneDamageBoost", "蓝球加成" },
            { "RuneDamageRate", "蓝球伤害" },
            { "ShadowDamageMult", "影子叠加加成" },
            { "ShadowDamageRate", "影子伤害" },
            { "SkeletonExposedArmorDamageSupport", "骨骼对二段加成" },
            { "SkullBashBurst", "头骨伤害" },
            { "SkullBashHeadMult", "头骨对头部额外加成" },
            { "SwarmDamageRate", "蜂巢伤害" },
            { "TeamTacticsClanMoraleBoost", "团队队友加成" },
            { "WhipOfLightningBurst", "电鞭伤害" },
            { "WhipOfLightningChance", "电鞭触发几率" },
            { "WhipOfLightningChanceCap", "电鞭触发几率上限" },
        };

        public static Image GetImage(string path, bool useCache = true)
        {
            if(useCache)
            {
                if (_imageCache.TryGetValue(path, out Image image))
                {
                    return image;
                }
            }

            if (!File.Exists(path))
                return null;
            var img = Image.FromFile(path);
            if (_imageCache.ContainsKey(path))
                _imageCache[path] = img;
            else
                _imageCache.Add(path,img);
            return img;
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string TryGet(this Dictionary<string, string> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out string t))
            {
                return t;
            }

            return key;
        }
        public static void DrawPlayerRaidDmg(
            ClubData club,
            string path,
            int count,
            Dictionary<string,List<string>> showCard=null,
            int showCardCount=0,
            string imgPath="")
        {
            var Players = club.clan_raid.leaderboard;
            int MaxCount = count;
            int height = 32;
            int width = 400;
            Rectangle rect=new Rectangle(width/2,20,width,height);
            Bitmap bitmap = new Bitmap(width + 20 * 2 + showCardCount * 32+20,
                40 * 2 + Players.Count * (height + 5));
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            
            Pen pen = new Pen(Color.Aqua);
            Brush brush1 = new SolidBrush(Color.Aqua);
            Brush greenBrush = new SolidBrush(Color.Green);
            Brush whiteBrush = new SolidBrush(Color.White);
            double allDmg = 0;
            int allAttack = 0;
            Players.ForEach((i)=>
            {
                allDmg += i.score;
                allAttack += i.num_attacks;
            });
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
            double currentAverage = allDmg / allAttack;
            g.DrawString(allDmg.ShowNum(),font,greenBrush,30,10);
            g.DrawString($"{currentAverage.ShowNum()} / {needAverage.ShowNum()}", font,
                currentAverage > needAverage ? Brushes.Indigo : Brushes.Red, 30, 30);
            g.DrawString(MaxCount.ToString(), font, Brushes.HotPink, 380, 10);

            for (int i = 1; i <= Players.Count; i++)
            {
                var p = Players[i-1];
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

                if (showCard != null)
                {
                    if (showCard.ContainsKey(p.player_code))
                    {
                        var cards = showCard[p.player_code];
                        cards.Sort();
                        int c = 0;
                        foreach (var card in cards)
                        {
                            var png = GetImage(imgPath + card + ".png");
                            if (png!=null)
                            {
                                g.DrawImage(png, new Point(width + 20 * 2+c*32, y));
                            }

                            c++;
                        }
                    }
                }
            }
            bitmap.Save(path);
        }

        public static void DrawPlayerCard(string player, Dictionary<string,int> card,Dictionary<string,string> dic, string path,string imgPath)
        {
            card = new Dictionary<string, int>(card);
            foreach (var (k,cn) in CardName)
            {
                if(!card.ContainsKey(k))
                    card.Add(k,-1);
            }
            
            int rowCount = 10;
            int len = (card.Count-1) / rowCount+1;
            int cardStartY = 0;
            if (dic != null)
                cardStartY = (dic.Count + 1) * 32-50;


            Bitmap bitmap = new Bitmap(32 * rowCount, cardStartY+(len + 1) * 64);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            Font sFont = new Font(FontFamily.GenericMonospace, 12f,FontStyle.Bold);

            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            int i = 0;
            
            i = rowCount;
            List<string> name = new List<string>(card.Keys);
            name.Sort((x, y) => card[y] - card[x]);
            int all = 0;
            int cost = 0;
            foreach (var key in name)
            {
                int x = i % rowCount;
                int y = i / rowCount;

                var png = GetImage(imgPath + key + ".png");
                if (png != null)
                    g.DrawImage(png, new Point(x * 32, cardStartY+y * 64));

                int level = card[key];
                if (level != -1)
                {
                    all += level;
                    cost += CardCost[level];
                    g.DrawString(level.ToString(), font, Brushes.Black, new Point(x * 32, cardStartY+y * 64 + 32));
                }
                else
                {
                    g.DrawString("?", font, Brushes.Black, new Point(x * 32, cardStartY+y * 64 + 32));
                }

                i++;
            }

            if (dic != null)
            {
                //dic.Add("统计总卡等",all.ToString());
                dic.Add("统计灰尘消耗",cost.ToString());
            }
            else
            {
                g.FillRectangle(Brushes.Bisque, 0,cardStartY+32,bitmap.Width,32);
                g.DrawString(player.ToString(), font, Brushes.Black, 10, cardStartY+10);
                g.DrawString(all.ToString(), sFont, Brushes.Brown, 10, cardStartY+40);
                g.DrawString(cost.ToString("N0"), sFont, Brushes.Indigo, 100, cardStartY+40);
            }

            i = 0;
            if (dic != null)
            {
                foreach (var (key, value) in dic)
                {
                    if (i % 2 == 0)
                    {
                        g.FillRectangle(Brushes.Cornsilk, 0, i * 32, bitmap.Width, 32);
                    }

                    g.DrawString(key, font, Brushes.Black, 10, i * 32 + 3);
                    g.DrawString(value, font, Brushes.Brown, 150, i * 32 + 3);

                    i++;
                }
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

        public static void DrawAllCardUse(Dictionary<string, Dictionary<string, int>> cards, string path,
            string imgPath)
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

                num.Add(key, c);
            }

            player.Sort((x, y) => num[y] - num[x]);
            Bitmap bitmap = new Bitmap(280 + 32 * cardName.Count + 50, cards.Count * 64);
            Graphics g = Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f, FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            for (var i = 0; i < player.Count; i++)
            {
                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Cornsilk, 0, i * 64, bitmap.Width, 64);
                }

                var name = player[i];
                g.DrawString(Regex.Unescape(name), font, Brushes.Black, 30, i * 64 + 10);
                int c = 0;
                foreach (var card in cardName)
                {
                    if (cards[name].ContainsKey(card))
                    {
                        var png = GetImage(imgPath + card + ".png");
                        if (png != null)
                            g.DrawImage(png, new Point(280 + c * 32, i * 64));

                        g.DrawString(cards[name][card].ToString(), font, Brushes.Black,
                            new Point(280 + c * 32, i * 64 + 32));
                    }

                    c++;
                }
            }

            bitmap.Save(path);
        }

        public static void DrawAllPlayerCard(Dictionary<string, Dictionary<string, int>> cards, string path,
            string imgPath)
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
                List<string> rl = new List<string>();
                foreach (var (item, level) in value)
                {
                    cardName.Add(item);
                    c += level;
                    if (level == -1)
                    {
                        rl.Add(item);
                    }
                }

                rl.ForEach((k) =>
                {
                    value.Remove(k);
                });
                num.Add(key, c);
            }

            player.Sort((x, y) => num[y] - num[x]);
            Bitmap bitmap = new Bitmap(280 + 32 * cardName.Count + 50, cards.Count * 64);
            Graphics g = Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f, FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            for (var i = 0; i < player.Count; i++)
            {
                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Cornsilk, 0, i * 64, bitmap.Width, 64);
                }

                var name = player[i];
                g.DrawString(Regex.Unescape(name), font, Brushes.Black, 30, i * 64 + 10);
                int c = 0;
                foreach (var card in cardName)
                {
                    if (cards[name].ContainsKey(card))
                    {

                        var png = GetImage(imgPath + card + ".png");
                        if (png != null)
                            g.DrawImage(png, new Point(280 + c * 32, i * 64));

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

                var png = GetImage(imgPath + id + ".png");
                if (png != null)
                    g.DrawImage(png, new Point(x * 64, y * 96));


                g.DrawString(name, font, Brushes.Black, x * 64 + 5, y * 96 + 70);
                i++;
            }

            bitmap.Save(path);
            
        }
        
        
        
        public static void DrawInfo(Dictionary<string, string> dic, string path,int len=100,float fontSize=15f)
        {
            
            Bitmap bitmap = new Bitmap(280 + len, 20+dic.Count*32);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, fontSize,FontStyle.Bold);
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

        public static Rectangle[] atkInfoTitansRect = new Rectangle[]
        {
            //头
            new Rectangle(120, 110, 100, 60),
            //左手
            new Rectangle(10, 180, 100, 60),
            new Rectangle(10, 250, 100, 60),
            //身
            new Rectangle(120, 180, 100, 130),
            //右手
            new Rectangle(230, 180, 100, 60),
            new Rectangle(230, 250, 100, 60),
            //左腿
            new Rectangle(70, 320, 100, 60),
            //右腿
            new Rectangle(180, 320, 100, 60),
        };
        
        public static Point[] atkInfoTitansPoint = new Point[]
        {
            //头
            new Point(123, 138),
            new Point(123, 113),
            
            //身
            new Point(123, 248),
            new Point(123, 183),
            
            new Point(10, 208),
            new Point(10, 183),
            
            new Point(230, 208),
            new Point(230, 183),
            
            new Point(70, 348),
            new Point(70, 323),
            
            new Point(180, 348),
            new Point(180, 323),
            
            new Point(10, 278),
            new Point(10, 253),
            
            new Point(230, 278),
            new Point(230, 253),
            
        };
        public static void DrawAtkInfo(string path, AttackShareInfo info)
        {
            
            Bitmap bitmap = new Bitmap(350, 400);
            Graphics g=Graphics.FromImage(bitmap);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            DrawAtkInfoInline(g, font, info, 0, 0);
            bitmap.Save(path);
        }
        
        public static void DrawAtkInfo(Dictionary<string, List<AttackShareInfo>> info, string path, string imgPath,bool cardLevel=false)
        {
            int max = 0;
            foreach (var (key, value) in info)
            {
                if (value.Count > max)
                    max = value.Count;
            }

            int height = 400;

            Bitmap bitmap = new Bitmap(350*max, info.Count * height);
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
                    g.FillRectangle(Brushes.Cornsilk, 0,i * height,bitmap.Width,height);
                }

                int c = 0;
                g.DrawString(k, font, Brushes.Black, c*350 + 10, i*height + 3);
                foreach (var attack in data)
                {
                    
                    DrawAtkInfoInline(g,font,attack,c*350,i*height);
                    c++;
                }
                i++;
            }
            bitmap.Save(path);
        }

        /// <summary>
        /// atkInfo w=350 h=400
        /// </summary>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="info"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public static void DrawAtkInfoInline(Graphics g, Font font, AttackShareInfo info, int startX, int startY)
        {
            g.DrawString(info.PlayerName, font, Brushes.Black, startX + 10, startY + 3);
            g.DrawString(info.Log.attackTime.ToString(), font, Brushes.Black, startX + 10, startY + 28);
            int c = 0;
            foreach (var (card, value) in info.Data.Card)
            {
                var png = GetImage(RaidRobotModel.Card32Path + card + ".png");
                if (png != null)
                    g.DrawImage(png, new Point(startX + 20 + c * (32 + 10), startY + 50));
                g.DrawString(value.ToString(), font, Brushes.Green,
                    startX + 23 + c * (32 + 10), startY + 85);
                c++;
            }

            g.DrawString($"突等：" + info.Data.RaidLevel, font, Brushes.Black, startX + 160, startY + 55);
            if (info.Data.Dmg <= 0)
                info.CalTap();
            g.DrawString($"伤害：" + info.Data.Dmg.ShowNum(), font, Brushes.Black, startX + 160, startY + 83);
            g.DrawString((info.index+1).ToString(), font, Brushes.Black, startX + 50, startY + 130);
            foreach (var rectangle in atkInfoTitansRect)
            {
                g.DrawRectangle(Pens.Black,
                    new Rectangle(rectangle.X + startX, rectangle.Y + startY, rectangle.Width, rectangle.Height));
            }
            
            foreach (var rectangle in atkInfoTitansRect)
            {
                g.FillRectangle(Brushes.Black,
                    new Rectangle(startX + rectangle.X, startY + rectangle.Y + rectangle.Height / 2, rectangle.Width,
                        rectangle.Height / 2));
            }

            var part = info.GetPartDmg();
            foreach (var (p, d) in part)
            {
                var i = (int)p;
                if (i < atkInfoTitansPoint.Length)
                {
                    var py = atkInfoTitansPoint[i];
                    g.DrawString(d.ShowNum(), font, i % 2 == 0 ? Brushes.Azure : Brushes.Brown,
                        new PointF(startX + py.X, startY + py.Y));
                }
            }
        }

        public static void DrawInfo(List<string> list, string path,int len=250)
        {
            Bitmap bitmap = new Bitmap(len, 20+list.Count*32);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            int i = 0;
            foreach (var key in list)
            {
                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Cornsilk, 0,i * 32,bitmap.Width,32);
                }
                g.DrawString(key, font, Brushes.Black, 10, i * 32 + 3);
                i++;
            }
            bitmap.Save(path);
        }

        public static Bitmap DrawTitanHPChangeInfoOne(Dictionary<TitanData.PartName,double> dmg,
            Dictionary<string,int> atkCount,
            Dictionary<string,List<AttackShareInfo>> atkInfo,
            double outDmg,
            DateTime start,DateTime end, string imgPath)
        {
            int max = 0;
            foreach (var (key, value) in atkInfo)
            {
                if (value.Count > max)
                    max = value.Count;
            }
            
            var maxRow = Math.Max(dmg.Count+1, atkCount.Count);
            
            Bitmap bitmap = new Bitmap(Math.Max(280 + (32 *3+50)* max+50,400+300), 40+(maxRow+1)*32+atkInfo.Count*96);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);

            g.DrawString($"{start:HH:mm:ss}-{end:HH:mm:ss}", font, Brushes.Black, 10, 10);
            if (outDmg > 0)
            {
                g.DrawString("溢出伤害", font, Brushes.Black, 10,  32 + 3);
                g.DrawString(outDmg.ShowNum(), font, Brushes.Brown, 150,  32 + 3);
            }
            int i = 2;
            foreach (var (key, value) in dmg)
            {
                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Cornsilk, 0,i * 32,bitmap.Width,32);
                }

                g.DrawString(TitanData.PartNameShow(key), font, Brushes.Black, 10, i * 32 + 3);
                g.DrawString(value.ShowNum(), font, Brushes.Brown, 150, i * 32 + 3);
                i++;
            }

            i = 0;
            foreach (var (key, value) in atkCount)
            {
                g.DrawString(key, font, Brushes.Black, 300, 32+i * 32 + 3);
                g.DrawString(value.ToString(), font, Brushes.Brown, 580, 32+i * 32 + 3);
                i++;
            }
            

            int cardStartY = 20 + (maxRow+1) * 32;
            
            i = 0;
            foreach (var k in atkInfo)
            {
                var data = k.Value;
                var name = k.Key;
                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Cornsilk, 0,cardStartY+i * 96,bitmap.Width,96);
                }
                g.DrawString(Regex.Unescape(name), font, Brushes.Black, 30, cardStartY+i * 96 + 32);
                int c = 0;
                foreach (var attack in data)
                {
                    g.DrawString((attack.GetTime() + new TimeSpan(8, 0, 0)).ToString("HH:mm:ss"), font, Brushes.Black,
                        280 + c * (32 * 3 + 50), cardStartY + i * 96 + 10);
                    int d = 0;
                    foreach (var (card, value) in attack.Data.Card)
                    {

                        var png = GetImage(imgPath + card + ".png");
                        if (png != null)
                            g.DrawImage(png, new Point(280 + c * (32 * 3 + 50) + d * 32, cardStartY+i * 96 + 32));
                        d++;
                    }

                    g.DrawString(attack.Data.Dmg.ShowNum(), font, Brushes.Black, 280 + c * (32 * 3 + 50),
                        cardStartY + i * 96 + 70);
                    c++;
                }

                i++;
            }

            return bitmap;
        }
        

        public static void DrawTitanHPChangeInfo(
            Dictionary<TitanData.PartName,double> dmg,
            Dictionary<string,int> atkCount,
            Dictionary<string,List<AttackShareInfo>> atkInfo,
            double outDmg,
            DateTime start,DateTime end,
            string path,string imgPath)
        {
            var bitmap= DrawTitanHPChangeInfoOne(dmg, atkCount, atkInfo,outDmg, start, end, imgPath);
            bitmap.Save(path);
        }

        public static void DrawTitanProgress(List<(string name, double hp)> titans, int currentIndex, double hp, string path)
        {
            Bitmap bitmap = new Bitmap(400, titans.Count*36);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            g.FillRectangle(Brushes.White, 0,0,bitmap.Width,bitmap.Height);
            for (var i = 0; i < titans.Count; i++)
            {
                var d = titans[i];
                g.DrawString($"{d.name}\t{d.hp.ShowNum()}",font,Brushes.DarkGreen,30,i*36+5);
                if (i == currentIndex)
                {
                    g.FillRectangle(Brushes.Aqua, 10,i*36+3,(float)(380*(hp/d.hp)),30);
                }else if (i > currentIndex)
                {
                    g.FillRectangle(Brushes.Aqua, 10,i*36+3,380,30);
                }
            }
            
            bitmap.Save(path);
        }
    }
    

}