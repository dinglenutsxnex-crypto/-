using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class BuyBoosterResponse : IMessage<BuyBoosterResponse>, IMessage, IEquatable<BuyBoosterResponse>, IDeepCloneable<BuyBoosterResponse>
	{
		private static readonly MessageParser<BuyBoosterResponse> _parser = new MessageParser<BuyBoosterResponse>(() => new BuyBoosterResponse());

		public const int BoosterFieldNumber = 1;

		private Booster booster_;

		public const int CurrencyFieldNumber = 2;

		private static readonly FieldCodec<Currency> _repeated_currency_codec = FieldCodec.ForMessage(18u, sf3DTO.Currency.Parser);

		private readonly RepeatedField<Currency> currency_ = new RepeatedField<Currency>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<BuyBoosterResponse> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[38];
			}
		}

		[DebuggerNonUserCode]
		public Booster Booster
		{
			get
			{
				return booster_;
			}
			set
			{
				booster_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Currency> Currency
		{
			get
			{
				return currency_;
			}
		}

		[DebuggerNonUserCode]
		public BuyBoosterResponse()
		{
		}

		[DebuggerNonUserCode]
		public BuyBoosterResponse(BuyBoosterResponse other)
			: this()
		{
			Booster = ((other.booster_ == null) ? null : other.Booster.Clone());
			currency_ = other.currency_.Clone();
		}

		[DebuggerNonUserCode]
		public BuyBoosterResponse Clone()
		{
			return new BuyBoosterResponse(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BuyBoosterResponse);
		}

		[DebuggerNonUserCode]
		public bool Equals(BuyBoosterResponse other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!object.Equals(Booster, other.Booster))
			{
				return false;
			}
			if (!currency_.Equals(other.currency_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (booster_ != null)
			{
				num ^= Booster.GetHashCode();
			}
			return num ^ currency_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (booster_ != null)
			{
				output.WriteRawTag(10);
				output.WriteMessage(Booster);
			}
			currency_.WriteTo(output, _repeated_currency_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (booster_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Booster);
			}
			return num + currency_.CalculateSize(_repeated_currency_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BuyBoosterResponse other)
		{
			if (other == null)
			{
				return;
			}
			if (other.booster_ != null)
			{
				if (booster_ == null)
				{
					booster_ = new Booster();
				}
				Booster.MergeFrom(other.Booster);
			}
			currency_.Add(other.currency_);
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
					if (booster_ == null)
					{
						booster_ = new Booster();
					}
					input.ReadMessage(booster_);
					break;
				case 18u:
					currency_.AddEntriesFrom(input, _repeated_currency_codec);
					break;
				}
			}
		}
	}
}
