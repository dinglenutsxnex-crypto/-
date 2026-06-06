using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class BrawlerFinishRequest : IMessage<BrawlerFinishRequest>, IMessage, IEquatable<BrawlerFinishRequest>, IDeepCloneable<BrawlerFinishRequest>
	{
		private static readonly MessageParser<BrawlerFinishRequest> _parser = new MessageParser<BrawlerFinishRequest>(() => new BrawlerFinishRequest());

		public const int EnemyFieldNumber = 1;

		private BrawlerEnemy enemy_;

		public const int ResultFieldNumber = 2;

		private FightResult result_;

		public const int TotalRoundsFieldNumber = 3;

		private int totalRounds_;

		public const int MultipliersFieldNumber = 4;

		private static readonly FieldCodec<FinishFightRewardMultiplier> _repeated_multipliers_codec = FieldCodec.ForMessage(34u, FinishFightRewardMultiplier.Parser);

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
		public static MessageParser<BrawlerFinishRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[46];
			}
		}

		[DebuggerNonUserCode]
		public BrawlerEnemy Enemy
		{
			get
			{
				return enemy_;
			}
			set
			{
				enemy_ = value;
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
		public BrawlerFinishRequest()
		{
		}

		[DebuggerNonUserCode]
		public BrawlerFinishRequest(BrawlerFinishRequest other)
			: this()
		{
			Enemy = ((other.enemy_ == null) ? null : other.Enemy.Clone());
			result_ = other.result_;
			totalRounds_ = other.totalRounds_;
			multipliers_ = other.multipliers_.Clone();
		}

		[DebuggerNonUserCode]
		public BrawlerFinishRequest Clone()
		{
			return new BrawlerFinishRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BrawlerFinishRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(BrawlerFinishRequest other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!object.Equals(Enemy, other.Enemy))
			{
				return false;
			}
			if (Result != other.Result)
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
			if (enemy_ != null)
			{
				num ^= Enemy.GetHashCode();
			}
			if (Result != 0)
			{
				num ^= Result.GetHashCode();
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
			if (enemy_ != null)
			{
				output.WriteRawTag(10);
				output.WriteMessage(Enemy);
			}
			if (Result != 0)
			{
				output.WriteRawTag(16);
				output.WriteEnum((int)Result);
			}
			if (TotalRounds != 0)
			{
				output.WriteRawTag(24);
				output.WriteInt32(TotalRounds);
			}
			multipliers_.WriteTo(output, _repeated_multipliers_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (enemy_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Enemy);
			}
			if (Result != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)Result);
			}
			if (TotalRounds != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(TotalRounds);
			}
			return num + multipliers_.CalculateSize(_repeated_multipliers_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BrawlerFinishRequest other)
		{
			if (other == null)
			{
				return;
			}
			if (other.enemy_ != null)
			{
				if (enemy_ == null)
				{
					enemy_ = new BrawlerEnemy();
				}
				Enemy.MergeFrom(other.Enemy);
			}
			if (other.Result != 0)
			{
				Result = other.Result;
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
				case 10u:
					if (enemy_ == null)
					{
						enemy_ = new BrawlerEnemy();
					}
					input.ReadMessage(enemy_);
					break;
				case 16u:
					result_ = (FightResult)input.ReadEnum();
					break;
				case 24u:
					TotalRounds = input.ReadInt32();
					break;
				case 34u:
					multipliers_.AddEntriesFrom(input, _repeated_multipliers_codec);
					break;
				}
			}
		}
	}
}
