using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace robot
{
    public static class RobotFunction
    {
        public static Dictionary<string, Func<long, string, object, GroupRobot, SoraMessage>> GetAllFun()
        {
            return new Dictionary<string, Func<long, string, object, GroupRobot, SoraMessage>>
            {
                {"随机图片", RandomImage}
            };
        }
        
        public static SoraMessage RandomImage(long qq, string text, object obj, GroupRobot robot)
        {
            if (text == "嘤嘤嘤")
            {
                return Tool.RandomImage("Img");
            }
            return SoraMessage.Null;
        }
    }
}