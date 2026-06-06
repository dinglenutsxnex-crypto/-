using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class RoundRule : IMessage<RoundRule>, IMessage, IEquatable<RoundRule>, IDeepCloneable<RoundRule>
	{
		private static readonly MessageParser<RoundRule> _parser = new MessageParser<RoundRule>(() => new RoundRule());

		public const int RuleIdFieldNumber = 1;

		private int ruleId_;

		public const int AttrsFieldNumber = 2;

		private static readonly FieldCodec<RoundRuleAttribute> _repeated_attrs_codec = FieldCodec.ForMessage(18u, RoundRuleAttribute.Parser);

		private readonly RepeatedField<RoundRuleAttribute> attrs_ = new RepeatedField<RoundRuleAttribute>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<RoundRule> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[28];
			}
		}

		[DebuggerNonUserCode]
		public int RuleId
		{
			get
			{
				return ruleId_;
			}
			set
			{
				ruleId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<RoundRuleAttribute> Attrs
		{
			get
			{
				return attrs_;
			}
		}

		[DebuggerNonUserCode]
		public RoundRule()
		{
		}

		[DebuggerNonUserCode]
		public RoundRule(RoundRule other)
			: this()
		{
			ruleId_ = other.ruleId_;
			attrs_ = other.attrs_.Clone();
		}

		[DebuggerNonUserCode]
		public RoundRule Clone()
		{
			return new RoundRule(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as RoundRule);
		}

		[DebuggerNonUserCode]
		public bool Equals(RoundRule other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (RuleId != other.RuleId)
			{
				return false;
			}
			if (!attrs_.Equals(other.attrs_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (RuleId != 0)
			{
				num ^= RuleId.GetHashCode();
			}
			return num ^ attrs_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (RuleId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(RuleId);
			}
			attrs_.WriteTo(output, _repeated_attrs_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (RuleId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(RuleId);
			}
			return num + attrs_.CalculateSize(_repeated_attrs_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(RoundRule other)
		{
			if (other != null)
			{
				if (other.RuleId != 0)
				{
					RuleId = other.RuleId;
				}
				attrs_.Add(other.attrs_);
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
					RuleId = input.ReadInt32();
					break;
				case 18u:
					attrs_.AddEntriesFrom(input, _repeated_attrs_codec);
					break;
				}
			}
		}
	}
}
