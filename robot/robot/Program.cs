// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;
using robot;
using robot.SocketTool;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.Util;

// TT2PostAPI api = new TT2PostAPI();
// api.OnLog = (s) =>
// {
//     File.AppendAllText("TT2Api.Log", s);
// };
// api.StartSocket();
// await Task.Delay(-1);
Console.WriteLine("Hello, Robot!");
Console.WriteLine($"Robot path : {AppDomain.CurrentDomain.BaseDirectory}");
Tool.AppPath = AppDomain.CurrentDomain.BaseDirectory;
Tool.DataPath = AppDomain.CurrentDomain.BaseDirectory+"Data\\";
if (!Directory.Exists(Tool.DataPath))
    Directory.CreateDirectory(Tool.DataPath);
await Manage.Run();

Console.WriteLine("Run begin!");
await Task.Delay(-1);



