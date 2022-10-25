using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;

namespace testrobot
{
    class Program
    {
        public static ClubData club;
        public static void Main(string[] args)
        {
            TT2Post post=new TT2Post();
            /*var club = post.RaidCurrent();
            club.Init();
            club.GetCurrentTitanData().DrawTitan("123.png");*/
            var s= post.GetForum(34571);
            Console.WriteLine("ok");
            /*var next = Expand.Parse("2022-03-05 19:37:49");
            var span = next - DateTime.Now;
            var tmp=Expand.Parse("2022-01-01 00:00:00")+span;
            Console.WriteLine($"刷新时间: {next:hh:mm:ss}\n剩余时间:{tmp:HH:mm:ss}");*/
            //Console.WriteLine(t.Post(TT2Post.TT2Fun.Forum, new[] {"148269"}));
            //Console.WriteLine(s);
            /*
            string f = File.ReadAllText(TT2Post.TT2Fun.Forum.ToString() + ".json");
            MsgDataList list = JsonConvert.DeserializeObject<MsgDataList>(f);
            var msg = list.messages;
            var cards=new List<MsgDmgData.CardBoard>();
            foreach (var data in msg)
            {
                if (data.message_type=="RaidAttackSummaryShare")
                {
                    var d = MsgDmgData.DeserializeObject(data.message,data.player_from);
                    if (d != null)
                    {
                        cards.AddRange(d.cards);
                    }
                }
            }
             cards.ForEach(Console.WriteLine);
            */
            //Console.WriteLine(list);
            //SaveFile(nameof(TT2Post.TT2Fun.RaidCurrent),s,true);
            //t.Club.Init();
            //t.Club.titan_lords.current.Target.SetAttack(TitanData.PartName.ArmorLegUpperLeft,1);
            //Console.WriteLine(t.SetCurrentTarget()); 
            //t.Club.titan_lords.current.DrawTitan("1.png");
            //Process.Start("1.png");
            //club = JsonConvert.DeserializeObject<ClubData>(
            //File.ReadAllText(@"D:\codeNet\projects\testrobot\bin\Debug\net45\raid.json"));
            //club.Init();
            //club.titan_lords.current.DrawTitan("1.png");
            //DrawPlayerRaidDmg("player.png");
            //Process.Start("1.png");
        }

        public static void SaveFile(string name,string s,bool isAddTime=false)
        {
            string time = "";
            if (isAddTime)
                time = DateTime.Now.ToString("u").Replace(":","-");
            File.WriteAllText(name+time+".json",s);
        }
        

        
        
        
    }
}