using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace robot
{
	public static class RobotAdminFunction
	{
		public static Dictionary<string, Func<long, string, bool, object, GroupRobot, SoraMessage>> GetAllFun()
		{
			return new Dictionary<string, Func<long, string, bool, object, GroupRobot, SoraMessage>>
			{
				{"别说话", RobotOff},
			};
		}
		
		public static SoraMessage RobotOff(long qq, string text, bool isAdmin, object obj, GroupRobot robot)
		{
			return SoraMessage.Null;
		}
		

	}
}
