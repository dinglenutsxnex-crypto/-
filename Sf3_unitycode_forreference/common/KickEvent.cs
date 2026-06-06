using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace common
{
	public sealed class KickEvent : IMessage<KickEvent>, IMessage, IEquatable<KickEvent>, IDeepCloneable<KickEvent>
	{
		private static readonly MessageParser<KickEvent> _parser = new MessageParser<KickEvent>(() => new KickEvent());

		public const int ReasonFieldNumber = 1;

		private KickReason reason_;

		public const int InitiatorFieldNumber = 2;

		private string initiator_ = string.Empty;

		public const int BanMillisLeftFieldNumber = 3;

		private long banMillisLeft_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<KickEvent> Parser
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
				return CommonReflection.Descriptor.MessageTypes[2];
			}
		}

		[DebuggerNonUserCode]
		public KickReason Reason
		{
			get
			{
				return reason_;
			}
			set
			{
				reason_ = value;
			}
		}

		[DebuggerNonUserCode]
		public string Initiator
		{
			get
			{
				return initiator_;
			}
			set
			{
				initiator_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public long BanMillisLeft
		{
			get
			{
				return banMillisLeft_;
			}
			set
			{
				banMillisLeft_ = value;
			}
		}

		[DebuggerNonUserCode]
		public KickEvent()
		{
		}

		[DebuggerNonUserCode]
		public KickEvent(KickEvent other)
			: this()
		{
			reason_ = other.reason_;
			initiator_ = other.initiator_;
			banMillisLeft_ = other.banMillisLeft_;
		}

		[DebuggerNonUserCode]
		public KickEvent Clone()
		{
			return new KickEvent(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as KickEvent);
		}

		[DebuggerNonUserCode]
		public bool Equals(KickEvent other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (Reason != other.Reason)
			{
				return false;
			}
			if (Initiator != other.Initiator)
			{
				return false;
			}
			if (BanMillisLeft != other.BanMillisLeft)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (Reason != 0)
			{
				num ^= Reason.GetHashCode();
			}
			if (Initiator.Length != 0)
			{
				num ^= Initiator.GetHashCode();
			}
			if (BanMillisLeft != 0)
			{
				num ^= BanMillisLeft.GetHashCode();
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
			if (Reason != 0)
			{
				output.WriteRawTag(8);
				output.WriteEnum((int)Reason);
			}
			if (Initiator.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteString(Initiator);
			}
			if (BanMillisLeft != 0)
			{
				output.WriteRawTag(24);
				output.WriteInt64(BanMillisLeft);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Reason != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)Reason);
			}
			if (Initiator.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Initiator);
			}
			if (BanMillisLeft != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(BanMillisLeft);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(KickEvent other)
		{
			if (other != null)
			{
				if (other.Reason != 0)
				{
					Reason = other.Reason;
				}
				if (other.Initiator.Length != 0)
				{
					Initiator = other.Initiator;
				}
				if (other.BanMillisLeft != 0)
				{
					BanMillisLeft = other.BanMillisLeft;
				}
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				switch (num)
				{
				default:
					input.SkipLastField();
					break;
				case 8u:
					reason_ = (KickReason)input.ReadEnum();
					break;
				case 18u:
					Initiator = input.ReadString();
					break;
				case 24u:
					BanMillisLeft = input.ReadInt64();
					break;
				}
			}
		}
	}
}
