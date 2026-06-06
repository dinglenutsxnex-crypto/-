using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace common
{
	public sealed class JoinZoneEvent : IMessage<JoinZoneEvent>, IMessage, IEquatable<JoinZoneEvent>, IDeepCloneable<JoinZoneEvent>
	{
		private static readonly MessageParser<JoinZoneEvent> _parser = new MessageParser<JoinZoneEvent>(() => new JoinZoneEvent());

		public const int PlayerExistsFieldNumber = 1;

		private bool playerExists_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<JoinZoneEvent> Parser
		{
			get
			{
				return _parser;
			}
		}

		[DebuggerNonUserCode]
		public static MessageDescriptor Descriptor
		{
			get
			{
				return CommonReflection.Descriptor.MessageTypes[3];
			}
		}

		[DebuggerNonUserCode]
		public bool PlayerExists
		{
			get
			{
				return playerExists_;
			}
			set
			{
				playerExists_ = value;
			}
		}

		[DebuggerNonUserCode]
		public JoinZoneEvent()
		{
		}

		[DebuggerNonUserCode]
		public JoinZoneEvent(JoinZoneEvent other)
			: this()
		{
			playerExists_ = other.playerExists_;
		}

		[DebuggerNonUserCode]
		public JoinZoneEvent Clone()
		{
			return new JoinZoneEvent(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as JoinZoneEvent);
		}

		[DebuggerNonUserCode]
		public bool Equals(JoinZoneEvent other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (PlayerExists != other.PlayerExists)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (PlayerExists)
			{
				num ^= PlayerExists.GetHashCode();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (PlayerExists)
			{
				output.WriteRawTag(8);
				output.WriteBool(PlayerExists);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (PlayerExists)
			{
				num += 2;
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(JoinZoneEvent other)
		{
			if (other != null && other.PlayerExists)
			{
				PlayerExists = other.PlayerExists;
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				if (num != 8)
				{
					input.SkipLastField();
				}
				else
				{
					PlayerExists = input.ReadBool();
				}
			}
		}
	}
}
