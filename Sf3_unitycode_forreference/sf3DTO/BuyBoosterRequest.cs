using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class BuyBoosterRequest : IMessage<BuyBoosterRequest>, IMessage, IEquatable<BuyBoosterRequest>, IDeepCloneable<BuyBoosterRequest>
	{
		private static readonly MessageParser<BuyBoosterRequest> _parser = new MessageParser<BuyBoosterRequest>(() => new BuyBoosterRequest());

		public const int ShopBoosterModelIdFieldNumber = 1;

		private int shopBoosterModelId_;

		public const int CurrencyFieldNumber = 2;

		private Currency currency_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<BuyBoosterRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[37];
			}
		}

		[DebuggerNonUserCode]
		public int ShopBoosterModelId
		{
			get
			{
				return shopBoosterModelId_;
			}
			set
			{
				shopBoosterModelId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Currency Currency
		{
			get
			{
				return currency_;
			}
			set
			{
				currency_ = value;
			}
		}

		[DebuggerNonUserCode]
		public BuyBoosterRequest()
		{
		}

		[DebuggerNonUserCode]
		public BuyBoosterRequest(BuyBoosterRequest other)
			: this()
		{
			shopBoosterModelId_ = other.shopBoosterModelId_;
			Currency = ((other.currency_ == null) ? null : other.Currency.Clone());
		}

		[DebuggerNonUserCode]
		public BuyBoosterRequest Clone()
		{
			return new BuyBoosterRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BuyBoosterRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(BuyBoosterRequest other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (ShopBoosterModelId != other.ShopBoosterModelId)
			{
				return false;
			}
			if (!object.Equals(Currency, other.Currency))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (ShopBoosterModelId != 0)
			{
				num ^= ShopBoosterModelId.GetHashCode();
			}
			if (currency_ != null)
			{
				num ^= Currency.GetHashCode();
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
			if (ShopBoosterModelId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(ShopBoosterModelId);
			}
			if (currency_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(Currency);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (ShopBoosterModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ShopBoosterModelId);
			}
			if (currency_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Currency);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BuyBoosterRequest other)
		{
			if (other == null)
			{
				return;
			}
			if (other.ShopBoosterModelId != 0)
			{
				ShopBoosterModelId = other.ShopBoosterModelId;
			}
			if (other.currency_ != null)
			{
				if (currency_ == null)
				{
					currency_ = new Currency();
				}
				Currency.MergeFrom(other.Currency);
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
					ShopBoosterModelId = input.ReadInt32();
					break;
				case 18u:
					if (currency_ == null)
					{
						currency_ = new Currency();
					}
					input.ReadMessage(currency_);
					break;
				}
			}
		}
	}
}
