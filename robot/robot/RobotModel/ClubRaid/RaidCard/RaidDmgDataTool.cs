
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using robot.RobotModel;
using testrobot;

// ReSharper disable All
#pragma warning disable CS8600

[SuppressMessage("Interoperability", "CA1416:验证平台兼容性")]
public static class RaidDmgDataTool
{
    public static void DrawDmgData(this RaidDmgData data, int x, int y, Graphics g, Font font)
    {
        g.DrawString(data.RaidLevel.ToString(), font, Brushes.Black, x + 20, y + 20);
        g.DrawString(data.Dmg.ShowNum(), font, Brushes.Indigo, x + 100, y + 20);
        int i = 0;
        foreach (var cardDate in data.CardDate)
        {
            var png = ClubTool.GetImage(RaidRobotModel.Card64Path + cardDate.ID + ".png");
            if (png != null)
            {
                g.DrawImage(png,x+20+i*110,y+50);
            }
            g.DrawString(cardDate.Level.ToString(),font,Brushes.DarkGreen, x+40+i*110,y+120);
            if (cardDate.CardType == "Support")
            {
                if (cardDate.UpdateAdd)
                    g.DrawString((cardDate.UpdateAllAdd / cardDate.UpdateCount).ToString("F2"), 
                        font, Brushes.Chocolate, x + 30 + i * 110, y + 140);
                else
                    g.DrawString(cardDate.Add.ToString("F2"), font, Brushes.Chocolate, x + 30 + i * 110, y + 140);

            }
            else
            {
                g.DrawString(cardDate.Dmg.ShowNum(),font,Brushes.Brown,x+20+i*110,y+140 );
                if(cardDate.CardType=="Burst")
                    g.DrawString(cardDate.Count.ToString(),font,Brushes.Indigo,x+30+i*110,y+160 );
            }
            i++;
        }
    }

    public static void DrawDmgDataList(List<RaidDmgData> list, string path,PlayerData play=null,float add=0,bool onlyHighLow=true)
    {
        //400*200
        int top = 0;
        if (onlyHighLow)
        {
            list.Sort((x,y)=>Comparer<double>.Default.Compare(y.Dmg,x.Dmg));
            List<RaidDmgData> tmp = new List<RaidDmgData>()
            {
                list[0],
                list[list.Count/2],
                list[^1]
            };
            list = tmp;
            top = 80;
        }
        
        Bitmap bitmap = new Bitmap(400, list.Count * 200+top);
        Graphics g = Graphics.FromImage(bitmap);
        Font font = new Font(FontFamily.GenericMonospace, 15f, FontStyle.Bold);
        g.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);


        if (onlyHighLow)
        {
            g.DrawString("平均："+list[1].Dmg.ShowNum(),font,Brushes.Black, 20,20);
            g.DrawString("最高："+list[0].Dmg.ShowNum(),font,Brushes.Black, 200,20);
            g.DrawString("最低："+list[2].Dmg.ShowNum(),font,Brushes.Black, 200,45);
            if (play != null)
            {
                g.DrawString("忠诚："+play.loyalty_level,font,Brushes.Black, 20,70);
                g.DrawString("团队："+add.ToString("P2"),font,Brushes.Black, 200,70);
            }

        }
        int i = 0;
        foreach (var data in list)
        {
            data.DrawDmgData(0,i*200+80,g,font);
            i++;
        }
        
        bitmap.Save(path);
    }
}