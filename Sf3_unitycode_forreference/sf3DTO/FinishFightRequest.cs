using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using common;

namespace sf3DTO
{
	public sealed class FinishFightRequest : IMessage<FinishFightRequest>, IMessage, IEquatable<FinishFightRequest>, IDeepCloneable<FinishFightRequest>
	{
		private static readonly MessageParser<FinishFightRequest> _parser = new MessageParser<FinishFightRequest>(() => new FinishFightRequest());

		public const int BattleModelIdFieldNumber = 1;

		private int battleModelId_;

		public const int BattleCounterFieldNumber = 2;

		private int battleCounter_;

		public const int CurrentFightIndexFieldNumber = 3;

		private int currentFightIndex_;

		public const int ResultFieldNumber = 4;

		private FightResult result_;

		public const int WonRoundsFieldNumber = 5;

		private int wonRounds_;

		public const int FinishTimeFieldNumber = 6;

		private Timestamp finishTime_;

		public const int TotalRoundsFieldNumber = 7;

		private int totalRounds_;

		public const int MultipliersFieldNumber = 8;

		private static readonly FieldCodec<FinishFightRewardMultiplier> _repeated_multipliers_codec = FieldCodec.ForMessage(66u, FinishFightRewardMultiplier.Parser);

		private readonly RepeatedField<FinishFightRewardMultiplier> multipliers_ = new RepeatedField<FinishFightRewardMultiplier>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<FinishFightRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[34];
			}
		}

		[DebuggerNonUserCode]
		public int BattleModelId
		{
			get
			{
				return battleModelId_;
			}
			set
			{
				battleModelId_ = value;
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
		public FightResult Result
		{
			get
			{
				return result_;
			}
			set
			{
				result_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int WonRounds
		{
			get
			{
				return wonRounds_;
			}
			set
			{
				wonRounds_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Timestamp FinishTime
		{
			get
			{
				return finishTime_;
			}
			set
			{
				finishTime_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int TotalRounds
		{
			get
			{
				return totalRounds_;
			}
			set
			{
				totalRounds_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<FinishFightRewardMultiplier> Multipliers
		{
			get
			{
				return multipliers_;
			}
		}

		[DebuggerNonUserCode]
		public FinishFightRequest()
		{
		}

		[DebuggerNonUserCode]
		public FinishFightRequest(FinishFightRequest other)
			: this()
		{
			battleModelId_ = other.battleModelId_;
			battleCounter_ = other.battleCounter_;
			currentFightIndex_ = other.currentFightIndex_;
			result_ = other.result_;
			wonRounds_ = other.wonRounds_;
			FinishTime = ((other.finishTime_ == null) ? null : other.FinishTime.Clone());
			totalRounds_ = other.totalRounds_;
			multipliers_ = other.multipliers_.Clone();
		}

		[DebuggerNonUserCode]
		public FinishFightRequest Clone()
		{
			return new FinishFightRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as FinishFightRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(FinishFightRequest other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (BattleModelId != other.BattleModelId)
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
			if (Result != other.Result)
			{
				return false;
			}
			if (WonRounds != other.WonRounds)
			{
				return false;
			}
			if (!object.Equals(FinishTime, other.FinishTime))
			{
				return false;
			}
			if (TotalRounds != other.TotalRounds)
			{
				return false;
			}
			if (!multipliers_.Equals(other.multipliers_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (BattleModelId != 0)
			{
				num ^= BattleModelId.GetHashCode();
			}
			if (BattleCounter != 0)
			{
				num ^= BattleCounter.GetHashCode();
			}
			if (CurrentFightIndex != 0)
			{
				num ^= CurrentFightIndex.GetHashCode();
			}
			if (Result != 0)
			{
				num ^= Result.GetHashCode();
			}
			if (WonRounds != 0)
			{
				num ^= WonRounds.GetHashCode();
			}
			if (finishTime_ != null)
			{
				num ^= FinishTime.GetHashCode();
			}
			if (TotalRounds != 0)
			{
				num ^= TotalRounds.GetHashCode();
			}
			return num ^ multipliers_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (BattleModelId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(BattleModelId);
			}
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
			if (Result != 0)
			{
				output.WriteRawTag(32);
				output.WriteEnum((int)Result);
			}
			if (WonRounds != 0)
			{
				output.WriteRawTag(40);
				output.WriteInt32(WonRounds);
			}
			if (finishTime_ != null)
			{
				output.WriteRawTag(50);
				output.WriteMessage(FinishTime);
			}
			if (TotalRounds != 0)
			{
				output.WriteRawTag(56);
				output.WriteInt32(TotalRounds);
			}
			multipliers_.WriteTo(output, _repeated_multipliers_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (BattleModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(BattleModelId);
			}
			if (BattleCounter != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(BattleCounter);
			}
			if (CurrentFightIndex != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(CurrentFightIndex);
			}
			if (Result != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)Result);
			}
			if (WonRounds != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(WonRounds);
			}
			if (finishTime_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(FinishTime);
			}
			if (TotalRounds != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(TotalRounds);
			}
			return num + multipliers_.CalculateSize(_repeated_multipliers_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(FinishFightRequest other)
		{
			if (other == null)
			{
				return;
			}
			if (other.BattleModelId != 0)
			{
				BattleModelId = other.BattleModelId;
			}
			if (other.BattleCounter != 0)
			{
				BattleCounter = other.BattleCounter;
			}
			if (other.CurrentFightIndex != 0)
			{
				CurrentFightIndex = other.CurrentFightIndex;
			}
			if (other.Result != 0)
			{
				Result = other.Result;
			}
			if (other.WonRounds != 0)
			{
				WonRounds = other.WonRounds;
			}
			if (other.finishTime_ != null)
			{
				if (finishTime_ == null)
				{
					finishTime_ = new Timestamp();
				}
				FinishTime.MergeFrom(other.FinishTime);
			}
			if (other.TotalRounds != 0)
			{
				TotalRounds = other.TotalRounds;
			}
			multipliers_.Add(other.multipliers_);
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
					BattleModelId = input.ReadInt32();
					break;
				case 16u:
					BattleCounter = input.ReadInt32();
					break;
				case 24u:
					CurrentFightIndex = input.ReadInt32();
					break;
				case 32u:
					result_ = (FightResult)input.ReadEnum();
					break;
				case 40u:
					WonRounds = input.ReadInt32();
					break;
				case 50u:
					if (finishTime_ == null)
					{
						finishTime_ = new Timestamp();
					}
					input.ReadMessage(finishTime_);
					break;
				case 56u:
					TotalRounds = input.ReadInt32();
					break;
				case 66u:
					multipliers_.AddEntriesFrom(input, _repeated_multipliers_codec);
					break;
				}
			}
		}
	}
}
