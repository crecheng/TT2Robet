// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;
using robot;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.Util;

Console.WriteLine("Hello, Robot!");
Console.WriteLine($"Robot path : {AppDomain.CurrentDomain.BaseDirectory}");
Tool.AppPath = AppDomain.CurrentDomain.BaseDirectory + "\\";
await Manage.Run();

Console.WriteLine("Run begin!");
await Task.Delay(-1);



