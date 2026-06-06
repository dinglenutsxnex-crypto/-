using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class RoundRuleAttribute : IMessage<RoundRuleAttribute>, IMessage, IEquatable<RoundRuleAttribute>, IDeepCloneable<RoundRuleAttribute>
	{
		private static readonly MessageParser<RoundRuleAttribute> _parser = new MessageParser<RoundRuleAttribute>(() => new RoundRuleAttribute());

		public const int AttrIdFieldNumber = 1;

		private int attrId_;

		public const int AttrValueFieldNumber = 2;

		private string attrValue_ = string.Empty;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<RoundRuleAttribute> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[27];
			}
		}

		[DebuggerNonUserCode]
		public int AttrId
		{
			get
			{
				return attrId_;
			}
			set
			{
				attrId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public string AttrValue
		{
			get
			{
				return attrValue_;
			}
			set
			{
				attrValue_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public RoundRuleAttribute()
		{
		}

		[DebuggerNonUserCode]
		public RoundRuleAttribute(RoundRuleAttribute other)
			: this()
		{
			attrId_ = other.attrId_;
			attrValue_ = other.attrValue_;
		}

		[DebuggerNonUserCode]
		public RoundRuleAttribute Clone()
		{
			return new RoundRuleAttribute(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as RoundRuleAttribute);
		}

		[DebuggerNonUserCode]
		public bool Equals(RoundRuleAttribute other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (AttrId != other.AttrId)
			{
				return false;
			}
			if (AttrValue != other.AttrValue)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (AttrId != 0)
			{
				num ^= AttrId.GetHashCode();
			}
			if (AttrValue.Length != 0)
			{
				num ^= AttrValue.GetHashCode();
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
			if (AttrId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(AttrId);
			}
			if (AttrValue.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteString(AttrValue);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (AttrId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(AttrId);
			}
			if (AttrValue.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(AttrValue);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(RoundRuleAttribute other)
		{
			if (other != null)
			{
				if (other.AttrId != 0)
				{
					AttrId = other.AttrId;
				}
				if (other.AttrValue.Length != 0)
				{
					AttrValue = other.AttrValue;
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
					AttrId = input.ReadInt32();
					break;
				case 18u:
					AttrValue = input.ReadString();
					break;
				}
			}
		}
	}
}
