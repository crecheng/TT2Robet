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

		private static string adminQQPath = "adminQQ.json";
		
		private static string groupConfigPath = "config.json";
		public static bool InitGroupRobot(long id, GroupRobot robot)
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
				
				if(!robot.Config.Admin.Contains(Config.AdminQQ))
					robot.Config.Admin.Add(Config.AdminQQ);
				
				foreach (var s in robot.Config.RobotModel)
				{
					var model = RobotModelFactory.GetModel(s, robot);
					if(model!=null)
						robot.RobotModel.Add(s,model);
				}
				result = true;
				SaveGroupRobot(robot);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				result = false;
			}
			return result;
		}
		
		public static bool SaveGroupRobot(GroupRobot robot)
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
