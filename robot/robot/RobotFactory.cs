using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace robot
{
	public static class RobotFactory
	{
		private static string Path = "Group";
		
		private static string pathReplyPath = "pathReply.json";
		
		private static string completeReplyPath = "completeReply.json";
		
		private static string functionPath = "fun.json";
		
		private static string adminFunctionPath = "adminFun.json";
		
		private static string adminQQPath = "adminQQ.json";
		
		private static string groupConfigPath = "config.json";
		
		private static Dictionary<string, Func<long, string, object, GroupRobot, SoraMessage>> fun = RobotFunction.GetAllFun();
		
		private static Dictionary<string, Func<long, string, bool, object, GroupRobot, SoraMessage>> adminFun = RobotAdminFunction.GetAllFun();
		public static bool InitGroupRobot(long id, GroupSoraRobot robot)
		{
			string text = $"{Path}/{id}";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			bool result;
			try
			{
				string path = text + "/" + groupConfigPath;
				robot.Config = GetJsonOrNew<RobotConfig>(path);
				string path2 = text + "/" + pathReplyPath;
				robot.PathReply = GetJsonOrNew<Dictionary<string, string>>(path2);
				string path3 = text + "/" + completeReplyPath;
				robot.CompleteReply = GetJsonOrNew<Dictionary<string, string>>(path3);
				string path4 = text + "/" + adminQQPath;
				robot.Admin = GetJsonOrNew<List<long>>(path4);
				string path5 = text + "/" + functionPath;
				robot.FunList = GetJsonOrNew<List<string>>(path5);
				foreach (string key in robot.FunList)
				{
					Func<long, string, object, GroupRobot, SoraMessage> func;
					RobotFactory.fun.TryGetValue(key, out func);
					if (func != null)
					{
						robot.Fun.Add(key, func);
					}
				}
				List<string> jsonOrNew = RobotFactory.GetJsonOrNew<List<string>>(text + "/" + RobotFactory.adminFunctionPath);
				foreach (string key2 in jsonOrNew)
				{
					Func<long, string, bool, object, GroupRobot, SoraMessage> func2;
					RobotFactory.adminFun.TryGetValue(key2, out func2);
					if (func2 != null)
					{
						robot.AdminFun.Add(key2, func2);
					}
				}
				result = true;
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				result = false;
			}
			return result;
		}
		
		public static bool SaveGroupRobot(GroupSoraRobot robot)
		{
			if (robot == null)
			{
				return false;
			}
			string text = $"{Path}/{robot.ID}";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			File.WriteAllText(text + "/" + groupConfigPath, JsonConvert.SerializeObject(robot.Config));
			File.WriteAllText(text + "/" + pathReplyPath, JsonConvert.SerializeObject(robot.PathReply));
			string path = text + "/" + completeReplyPath;
			File.WriteAllText(path, JsonConvert.SerializeObject(robot.CompleteReply));
			robot.CompleteReply = GetJsonOrNew<Dictionary<string, string>>(path);
			File.WriteAllText(text + "/" + adminQQPath, JsonConvert.SerializeObject(robot.Admin));
			File.WriteAllText(text + "/" + functionPath, JsonConvert.SerializeObject(robot.FunList));
			return true;
		}
		
		public static T GetJsonOrNew<T>(string path) where T : new()
		{
			if (!File.Exists(path))
			{
				return Activator.CreateInstance<T>();
			}
			return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
		}
		

	}
}
