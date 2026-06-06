using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class MutableOfflineState : IMessage<MutableOfflineState>, IMessage, IEquatable<MutableOfflineState>, IDeepCloneable<MutableOfflineState>
	{
		private static readonly MessageParser<MutableOfflineState> _parser = new MessageParser<MutableOfflineState>(() => new MutableOfflineState());

		public const int StateIdFieldNumber = 1;

		private long stateId_;

		public const int ExperienceFieldNumber = 2;

		private long experience_;

		public const int LevelFieldNumber = 3;

		private int level_;

		public const int CurrenciesFieldNumber = 4;

		private static readonly FieldCodec<Currency> _repeated_currencies_codec = FieldCodec.ForMessage(34u, Currency.Parser);

		private readonly RepeatedField<Currency> currencies_ = new RepeatedField<Currency>();

		public const int InventoryFieldNumber = 5;

		private Inventory inventory_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<MutableOfflineState> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[40];
			}
		}

		[DebuggerNonUserCode]
		public long StateId
		{
			get
			{
				return stateId_;
			}
			set
			{
				stateId_ = value;
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
		public int Level
		{
			get
			{
				return level_;
			}
			set
			{
				level_ = value;
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
		public Inventory Inventory
		{
			get
			{
				return inventory_;
			}
			set
			{
				inventory_ = value;
			}
		}

		[DebuggerNonUserCode]
		public MutableOfflineState()
		{
		}

		[DebuggerNonUserCode]
		public MutableOfflineState(MutableOfflineState other)
			: this()
		{
			stateId_ = other.stateId_;
			experience_ = other.experience_;
			level_ = other.level_;
			currencies_ = other.currencies_.Clone();
			Inventory = ((other.inventory_ == null) ? null : other.Inventory.Clone());
		}

		[DebuggerNonUserCode]
		public MutableOfflineState Clone()
		{
			return new MutableOfflineState(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as MutableOfflineState);
		}

		[DebuggerNonUserCode]
		public bool Equals(MutableOfflineState other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (StateId != other.StateId)
			{
				return false;
			}
			if (Experience != other.Experience)
			{
				return false;
			}
			if (Level != other.Level)
			{
				return false;
			}
			if (!currencies_.Equals(other.currencies_))
			{
				return false;
			}
			if (!object.Equals(Inventory, other.Inventory))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (StateId != 0)
			{
				num ^= StateId.GetHashCode();
			}
			if (Experience != 0)
			{
				num ^= Experience.GetHashCode();
			}
			if (Level != 0)
			{
				num ^= Level.GetHashCode();
			}
			num ^= currencies_.GetHashCode();
			if (inventory_ != null)
			{
				num ^= Inventory.GetHashCode();
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
			if (StateId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt64(StateId);
			}
			if (Experience != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt64(Experience);
			}
			if (Level != 0)
			{
				output.WriteRawTag(24);
				output.WriteInt32(Level);
			}
			currencies_.WriteTo(output, _repeated_currencies_codec);
			if (inventory_ != null)
			{
				output.WriteRawTag(42);
				output.WriteMessage(Inventory);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (StateId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(StateId);
			}
			if (Experience != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(Experience);
			}
			if (Level != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(Level);
			}
			num += currencies_.CalculateSize(_repeated_currencies_codec);
			if (inventory_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Inventory);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(MutableOfflineState other)
		{
			if (other == null)
			{
				return;
			}
			if (other.StateId != 0)
			{
				StateId = other.StateId;
			}
			if (other.Experience != 0)
			{
				Experience = other.Experience;
			}
			if (other.Level != 0)
			{
				Level = other.Level;
			}
			currencies_.Add(other.currencies_);
			if (other.inventory_ != null)
			{
				if (inventory_ == null)
				{
					inventory_ = new Inventory();
				}
				Inventory.MergeFrom(other.Inventory);
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
					StateId = input.ReadInt64();
					break;
				case 16u:
					Experience = input.ReadInt64();
					break;
				case 24u:
					Level = input.ReadInt32();
					break;
				case 34u:
					currencies_.AddEntriesFrom(input, _repeated_currencies_codec);
					break;
				case 42u:
					if (inventory_ == null)
					{
						inventory_ = new Inventory();
					}
					input.ReadMessage(inventory_);
					break;
				}
			}
		}
	}
}
