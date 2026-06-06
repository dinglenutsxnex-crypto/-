using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class GeneratedFight : IMessage<GeneratedFight>, IMessage, IEquatable<GeneratedFight>, IDeepCloneable<GeneratedFight>
	{
		private static readonly MessageParser<GeneratedFight> _parser = new MessageParser<GeneratedFight>(() => new GeneratedFight());

		public const int RoundsFieldNumber = 1;

		private static readonly FieldCodec<GeneratedRound> _repeated_rounds_codec = FieldCodec.ForMessage(10u, GeneratedRound.Parser);

		private readonly RepeatedField<GeneratedRound> rounds_ = new RepeatedField<GeneratedRound>();

		public const int RewardsByRoundWinsFieldNumber = 2;

		private static readonly FieldCodec<Loot> _repeated_rewardsByRoundWins_codec = FieldCodec.ForMessage(18u, Loot.Parser);

		private readonly RepeatedField<Loot> rewardsByRoundWins_ = new RepeatedField<Loot>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<GeneratedFight> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[30];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<GeneratedRound> Rounds
		{
			get
			{
				return rounds_;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Loot> RewardsByRoundWins
		{
			get
			{
				return rewardsByRoundWins_;
			}
		}

		[DebuggerNonUserCode]
		public GeneratedFight()
		{
		}

		[DebuggerNonUserCode]
		public GeneratedFight(GeneratedFight other)
			: this()
		{
			rounds_ = other.rounds_.Clone();
			rewardsByRoundWins_ = other.rewardsByRoundWins_.Clone();
		}

		[DebuggerNonUserCode]
		public GeneratedFight Clone()
		{
			return new GeneratedFight(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as GeneratedFight);
		}

		[DebuggerNonUserCode]
		public bool Equals(GeneratedFight other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!rounds_.Equals(other.rounds_))
			{
				return false;
			}
			if (!rewardsByRoundWins_.Equals(other.rewardsByRoundWins_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			num ^= rounds_.GetHashCode();
			return num ^ rewardsByRoundWins_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			rounds_.WriteTo(output, _repeated_rounds_codec);
			rewardsByRoundWins_.WriteTo(output, _repeated_rewardsByRoundWins_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			num += rounds_.CalculateSize(_repeated_rounds_codec);
			return num + rewardsByRoundWins_.CalculateSize(_repeated_rewardsByRoundWins_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(GeneratedFight other)
		{
			if (other != null)
			{
				rounds_.Add(other.rounds_);
				rewardsByRoundWins_.Add(other.rewardsByRoundWins_);
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
					rounds_.AddEntriesFrom(input, _repeated_rounds_codec);
					break;
				case 18u:
					rewardsByRoundWins_.AddEntriesFrom(input, _repeated_rewardsByRoundWins_codec);
					break;
				}
			}
		}
	}
}
