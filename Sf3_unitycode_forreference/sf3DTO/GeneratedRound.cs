using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class GeneratedRound : IMessage<GeneratedRound>, IMessage, IEquatable<GeneratedRound>, IDeepCloneable<GeneratedRound>
	{
		private static readonly MessageParser<GeneratedRound> _parser = new MessageParser<GeneratedRound>(() => new GeneratedRound());

		public const int RulesFieldNumber = 1;

		private static readonly FieldCodec<RoundRule> _repeated_rules_codec = FieldCodec.ForMessage(10u, RoundRule.Parser);

		private readonly RepeatedField<RoundRule> rules_ = new RepeatedField<RoundRule>();

		public const int WarriorFieldNumber = 2;

		private Warrior warrior_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<GeneratedRound> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[29];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<RoundRule> Rules
		{
			get
			{
				return rules_;
			}
		}

		[DebuggerNonUserCode]
		public Warrior Warrior
		{
			get
			{
				return warrior_;
			}
			set
			{
				warrior_ = value;
			}
		}

		[DebuggerNonUserCode]
		public GeneratedRound()
		{
		}

		[DebuggerNonUserCode]
		public GeneratedRound(GeneratedRound other)
			: this()
		{
			rules_ = other.rules_.Clone();
			Warrior = ((other.warrior_ == null) ? null : other.Warrior.Clone());
		}

		[DebuggerNonUserCode]
		public GeneratedRound Clone()
		{
			return new GeneratedRound(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as GeneratedRound);
		}

		[DebuggerNonUserCode]
		public bool Equals(GeneratedRound other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!rules_.Equals(other.rules_))
			{
				return false;
			}
			if (!object.Equals(Warrior, other.Warrior))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			num ^= rules_.GetHashCode();
			if (warrior_ != null)
			{
				num ^= Warrior.GetHashCode();
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
			rules_.WriteTo(output, _repeated_rules_codec);
			if (warrior_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(Warrior);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			num += rules_.CalculateSize(_repeated_rules_codec);
			if (warrior_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Warrior);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(GeneratedRound other)
		{
			if (other == null)
			{
				return;
			}
			rules_.Add(other.rules_);
			if (other.warrior_ != null)
			{
				if (warrior_ == null)
				{
					warrior_ = new Warrior();
				}
				Warrior.MergeFrom(other.Warrior);
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
				case 10u:
					rules_.AddEntriesFrom(input, _repeated_rules_codec);
					break;
				case 18u:
					if (warrior_ == null)
					{
						warrior_ = new Warrior();
					}
					input.ReadMessage(warrior_);
					break;
				}
			}
		}
	}
}
