using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Currency : IMessage<Currency>, IMessage, IEquatable<Currency>, IDeepCloneable<Currency>
	{
		private static readonly MessageParser<Currency> _parser = new MessageParser<Currency>(() => new Currency());

		public const int CurrencyTypeFieldNumber = 1;

		private CurrencyType currencyType_;

		public const int ValueFieldNumber = 2;

		private long value_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Currency> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[2];
			}
		}

		[DebuggerNonUserCode]
		public CurrencyType CurrencyType
		{
			get
			{
				return currencyType_;
			}
			set
			{
				currencyType_ = value;
			}
		}

		[DebuggerNonUserCode]
		public long Value
		{
			get
			{
				return value_;
			}
			set
			{
				value_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Currency()
		{
		}

		[DebuggerNonUserCode]
		public Currency(Currency other)
			: this()
		{
			currencyType_ = other.currencyType_;
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public Currency Clone()
		{
			return new Currency(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Currency);
		}

		[DebuggerNonUserCode]
		public bool Equals(Currency other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (CurrencyType != other.CurrencyType)
			{
				return false;
			}
			if (Value != other.Value)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (CurrencyType != 0)
			{
				num ^= CurrencyType.GetHashCode();
			}
			if (Value != 0)
			{
				num ^= Value.GetHashCode();
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
			if (CurrencyType != 0)
			{
				output.WriteRawTag(8);
				output.WriteEnum((int)CurrencyType);
			}
			if (Value != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt64(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (CurrencyType != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)CurrencyType);
			}
			if (Value != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(Value);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Currency other)
		{
			if (other != null)
			{
				if (other.CurrencyType != 0)
				{
					CurrencyType = other.CurrencyType;
				}
				if (other.Value != 0)
				{
					Value = other.Value;
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
					currencyType_ = (CurrencyType)input.ReadEnum();
					break;
				case 16u:
					Value = input.ReadInt64();
					break;
				}
			}
		}
	}
}
