using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using common;

namespace sf3DTO
{
	public sealed class Shop : IMessage<Shop>, IMessage, IEquatable<Shop>, IDeepCloneable<Shop>
	{
		private static readonly MessageParser<Shop> _parser = new MessageParser<Shop>(() => new Shop());

		public const int ItemsFieldNumber = 1;

		private static readonly FieldCodec<ShopItem> _repeated_items_codec = FieldCodec.ForMessage(10u, ShopItem.Parser);

		private readonly RepeatedField<ShopItem> items_ = new RepeatedField<ShopItem>();

		public const int LastGenerationTimeFieldNumber = 2;

		private Timestamp lastGenerationTime_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Shop> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[6];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<ShopItem> Items
		{
			get
			{
				return items_;
			}
		}

		[DebuggerNonUserCode]
		public Timestamp LastGenerationTime
		{
			get
			{
				return lastGenerationTime_;
			}
			set
			{
				lastGenerationTime_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Shop()
		{
		}

		[DebuggerNonUserCode]
		public Shop(Shop other)
			: this()
		{
			items_ = other.items_.Clone();
			LastGenerationTime = ((other.lastGenerationTime_ == null) ? null : other.LastGenerationTime.Clone());
		}

		[DebuggerNonUserCode]
		public Shop Clone()
		{
			return new Shop(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Shop);
		}

		[DebuggerNonUserCode]
		public bool Equals(Shop other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!items_.Equals(other.items_))
			{
				return false;
			}
			if (!object.Equals(LastGenerationTime, other.LastGenerationTime))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			num ^= items_.GetHashCode();
			if (lastGenerationTime_ != null)
			{
				num ^= LastGenerationTime.GetHashCode();
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
			items_.WriteTo(output, _repeated_items_codec);
			if (lastGenerationTime_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(LastGenerationTime);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			num += items_.CalculateSize(_repeated_items_codec);
			if (lastGenerationTime_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(LastGenerationTime);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Shop other)
		{
			if (other == null)
			{
				return;
			}
			items_.Add(other.items_);
			if (other.lastGenerationTime_ != null)
			{
				if (lastGenerationTime_ == null)
				{
					lastGenerationTime_ = new Timestamp();
				}
				LastGenerationTime.MergeFrom(other.LastGenerationTime);
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
					items_.AddEntriesFrom(input, _repeated_items_codec);
					break;
				case 18u:
					if (lastGenerationTime_ == null)
					{
						lastGenerationTime_ = new Timestamp();
					}
					input.ReadMessage(lastGenerationTime_);
					break;
				}
			}
		}
	}
}
