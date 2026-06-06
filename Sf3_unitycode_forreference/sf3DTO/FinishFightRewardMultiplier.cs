using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class FinishFightRewardMultiplier : IMessage<FinishFightRewardMultiplier>, IMessage, IEquatable<FinishFightRewardMultiplier>, IDeepCloneable<FinishFightRewardMultiplier>
	{
		private static readonly MessageParser<FinishFightRewardMultiplier> _parser = new MessageParser<FinishFightRewardMultiplier>(() => new FinishFightRewardMultiplier());

		public const int MultiplierIdFieldNumber = 1;

		private int multiplierId_;

		public const int AmountFieldNumber = 2;

		private int amount_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<FinishFightRewardMultiplier> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[39];
			}
		}

		[DebuggerNonUserCode]
		public int MultiplierId
		{
			get
			{
				return multiplierId_;
			}
			set
			{
				multiplierId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int Amount
		{
			get
			{
				return amount_;
			}
			set
			{
				amount_ = value;
			}
		}

		[DebuggerNonUserCode]
		public FinishFightRewardMultiplier()
		{
		}

		[DebuggerNonUserCode]
		public FinishFightRewardMultiplier(FinishFightRewardMultiplier other)
			: this()
		{
			multiplierId_ = other.multiplierId_;
			amount_ = other.amount_;
		}

		[DebuggerNonUserCode]
		public FinishFightRewardMultiplier Clone()
		{
			return new FinishFightRewardMultiplier(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as FinishFightRewardMultiplier);
		}

		[DebuggerNonUserCode]
		public bool Equals(FinishFightRewardMultiplier other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (MultiplierId != other.MultiplierId)
			{
				return false;
			}
			if (Amount != other.Amount)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (MultiplierId != 0)
			{
				num ^= MultiplierId.GetHashCode();
			}
			if (Amount != 0)
			{
				num ^= Amount.GetHashCode();
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
			if (MultiplierId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(MultiplierId);
			}
			if (Amount != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt32(Amount);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (MultiplierId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(MultiplierId);
			}
			if (Amount != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(Amount);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(FinishFightRewardMultiplier other)
		{
			if (other != null)
			{
				if (other.MultiplierId != 0)
				{
					MultiplierId = other.MultiplierId;
				}
				if (other.Amount != 0)
				{
					Amount = other.Amount;
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
					MultiplierId = input.ReadInt32();
					break;
				case 16u:
					Amount = input.ReadInt32();
					break;
				}
			}
		}
	}
}
