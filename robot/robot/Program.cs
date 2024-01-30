// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using robot;
using robot.SocketTool;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.Net;
using Sora.Util;
using testrobot;
using Websocket.Client;

// var start = DateTime.Now;
// var _config=JsonConvert.DeserializeObject<TT2PostConfig>(File.ReadAllText("config.json"));
// var _post = new TT2Post();
// _post.Tt2Post = _config;
// var raid= _post.RaidCurrent("raid.json");
// Console.WriteLine(File.ReadAllText("raid.json"));
// var span = DateTime.Now - start;
// return;
// Console.WriteLine($"耗时：{span.TotalMilliseconds:F3}ms");
// Console.ReadLine();
// Console.ReadLine();
// return;

// var t = File.ReadAllText("raid.json");
// var data = JsonConvert.DeserializeObject<RaidCurrentData>(t);
// Console.WriteLine(data);
// return;



// TT2PostAPI api = new TT2PostAPI();
// api.OnLog = (s) =>
// {
//     File.AppendAllText("TT2Api.Log", s);
// };
// api.StartSocket();
// await Task.Delay(-1);
// return;

Console.WriteLine("Hello, Robot!");
Console.WriteLine($"Robot path : {AppDomain.CurrentDomain.BaseDirectory}");
Tool.AppPath = AppDomain.CurrentDomain.BaseDirectory;
Tool.DataPath = AppDomain.CurrentDomain.BaseDirectory+"Data\\";
if (!Directory.Exists(Tool.DataPath))
    Directory.CreateDirectory(Tool.DataPath);
await Manage.Run();

Console.WriteLine("Run begin!");
await Task.Delay(-1);



