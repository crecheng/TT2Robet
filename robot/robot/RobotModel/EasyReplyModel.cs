using Newtonsoft.Json;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.Enumeration;

namespace robot.RobotModel;

public class EasyReplyModel : RobotModelBase
{
	public override string ModelName => "EasyReplyModel";

	public override bool IsAdmin => true;

	public override bool NeedRobotName => true;

	private Dictionary<string, string> _pathReply;
	
	private Dictionary<string, string> _completeReply;
	private string _pathFileName = "PathReply.json";
	private string _completeFileName = "CompleteReply.json";

	public override async Task<SoraMessage> GetMsg(GroupMsgData data)
	{
		long sender = data.Sender;
		bool isAdmin = data.IsAdmin;
		string text = data.Text;
		object obj = data.obj;
		var msg = AddPathReply(sender, text, isAdmin, obj);
		if (msg.HaveData())
			return msg;

		msg = AddCompleteReply(sender, text, isAdmin, obj);
		if (msg.HaveData())
			return msg;
		
		msg = RemoveKeyReply(sender, text, isAdmin, obj);
		if (msg.HaveData())
			return msg;
		
		foreach (var (key, value) in _pathReply)
		{
			if (text.Contains(key))
				return value;
		}

		foreach (var (key, value) in _completeReply)
		{
			if (text==key)
				return value;
		}
		
		return SoraMessage.Null;
		await Task.CompletedTask;
	}

	public override void Init(long group,string robotName)
	{
		base.Init(group,robotName);
		_pathReply = Load<Dictionary<string, string>>(_pathFileName);
		_completeReply = Load<Dictionary<string, string>>(_completeFileName);
	}

	private void SaveData()
	{
		Save(_pathFileName,JsonConvert.SerializeObject(_pathReply));
		Save(_completeFileName,JsonConvert.SerializeObject(_completeReply));
	}

	public SoraMessage AddPathReply(long qq, string text, bool isAdmin, object? obj)
	{
		string text2 = text.Replace("\r", "");
		string[] array = text2.Split('\n');
		if (array.Length >= 4)
		{
			string text3 = array[2];
			int num = RobotName.Length + 1 + array[1].Length + 1 + array[2].Length + 1;
			string value = "";
			if (num < text2.Length)
			{
				value = text2.Substring(num);
			}

			if (array[1].Equals("学习"))
			{
				if (array.Length < 4)
				{
					return "";
				}

				if (!isAdmin)
				{
					return "你是谁，" + RobotName + "和你不熟诶！";
				}

				if (_pathReply.ContainsKey(array[2]))
				{
					return "这个" + RobotName + "之前学过了哦";
				}

				_pathReply.Add(array[2], value);
				SaveData();
				return RobotName + "知道了";
			}
		}

		return SoraMessage.Null;
	}

	public SoraMessage AddCompleteReply(long qq, string text, bool isAdmin, object? obj)
	{
		if (text.StartsWith(RobotName))
		{
			string text2 = text.Replace("\r", "");
			string[] array = text2.Split('\n', StringSplitOptions.None);
			if (array.Length >= 4)
			{
				string text3 = array[2];
				int num = RobotName.Length + 1 + array[1].Length + 1 + array[2].Length + 1;
				string value = "";
				if (num < text2.Length)
				{
					value = text2.Substring(num);
				}

				if (array[1].Equals("学会"))
				{
					if (array.Length < 4)
					{
						return "";
					}

					if (!isAdmin)
					{
						return "你是谁，" + RobotName + "和你不熟诶！";
					}

					if (_completeReply.ContainsKey(array[2]))
					{
						return "这个" + RobotName + "之前知道了哦";
					}

					_completeReply.Add(array[2], value);
					SaveData();
					return RobotName + "知道了";
				}
			}
		}

		return SoraMessage.Null;
	}

	public SoraMessage RemoveKeyReply(long qq, string text, bool isAdmin, object? obj)
	{
		if (text.StartsWith(RobotName))
		{
			string text2 = text.Replace("\r", "");
			string[] array = text2.Split('\n', StringSplitOptions.None);
			if (array.Length >= 3)
			{
				string text3 = array[2];
				int num = RobotName.Length + 1 + array[1].Length + 1 + array[2].Length + 1;
				if (num < text2.Length)
				{
					text2.Substring(num);
				}

				if (array[1].Equals("忘记"))
				{
					if (isAdmin)
					{
						_completeReply.Remove(array[2]);
						_pathReply.Remove(array[2]);
						SaveData();
						return "阿巴阿巴";
					}

					return "你是谁，" + RobotName + "和你不熟诶！";
				}
			}
		}

		return SoraMessage.Null;
	}
}