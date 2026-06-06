using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Item : IMessage<Item>, IMessage, IEquatable<Item>, IDeepCloneable<Item>
	{
		private static readonly MessageParser<Item> _parser = new MessageParser<Item>(() => new Item());

		public const int ModelIdFieldNumber = 1;

		private int modelId_;

		public const int StackLevelFieldNumber = 2;

		private double stackLevel_;

		public const int EquippedFieldNumber = 3;

		private bool equipped_;

		public const int PerksFieldNumber = 4;

		private static readonly FieldCodec<PerkSlot> _repeated_perks_codec = FieldCodec.ForMessage(34u, PerkSlot.Parser);

		private readonly RepeatedField<PerkSlot> perks_ = new RepeatedField<PerkSlot>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Item> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[9];
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
		public bool Equipped
		{
			get
			{
				return equipped_;
			}
			set
			{
				equipped_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<PerkSlot> Perks
		{
			get
			{
				return perks_;
			}
		}

		[DebuggerNonUserCode]
		public Item()
		{
		}

		[DebuggerNonUserCode]
		public Item(Item other)
			: this()
		{
			modelId_ = other.modelId_;
			stackLevel_ = other.stackLevel_;
			equipped_ = other.equipped_;
			perks_ = other.perks_.Clone();
		}

		[DebuggerNonUserCode]
		public Item Clone()
		{
			return new Item(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Item);
		}

		[DebuggerNonUserCode]
		public bool Equals(Item other)
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
			if (Equipped != other.Equipped)
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
			if (ModelId != 0)
			{
				num ^= ModelId.GetHashCode();
			}
			if (StackLevel != 0.0)
			{
				num ^= StackLevel.GetHashCode();
			}
			if (Equipped)
			{
				num ^= Equipped.GetHashCode();
			}
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
			if (Equipped)
			{
				output.WriteRawTag(24);
				output.WriteBool(Equipped);
			}
			perks_.WriteTo(output, _repeated_perks_codec);
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
			if (Equipped)
			{
				num += 2;
			}
			return num + perks_.CalculateSize(_repeated_perks_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Item other)
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
				if (other.Equipped)
				{
					Equipped = other.Equipped;
				}
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
				case 8u:
					ModelId = input.ReadInt32();
					break;
				case 17u:
					StackLevel = input.ReadDouble();
					break;
				case 24u:
					Equipped = input.ReadBool();
					break;
				case 34u:
					perks_.AddEntriesFrom(input, _repeated_perks_codec);
					break;
				}
			}
		}
	}
}
