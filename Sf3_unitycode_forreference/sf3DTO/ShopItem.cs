using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class ShopItem : IMessage<ShopItem>, IMessage, IEquatable<ShopItem>, IDeepCloneable<ShopItem>
	{
		private static readonly MessageParser<ShopItem> _parser = new MessageParser<ShopItem>(() => new ShopItem());

		public const int ModelIdFieldNumber = 1;

		private int modelId_;

		public const int StackLevelFieldNumber = 2;

		private double stackLevel_;

		public const int PurchaseCountFieldNumber = 3;

		private int purchaseCount_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<ShopItem> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[5];
			}
		}

		[DebuggerNonUserCode]
		public int ModelId
		{
			get
			{
				return modelId_;
			}
			set
			{
				modelId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public double StackLevel
		{
			get
			{
				return stackLevel_;
			}
			set
			{
				stackLevel_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int PurchaseCount
		{
			get
			{
				return purchaseCount_;
			}
			set
			{
				purchaseCount_ = value;
			}
		}

		[DebuggerNonUserCode]
		public ShopItem()
		{
		}

		[DebuggerNonUserCode]
		public ShopItem(ShopItem other)
			: this()
		{
			modelId_ = other.modelId_;
			stackLevel_ = other.stackLevel_;
			purchaseCount_ = other.purchaseCount_;
		}

		[DebuggerNonUserCode]
		public ShopItem Clone()
		{
			return new ShopItem(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as ShopItem);
		}

		[DebuggerNonUserCode]
		public bool Equals(ShopItem other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (ModelId != other.ModelId)
			{
				return false;
			}
			if (StackLevel != other.StackLevel)
			{
				return false;
			}
			if (PurchaseCount != other.PurchaseCount)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (ModelId != 0)
			{
				num ^= ModelId.GetHashCode();
			}
			if (StackLevel != 0.0)
			{
				num ^= StackLevel.GetHashCode();
			}
			if (PurchaseCount != 0)
			{
				num ^= PurchaseCount.GetHashCode();
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
			if (ModelId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(ModelId);
			}
			if (StackLevel != 0.0)
			{
				output.WriteRawTag(17);
				output.WriteDouble(StackLevel);
			}
			if (PurchaseCount != 0)
			{
				output.WriteRawTag(24);
				output.WriteInt32(PurchaseCount);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (ModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ModelId);
			}
			if (StackLevel != 0.0)
			{
				num += 9;
			}
			if (PurchaseCount != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(PurchaseCount);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(ShopItem other)
		{
			if (other != null)
			{
				if (other.ModelId != 0)
				{
					ModelId = other.ModelId;
				}
				if (other.StackLevel != 0.0)
				{
					StackLevel = other.StackLevel;
				}
				if (other.PurchaseCount != 0)
				{
					PurchaseCount = other.PurchaseCount;
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
					ModelId = input.ReadInt32();
					break;
				case 17u:
					StackLevel = input.ReadDouble();
					break;
				case 24u:
					PurchaseCount = input.ReadInt32();
					break;
				}
			}
		}
	}
}
