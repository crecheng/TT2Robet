using Sora.Entities;
using Sora.Entities.Segment;
using Sora.Entities.Segment.DataModel;
using Sora.Enumeration;
using Sora.EventArgs.SoraEvent;
using Sora.Util;

namespace robot;

public class AdminGroupRobot: GroupSoraRobot
{
    public AdminGroupRobot(long id) : base(id)
    {
    }
    
    

    public override async Task<string> GetMsg(long sendPlayer, string text, object? obj = null)
    {
        GroupMessageEventArgs? args = obj as GroupMessageEventArgs;

        return "";
    }
}