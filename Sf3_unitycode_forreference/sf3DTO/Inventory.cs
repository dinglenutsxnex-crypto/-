using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Inventory : IMessage<Inventory>, IMessage, IEquatable<Inventory>, IDeepCloneable<Inventory>
	{
		private static readonly MessageParser<Inventory> _parser = new MessageParser<Inventory>(() => new Inventory());

		public const int ItemsFieldNumber = 1;

		private static readonly FieldCodec<Item> _repeated_items_codec = FieldCodec.ForMessage(10u, Item.Parser);

		private readonly RepeatedField<Item> items_ = new RepeatedField<Item>();

		public const int PerksFieldNumber = 2;

		private static readonly FieldCodec<Perk> _repeated_perks_codec = FieldCodec.ForMessage(18u, Perk.Parser);

		private readonly RepeatedField<Perk> perks_ = new RepeatedField<Perk>();

		public const int BoostersFieldNumber = 3;

		private static readonly FieldCodec<Booster> _repeated_boosters_codec = FieldCodec.ForMessage(26u, Booster.Parser);

		private readonly RepeatedField<Booster> boosters_ = new RepeatedField<Booster>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Inventory> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[10];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Item> Items
		{
			get
			{
				return items_;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Perk> Perks
		{
			get
			{
				return perks_;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Booster> Boosters
		{
			get
			{
				return boosters_;
			}
		}

		[DebuggerNonUserCode]
		public Inventory()
		{
		}

		[DebuggerNonUserCode]
		public Inventory(Inventory other)
			: this()
		{
			items_ = other.items_.Clone();
			perks_ = other.perks_.Clone();
			boosters_ = other.boosters_.Clone();
		}

		[DebuggerNonUserCode]
		public Inventory Clone()
		{
			return new Inventory(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Inventory);
		}

		[DebuggerNonUserCode]
		public bool Equals(Inventory other)
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
			if (!perks_.Equals(other.perks_))
			{
				return false;
			}
			if (!boosters_.Equals(other.boosters_))
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
			num ^= perks_.GetHashCode();
			return num ^ boosters_.GetHashCode();
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
			perks_.WriteTo(output, _repeated_perks_codec);
			boosters_.WriteTo(output, _repeated_boosters_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			num += items_.CalculateSize(_repeated_items_codec);
			num += perks_.CalculateSize(_repeated_perks_codec);
			return num + boosters_.CalculateSize(_repeated_boosters_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Inventory other)
		{
			if (other != null)
			{
				items_.Add(other.items_);
				perks_.Add(other.perks_);
				boosters_.Add(other.boosters_);
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
					perks_.AddEntriesFrom(input, _repeated_perks_codec);
					break;
				case 26u:
					boosters_.AddEntriesFrom(input, _repeated_boosters_codec);
					break;
				}
			}
		}
	}
}
