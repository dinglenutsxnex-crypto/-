using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Loot : IMessage<Loot>, IMessage, IEquatable<Loot>, IDeepCloneable<Loot>
	{
		private static readonly MessageParser<Loot> _parser = new MessageParser<Loot>(() => new Loot());

		public const int CurrenciesFieldNumber = 1;

		private static readonly FieldCodec<Currency> _repeated_currencies_codec = FieldCodec.ForMessage(10u, Currency.Parser);

		private readonly RepeatedField<Currency> currencies_ = new RepeatedField<Currency>();

		public const int ExperienceFieldNumber = 2;

		private long experience_;

		public const int EquipmentsFieldNumber = 3;

		private static readonly FieldCodec<Item> _repeated_equipments_codec = FieldCodec.ForMessage(26u, Item.Parser);

		private readonly RepeatedField<Item> equipments_ = new RepeatedField<Item>();

		public const int PerksFieldNumber = 4;

		private static readonly FieldCodec<Perk> _repeated_perks_codec = FieldCodec.ForMessage(34u, Perk.Parser);

		private readonly RepeatedField<Perk> perks_ = new RepeatedField<Perk>();

		public const int BoostersFieldNumber = 5;

		private static readonly FieldCodec<Booster> _repeated_boosters_codec = FieldCodec.ForMessage(42u, Booster.Parser);

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
		public static MessageParser<Loot> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[24];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Currency> Currencies
		{
			get
			{
				return currencies_;
			}
		}

		[DebuggerNonUserCode]
		public long Experience
		{
			get
			{
				return experience_;
			}
			set
			{
				experience_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Item> Equipments
		{
			get
			{
				return equipments_;
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
		public Loot()
		{
		}

		[DebuggerNonUserCode]
		public Loot(Loot other)
			: this()
		{
			currencies_ = other.currencies_.Clone();
			experience_ = other.experience_;
			equipments_ = other.equipments_.Clone();
			perks_ = other.perks_.Clone();
			boosters_ = other.boosters_.Clone();
		}

		[DebuggerNonUserCode]
		public Loot Clone()
		{
			return new Loot(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Loot);
		}

		[DebuggerNonUserCode]
		public bool Equals(Loot other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!currencies_.Equals(other.currencies_))
			{
				return false;
			}
			if (Experience != other.Experience)
			{
				return false;
			}
			if (!equipments_.Equals(other.equipments_))
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
			num ^= currencies_.GetHashCode();
			if (Experience != 0)
			{
				num ^= Experience.GetHashCode();
			}
			num ^= equipments_.GetHashCode();
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
			currencies_.WriteTo(output, _repeated_currencies_codec);
			if (Experience != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt64(Experience);
			}
			equipments_.WriteTo(output, _repeated_equipments_codec);
			perks_.WriteTo(output, _repeated_perks_codec);
			boosters_.WriteTo(output, _repeated_boosters_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			num += currencies_.CalculateSize(_repeated_currencies_codec);
			if (Experience != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(Experience);
			}
			num += equipments_.CalculateSize(_repeated_equipments_codec);
			num += perks_.CalculateSize(_repeated_perks_codec);
			return num + boosters_.CalculateSize(_repeated_boosters_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Loot other)
		{
			if (other != null)
			{
				currencies_.Add(other.currencies_);
				if (other.Experience != 0)
				{
					Experience = other.Experience;
				}
				equipments_.Add(other.equipments_);
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
					currencies_.AddEntriesFrom(input, _repeated_currencies_codec);
					break;
				case 16u:
					Experience = input.ReadInt64();
					break;
				case 26u:
					equipments_.AddEntriesFrom(input, _repeated_equipments_codec);
					break;
				case 34u:
					perks_.AddEntriesFrom(input, _repeated_perks_codec);
					break;
				case 42u:
					boosters_.AddEntriesFrom(input, _repeated_boosters_codec);
					break;
				}
			}
		}
	}
}
