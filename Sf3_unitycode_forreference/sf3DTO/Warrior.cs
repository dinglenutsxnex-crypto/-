using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Warrior : IMessage<Warrior>, IMessage, IEquatable<Warrior>, IDeepCloneable<Warrior>
	{
		private static readonly MessageParser<Warrior> _parser = new MessageParser<Warrior>(() => new Warrior());

		public const int AliasFieldNumber = 1;

		private string alias_ = string.Empty;

		public const int GenderFieldNumber = 2;

		private Gender gender_;

		public const int AppearanceIdFieldNumber = 3;

		private int appearanceId_;

		public const int AiModeFieldNumber = 4;

		private AiMode aiMode_;

		public const int PowerFieldNumber = 5;

		private double power_;

		public const int EquipmentsFieldNumber = 6;

		private static readonly FieldCodec<WarriorItemId> _repeated_equipments_codec = FieldCodec.ForMessage(50u, WarriorItemId.Parser);

		private readonly RepeatedField<WarriorItemId> equipments_ = new RepeatedField<WarriorItemId>();

		public const int PerksFieldNumber = 7;

		private static readonly FieldCodec<Perk> _repeated_perks_codec = FieldCodec.ForMessage(58u, Perk.Parser);

		private readonly RepeatedField<Perk> perks_ = new RepeatedField<Perk>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Warrior> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[26];
			}
		}

		[DebuggerNonUserCode]
		public string Alias
		{
			get
			{
				return alias_;
			}
			set
			{
				alias_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public Gender Gender
		{
			get
			{
				return gender_;
			}
			set
			{
				gender_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int AppearanceId
		{
			get
			{
				return appearanceId_;
			}
			set
			{
				appearanceId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public AiMode AiMode
		{
			get
			{
				return aiMode_;
			}
			set
			{
				aiMode_ = value;
			}
		}

		[DebuggerNonUserCode]
		public double Power
		{
			get
			{
				return power_;
			}
			set
			{
				power_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<WarriorItemId> Equipments
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
		public Warrior()
		{
		}

		[DebuggerNonUserCode]
		public Warrior(Warrior other)
			: this()
		{
			alias_ = other.alias_;
			gender_ = other.gender_;
			appearanceId_ = other.appearanceId_;
			aiMode_ = other.aiMode_;
			power_ = other.power_;
			equipments_ = other.equipments_.Clone();
			perks_ = other.perks_.Clone();
		}

		[DebuggerNonUserCode]
		public Warrior Clone()
		{
			return new Warrior(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Warrior);
		}

		[DebuggerNonUserCode]
		public bool Equals(Warrior other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (Alias != other.Alias)
			{
				return false;
			}
			if (Gender != other.Gender)
			{
				return false;
			}
			if (AppearanceId != other.AppearanceId)
			{
				return false;
			}
			if (AiMode != other.AiMode)
			{
				return false;
			}
			if (Power != other.Power)
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
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (Alias.Length != 0)
			{
				num ^= Alias.GetHashCode();
			}
			if (Gender != 0)
			{
				num ^= Gender.GetHashCode();
			}
			if (AppearanceId != 0)
			{
				num ^= AppearanceId.GetHashCode();
			}
			if (AiMode != 0)
			{
				num ^= AiMode.GetHashCode();
			}
			if (Power != 0.0)
			{
				num ^= Power.GetHashCode();
			}
			num ^= equipments_.GetHashCode();
			return num ^ perks_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (Alias.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(Alias);
			}
			if (Gender != 0)
			{
				output.WriteRawTag(16);
				output.WriteEnum((int)Gender);
			}
			if (AppearanceId != 0)
			{
				output.WriteRawTag(24);
				output.WriteInt32(AppearanceId);
			}
			if (AiMode != 0)
			{
				output.WriteRawTag(32);
				output.WriteEnum((int)AiMode);
			}
			if (Power != 0.0)
			{
				output.WriteRawTag(41);
				output.WriteDouble(Power);
			}
			equipments_.WriteTo(output, _repeated_equipments_codec);
			perks_.WriteTo(output, _repeated_perks_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Alias.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Alias);
			}
			if (Gender != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)Gender);
			}
			if (AppearanceId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(AppearanceId);
			}
			if (AiMode != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)AiMode);
			}
			if (Power != 0.0)
			{
				num += 9;
			}
			num += equipments_.CalculateSize(_repeated_equipments_codec);
			return num + perks_.CalculateSize(_repeated_perks_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Warrior other)
		{
			if (other != null)
			{
				if (other.Alias.Length != 0)
				{
					Alias = other.Alias;
				}
				if (other.Gender != 0)
				{
					Gender = other.Gender;
				}
				if (other.AppearanceId != 0)
				{
					AppearanceId = other.AppearanceId;
				}
				if (other.AiMode != 0)
				{
					AiMode = other.AiMode;
				}
				if (other.Power != 0.0)
				{
					Power = other.Power;
				}
				equipments_.Add(other.equipments_);
				perks_.Add(other.perks_);
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
					Alias = input.ReadString();
					break;
				case 16u:
					gender_ = (Gender)input.ReadEnum();
					break;
				case 24u:
					AppearanceId = input.ReadInt32();
					break;
				case 32u:
					aiMode_ = (AiMode)input.ReadEnum();
					break;
				case 41u:
					Power = input.ReadDouble();
					break;
				case 50u:
					equipments_.AddEntriesFrom(input, _repeated_equipments_codec);
					break;
				case 58u:
					perks_.AddEntriesFrom(input, _repeated_perks_codec);
					break;
				}
			}
		}
	}
}
