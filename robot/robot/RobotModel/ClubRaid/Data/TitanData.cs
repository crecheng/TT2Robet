using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace testrobot
{
    [SuppressMessage("Interoperability", "CA1416:验证平台兼容性")]
    public class TitanData
    {
        public List<DeBuff> bonuses;
        public double current_hp;
        public double total_hp;
        public List<DeBuff> enchant_bonuses;
        public string enemy_id;
        public string enemy_name;
        public List<Part> parts;
        public RaidAttackTarget Target;
        public List<RDebuff> area_debuffs;
        public List<RDebuff> cursed_debuffs;
        

        public class Part
        {
            public double current_hp;
            public bool enchanted;
            public PartName part_id;
            public double total_hp;
            public bool cursed;
            
            /// <summary>
            /// 获得当前部位的百分比
            /// </summary>
            /// <returns></returns>
            public float GetPercentage()
            {
                return (float)(current_hp / total_hp);
            }

            public string ShowNum()
            {
                return current_hp.ShowNum();
            }

            public bool HaveArmor()
            {
                return ((int) part_id % 2 == 1) && current_hp > 0;
            }

            public override string ToString()
            {
                return $"当前血量: {current_hp}\n" +
                       $" {nameof(enchanted)}: {enchanted}\n" +
                       $" 部位: {PartNameShow(part_id)}\n" +
                       $" 总血量: {total_hp}";
            }
        }

        public (int blue,int bone)GetBodyInfo()
        {
            int body = 0;
            int armor = 0;
            int all = 0;
            foreach (var part in parts)
            {
                all++;
                if (part.current_hp > 0)
                {
                    if ((int) part.part_id %2!=0)
                        armor++;
                    else
                        body++;
                    
                }
            }

            var blue = body - armor;
            var bone =body- all / 2 ;
            return (blue,bone);
        }

        public void DrawTitan(string name)
        {
            List<Rectangle> rects = new List<Rectangle>()
            {
                new Rectangle(200,80,100,90),//头
                new Rectangle(150,180,200,180),//胸
                
                new Rectangle(10,180,100,90),//左臂-右
                new Rectangle(390,180,100,90),//右臂-左
                

                new Rectangle(140,380,100,110),//左腿-右
                new Rectangle(260,380,100,110),//右腿-左

                new Rectangle(10,280,100,90),//左手-右
                new Rectangle(390,280,100,90),//右手-左
            };
            Bitmap bitmap = new Bitmap(520, 520);
            Graphics g=Graphics.FromImage(bitmap);
            Font font = new Font(FontFamily.GenericMonospace, 15f,FontStyle.Bold);
            Font fontS = new Font(FontFamily.GenericMonospace, 12f,FontStyle.Bold);
            Brush brush = new SolidBrush(Color.Black);
            Brush whiteBrush = new SolidBrush(Color.White);
            Brush indianRedBrush = new SolidBrush(Color.IndianRed);
            g.FillRectangle(brush,0,0,520,520);
            
            Pen pen = new Pen(Color.Aqua);
            g.DrawRectangles(pen,rects.ToArray());
                
            double debuffDmg = 0f;
            #region 绘制肉
            Brush brush1 = new SolidBrush(Color.Aqua);
            parts.ForEach((i) =>
            {
                if (i.enchanted)
                    debuffDmg += i.current_hp;
                if(((int) i.part_id) %2!=0)
                    return;
                int index = ((int) i.part_id) / 2;
                var r = rects[index];
                g.FillRectangle(brush1,r.X,r.Y,r.Width,r.Height*i.GetPercentage());
            });
            #endregion

            #region 绘制顶部血条
            g.DrawRectangle(pen,20,40,490,25);
            g.FillRectangle(brush1,20,40,490*(float)(current_hp/total_hp),25);
            g.DrawString(current_hp.ShowNum(),font,whiteBrush,25,42);
            g.DrawString(total_hp.ShowNum(),font,whiteBrush,20,110);
            g.DrawString(EnemyIdName(enemy_id),font,indianRedBrush,30,90);
            #endregion
            
            #region 绘制甲
            pen.Color=Color.White;
            g.DrawRectangles(pen,rects.ToArray());
            bool[] bools = new bool[parts.Count/2];
            
            parts.ForEach((i) =>
            {
                int tmp=(int) i.part_id;
                if(tmp %2==0)
                    return;
                bools[tmp / 2] = i.HaveArmor();
                int index = ((int) i.part_id) / 2;
                var r = rects[index];
                g.FillRectangle(i.enchanted? Brushes.HotPink:whiteBrush,r.X,r.Y,r.Width,r.Height*i.GetPercentage());
            });
            #endregion

            #region 绘制顶部甲条
            Brush grayBrush=new SolidBrush(Color.FromArgb(215,215,239));
            pen.Color = Color.FromArgb(215,215,239);
            Brush grayBrush1=new SolidBrush(Color.FromArgb(130, 130, 156));
            g.DrawRectangle(pen,20,10,490,25);
            g.FillRectangle(grayBrush,20,10,490*ArmorPercentage(out double haveArmor,out double allArmor),25);
            if(debuffDmg>0)
                g.FillRectangle(Brushes.HotPink,20,10,490*ArmorDeBuffPercentage(),25);
            g.DrawString(haveArmor.ShowNum(),font,grayBrush1,25,12);

            #endregion

            #region 描述诅咒

            if (bonuses != null && bonuses.Count > 0)
            {
                var sing = "";
                if (bonuses[0].BonusAmount > 0)
                    sing = "+";
                g.DrawString($"{RDebuff.Translate(bonuses[0].BonusType)} {sing}{bonuses[0].BonusAmount:P2}",
                    fontS,Brushes.Azure, 330,100);
            }
            if (enchant_bonuses != null && enchant_bonuses.Count > 0)
            {
                var sing = "";
                if (enchant_bonuses[0].BonusAmount > 0)
                    sing = "+";
                g.DrawString(
                    $"{RDebuff.Translate(enchant_bonuses[0].BonusType)} {sing}{enchant_bonuses[0].BonusAmount:P2}",
                    fontS, Brushes.Azure, 330, 130);
            }
            

            #endregion
            
            Brush brush3 = new SolidBrush(Color.Green);
            Brush brush4 = new SolidBrush(Color.CornflowerBlue);

            for (int i = 0; i < bools.Length; i++)
            {
                Part p = null;
                p = bools[i] ? parts[i * 2 + 1] : parts[i * 2];
                var r = rects[i];
                g.DrawString(p.ShowNum(), font, bools[i] ? brush3 : brush4,
                    r.X + r.Width / 2 - 45,
                    r.Y + r.Height / 2 - 5);
            }
            pen.Color=Color.IndianRed;
            parts.ForEach((i) =>
            {
                int index = (int) i.part_id;
                g.DrawString(Target.AttackType(i.part_id),font,indianRedBrush,rects[index/2]);
 
            });
            bitmap.Save(name);
        }
        
        public enum PartName
        {
            BodyHead,
            ArmorHead,
            BodyChestUpper,
            ArmorChestUpper,
            BodyArmUpperRight,
            ArmorArmUpperRight,
            BodyArmUpperLeft,
            ArmorArmUpperLeft,
            BodyLegUpperRight,
            ArmorLegUpperRight,
            BodyLegUpperLeft,
            ArmorLegUpperLeft,
            BodyHandRight,
            ArmorHandRight,
            BodyHandLeft,
            ArmorHandLeft,
            Last
            
        }

        public double GetNeedAllDmg()
        {
            double all = 0;
            parts.ForEach((p) => all += Target.IsAttack(p.part_id) ? p.total_hp : 0);
            return all;
        }

        public float ArmorPercentage(out double HaveArmor, out double AllArmor)
        {
            HaveArmor = 0;
            AllArmor = 0;
            foreach (var t in parts)
            {
                int tmp=(int) t.part_id;
                if(tmp %2==0)
                    continue;
                HaveArmor += t.current_hp;
                AllArmor += t.total_hp;
            }
            return (float)(HaveArmor / AllArmor);
        }

        public float ArmorDeBuffPercentage()
        {
            double HaveDeBuff = 0;
            double AllArmor = 0;
            foreach (var t in parts)
            {
                int tmp=(int) t.part_id;
                if(tmp %2==0)
                    continue;
                if(t.enchanted)
                    HaveDeBuff += t.current_hp;
                AllArmor += t.total_hp;
            }
            return (float)(HaveDeBuff / AllArmor);
        }

        public static string EnemyIdName(string id)
        {
            switch (id)
            {
                case "Enemy1":
                    return "Lojak";
                case "Enemy2":
                    return "Takedar";
                case "Enemy3":
                    return "Jukk";
                case "Enemy4":
                    return "Sterl";
                case "Enemy5":
                    return "Mohaca";
                case "Enemy6":
                    return "Terro";
                case "Enemy7":
                    return "Klonk";
                case "Enemy8":
                    return "Priker";
            }

            return id;
        }
        public static string PartNameShow(PartName partName)
        {
            switch (partName)
            {
                   case PartName.BodyHead:return "头部蓝条";
                   case PartName.ArmorHead:return "头部盔甲";
                   case PartName.BodyChestUpper:return "胸蓝条";
                   case PartName.ArmorChestUpper:return "胸盔甲";
                   case PartName.BodyArmUpperRight:return "左肩蓝条";
                   case PartName.ArmorArmUpperRight:return "左肩盔甲";
                   case PartName.BodyArmUpperLeft:return "右肩蓝条";
                   case PartName.ArmorArmUpperLeft:return "右肩盔甲";
                   case PartName.BodyLegUpperRight:return "左腿蓝条";
                   case PartName.ArmorLegUpperRight:return "左腿盔甲";
                   case PartName.BodyLegUpperLeft:return "右腿蓝条";
                   case PartName.ArmorLegUpperLeft:return "右腿盔甲";
                   case PartName.BodyHandRight:return "左手蓝条";
                   case PartName.ArmorHandRight:return "左手盔甲";
                   case PartName.BodyHandLeft:return "右手蓝条";
                   case PartName.ArmorHandLeft:return "右手盔甲";
                   case PartName.Last:return "上一个";
            }
            return partName.ToString();
        }
        public static string PartNameShowNo(PartName partName)
        {
            switch (partName)
            {
                case PartName.ArmorHead:return "头";
                case PartName.ArmorChestUpper:return "胸";
                case PartName.ArmorArmUpperRight:return "左肩";
                case PartName.ArmorArmUpperLeft:return "右肩";
                case PartName.ArmorLegUpperRight:return "左腿";
                case PartName.ArmorLegUpperLeft:return "右腿";
                case PartName.ArmorHandRight:return "左手";
                case PartName.ArmorHandLeft:return "右手";
            }
            return "";
        }


        public override string ToString()
        {
            return $"{nameof(bonuses)}: {bonuses.ShowAll("\n")}\n" +
                   $" 当前血量: {current_hp}\n" +
                   $" 总血量: {total_hp}\n" +
                   $" {nameof(enchant_bonuses)}: {enchant_bonuses.ShowAll("\n")}\n" +
                   $" 类型: { EnemyIdName(enemy_id)}\n" +
                   $" 部位: {parts.ShowAll("\n")}";
        }
    }
}