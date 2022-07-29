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
        if (text.StartsWith("试试\n"))
        {
            var txt = text.Replace("\t", "");
            var len = "试试\n".Length;
            var s = "";
            if (txt.Length > len)
            {
                s = text.Substring(len);
            }

            if (s.Length > 0)
              await SendMsg(s,obj);
        }else if (text.StartsWith("试试图片"))
        {
            MessageBody b = new MessageBody();
            b.Add(Tool.RandomImage("img"));
            await SendMsgObj(b, obj);
        }else if (text.StartsWith("小助手"))
        {
            var s = "";
            if (args.Message.MessageBody[0].MessageType == SegmentType.Text && args.Message.MessageBody.Count==1)
            {
                s = args.Message.RawText;
            }

            if (s.Length <= 0)
                return "";
            var len = "小助手".Length;
            if (s.Length > len)
                s = s.Substring(len);
            if (s.Length <= 0)
                return "";
            var reply= await AI.GetAIReply(args.SourceGroup.Id, args.SenderInfo.UserId, s);
            await SendMsg(reply, obj);
        }

        return "";
    }
}