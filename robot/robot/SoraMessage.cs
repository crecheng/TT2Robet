using System;
using System.Runtime.CompilerServices;
using Sora.Entities;
using Sora.Entities.Segment;

namespace robot
{
	public struct SoraMessage
	{
		public int Type;
		
		public const int OnlyString = 0;
		
		public const int OnlySegment = 1;
		
		public const int StringAndSegment = 2;
		
		public const int SegmentAndString = 3;

		public string Text;
		
		public SoraSegment Segment;

		private MessageBody Body =null;
		
		public static readonly SoraMessage Null = new SoraMessage
		{
			Type = -1
		};
		
		public SoraMessage(string text)
		{
			this.Text = text;
			this.Type = 0;
			this.Segment = SoraSegment.Text(text);
		}
		
		public SoraMessage(SoraSegment segment)
		{
			this.Text = "";
			this.Segment = segment;
			this.Type = 1;
		}
		
		public SoraMessage(string text, SoraSegment segment)
		{
			this.Text = text;
			this.Segment = segment;
			Body = SoraSegment.Text(text) + segment;
			this.Type = 2;
		}
		
		public SoraMessage(SoraSegment segment, string text)
		{
			this.Text = text;
			this.Segment = segment;
			Body = segment+SoraSegment.Text(text);
			this.Type = 3;
		}

		public MessageBody GetSendMsg()
		{
			return Body == null ? Segment : Body;
		}
		
		public bool HaveData()
		{
			return this.Type != -1;
		}
		
		public static implicit operator SoraMessage(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return SoraMessage.Null;
			}
			return new SoraMessage(text);
		}

		public static implicit operator SoraMessage(SoraSegment segment)
		{
			return new SoraMessage(segment);
		}
		

	}
}
