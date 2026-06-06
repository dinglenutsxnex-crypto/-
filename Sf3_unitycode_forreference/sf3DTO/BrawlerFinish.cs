using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class BrawlerFinish : IMessage<BrawlerFinish>, IMessage, IEquatable<BrawlerFinish>, IDeepCloneable<BrawlerFinish>
	{
		private static readonly MessageParser<BrawlerFinish> _parser = new MessageParser<BrawlerFinish>(() => new BrawlerFinish());

		public const int RewardFieldNumber = 1;

		private Loot reward_;

		public const int RatingDeltaFieldNumber = 2;

		private double ratingDelta_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<BrawlerFinish> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[47];
			}
		}

		[DebuggerNonUserCode]
		public Loot Reward
		{
			get
			{
				return reward_;
			}
			set
			{
				reward_ = value;
			}
		}

		[DebuggerNonUserCode]
		public double RatingDelta
		{
			get
			{
				return ratingDelta_;
			}
			set
			{
				ratingDelta_ = value;
			}
		}

		[DebuggerNonUserCode]
		public BrawlerFinish()
		{
		}

		[DebuggerNonUserCode]
		public BrawlerFinish(BrawlerFinish other)
			: this()
		{
			Reward = ((other.reward_ == null) ? null : other.Reward.Clone());
			ratingDelta_ = other.ratingDelta_;
		}

		[DebuggerNonUserCode]
		public BrawlerFinish Clone()
		{
			return new BrawlerFinish(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BrawlerFinish);
		}

		[DebuggerNonUserCode]
		public bool Equals(BrawlerFinish other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!object.Equals(Reward, other.Reward))
			{
				return false;
			}
			if (RatingDelta != other.RatingDelta)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (reward_ != null)
			{
				num ^= Reward.GetHashCode();
			}
			if (RatingDelta != 0.0)
			{
				num ^= RatingDelta.GetHashCode();
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
			if (reward_ != null)
			{
				output.WriteRawTag(10);
				output.WriteMessage(Reward);
			}
			if (RatingDelta != 0.0)
			{
				output.WriteRawTag(17);
				output.WriteDouble(RatingDelta);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (reward_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Reward);
			}
			if (RatingDelta != 0.0)
			{
				num += 9;
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BrawlerFinish other)
		{
			if (other == null)
			{
				return;
			}
			if (other.reward_ != null)
			{
				if (reward_ == null)
				{
					reward_ = new Loot();
				}
				Reward.MergeFrom(other.Reward);
			}
			if (other.RatingDelta != 0.0)
			{
				RatingDelta = other.RatingDelta;
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
					if (reward_ == null)
					{
						reward_ = new Loot();
					}
					input.ReadMessage(reward_);
					break;
				case 17u:
					RatingDelta = input.ReadDouble();
					break;
				}
			}
		}
	}
}
