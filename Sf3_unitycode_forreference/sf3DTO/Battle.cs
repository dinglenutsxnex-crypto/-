using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using common;

namespace sf3DTO
{
	public sealed class Battle : IMessage<Battle>, IMessage, IEquatable<Battle>, IDeepCloneable<Battle>
	{
		private static readonly MessageParser<Battle> _parser = new MessageParser<Battle>(() => new Battle());

		public const int BattlesFieldNumber = 1;

		private static readonly FieldCodec<GeneratedBattle> _repeated_battles_codec = FieldCodec.ForMessage(10u, GeneratedBattle.Parser);

		private readonly RepeatedField<GeneratedBattle> battles_ = new RepeatedField<GeneratedBattle>();

		public const int BattleCounterFieldNumber = 2;

		private int battleCounter_;

		public const int CurrentFightIndexFieldNumber = 3;

		private int currentFightIndex_;

		public const int GenTimeFieldNumber = 4;

		private Timestamp genTime_;

		public const int LastFightFinishTimeFieldNumber = 5;

		private Timestamp lastFightFinishTime_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Battle> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[32];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<GeneratedBattle> Battles
		{
			get
			{
				return battles_;
			}
		}

		[DebuggerNonUserCode]
		public int BattleCounter
		{
			get
			{
				return battleCounter_;
			}
			set
			{
				battleCounter_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int CurrentFightIndex
		{
			get
			{
				return currentFightIndex_;
			}
			set
			{
				currentFightIndex_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Timestamp GenTime
		{
			get
			{
				return genTime_;
			}
			set
			{
				genTime_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Timestamp LastFightFinishTime
		{
			get
			{
				return lastFightFinishTime_;
			}
			set
			{
				lastFightFinishTime_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Battle()
		{
		}

		[DebuggerNonUserCode]
		public Battle(Battle other)
			: this()
		{
			battles_ = other.battles_.Clone();
			battleCounter_ = other.battleCounter_;
			currentFightIndex_ = other.currentFightIndex_;
			GenTime = ((other.genTime_ == null) ? null : other.GenTime.Clone());
			LastFightFinishTime = ((other.lastFightFinishTime_ == null) ? null : other.LastFightFinishTime.Clone());
		}

		[DebuggerNonUserCode]
		public Battle Clone()
		{
			return new Battle(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Battle);
		}

		[DebuggerNonUserCode]
		public bool Equals(Battle other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!battles_.Equals(other.battles_))
			{
				return false;
			}
			if (BattleCounter != other.BattleCounter)
			{
				return false;
			}
			if (CurrentFightIndex != other.CurrentFightIndex)
			{
				return false;
			}
			if (!object.Equals(GenTime, other.GenTime))
			{
				return false;
			}
			if (!object.Equals(LastFightFinishTime, other.LastFightFinishTime))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			num ^= battles_.GetHashCode();
			if (BattleCounter != 0)
			{
				num ^= BattleCounter.GetHashCode();
			}
			if (CurrentFightIndex != 0)
			{
				num ^= CurrentFightIndex.GetHashCode();
			}
			if (genTime_ != null)
			{
				num ^= GenTime.GetHashCode();
			}
			if (lastFightFinishTime_ != null)
			{
				num ^= LastFightFinishTime.GetHashCode();
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
			battles_.WriteTo(output, _repeated_battles_codec);
			if (BattleCounter != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt32(BattleCounter);
			}
			if (CurrentFightIndex != 0)
			{
				output.WriteRawTag(24);
				output.WriteInt32(CurrentFightIndex);
			}
			if (genTime_ != null)
			{
				output.WriteRawTag(34);
				output.WriteMessage(GenTime);
			}
			if (lastFightFinishTime_ != null)
			{
				output.WriteRawTag(42);
				output.WriteMessage(LastFightFinishTime);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			num += battles_.CalculateSize(_repeated_battles_codec);
			if (BattleCounter != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(BattleCounter);
			}
			if (CurrentFightIndex != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(CurrentFightIndex);
			}
			if (genTime_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(GenTime);
			}
			if (lastFightFinishTime_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(LastFightFinishTime);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Battle other)
		{
			if (other == null)
			{
				return;
			}
			battles_.Add(other.battles_);
			if (other.BattleCounter != 0)
			{
				BattleCounter = other.BattleCounter;
			}
			if (other.CurrentFightIndex != 0)
			{
				CurrentFightIndex = other.CurrentFightIndex;
			}
			if (other.genTime_ != null)
			{
				if (genTime_ == null)
				{
					genTime_ = new Timestamp();
				}
				GenTime.MergeFrom(other.GenTime);
			}
			if (other.lastFightFinishTime_ != null)
			{
				if (lastFightFinishTime_ == null)
				{
					lastFightFinishTime_ = new Timestamp();
				}
				LastFightFinishTime.MergeFrom(other.LastFightFinishTime);
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
					battles_.AddEntriesFrom(input, _repeated_battles_codec);
					break;
				case 16u:
					BattleCounter = input.ReadInt32();
					break;
				case 24u:
					CurrentFightIndex = input.ReadInt32();
					break;
				case 34u:
					if (genTime_ == null)
					{
						genTime_ = new Timestamp();
					}
					input.ReadMessage(genTime_);
					break;
				case 42u:
					if (lastFightFinishTime_ == null)
					{
						lastFightFinishTime_ = new Timestamp();
					}
					input.ReadMessage(lastFightFinishTime_);
					break;
				}
			}
		}
	}
}
