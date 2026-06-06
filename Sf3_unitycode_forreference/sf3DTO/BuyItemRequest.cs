using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class BuyItemRequest : IMessage<BuyItemRequest>, IMessage, IEquatable<BuyItemRequest>, IDeepCloneable<BuyItemRequest>
	{
		private static readonly MessageParser<BuyItemRequest> _parser = new MessageParser<BuyItemRequest>(() => new BuyItemRequest());

		public const int ModelIdFieldNumber = 1;

		private int modelId_;

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
		public static MessageParser<BuyItemRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[12];
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
		public BuyItemRequest()
		{
		}

		[DebuggerNonUserCode]
		public BuyItemRequest(BuyItemRequest other)
			: this()
		{
			modelId_ = other.modelId_;
			Currency = ((other.currency_ == null) ? null : other.Currency.Clone());
		}

		[DebuggerNonUserCode]
		public BuyItemRequest Clone()
		{
			return new BuyItemRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BuyItemRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(BuyItemRequest other)
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
			if (ModelId != 0)
			{
				num ^= ModelId.GetHashCode();
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
			if (ModelId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(ModelId);
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
			if (ModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ModelId);
			}
			if (currency_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Currency);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BuyItemRequest other)
		{
			if (other == null)
			{
				return;
			}
			if (other.ModelId != 0)
			{
				ModelId = other.ModelId;
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
					ModelId = input.ReadInt32();
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
