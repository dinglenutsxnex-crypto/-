using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class InsertPerkRequest : IMessage<InsertPerkRequest>, IMessage, IEquatable<InsertPerkRequest>, IDeepCloneable<InsertPerkRequest>
	{
		private static readonly MessageParser<InsertPerkRequest> _parser = new MessageParser<InsertPerkRequest>(() => new InsertPerkRequest());

		public const int ItemModelIdFieldNumber = 1;

		private int itemModelId_;

		public const int SlotIndexFieldNumber = 2;

		private int slotIndex_;

		public const int PerkModelIdFieldNumber = 3;

		private int perkModelId_;

		public const int InsertFieldNumber = 4;

		private bool insert_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<InsertPerkRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[14];
			}
		}

		[DebuggerNonUserCode]
		public int ItemModelId
		{
			get
			{
				return itemModelId_;
			}
			set
			{
				itemModelId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int SlotIndex
		{
			get
			{
				return slotIndex_;
			}
			set
			{
				slotIndex_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int PerkModelId
		{
			get
			{
				return perkModelId_;
			}
			set
			{
				perkModelId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool Insert
		{
			get
			{
				return insert_;
			}
			set
			{
				insert_ = value;
			}
		}

		[DebuggerNonUserCode]
		public InsertPerkRequest()
		{
		}

		[DebuggerNonUserCode]
		public InsertPerkRequest(InsertPerkRequest other)
			: this()
		{
			itemModelId_ = other.itemModelId_;
			slotIndex_ = other.slotIndex_;
			perkModelId_ = other.perkModelId_;
			insert_ = other.insert_;
		}

		[DebuggerNonUserCode]
		public InsertPerkRequest Clone()
		{
			return new InsertPerkRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as InsertPerkRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(InsertPerkRequest other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (ItemModelId != other.ItemModelId)
			{
				return false;
			}
			if (SlotIndex != other.SlotIndex)
			{
				return false;
			}
			if (PerkModelId != other.PerkModelId)
			{
				return false;
			}
			if (Insert != other.Insert)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (ItemModelId != 0)
			{
				num ^= ItemModelId.GetHashCode();
			}
			if (SlotIndex != 0)
			{
				num ^= SlotIndex.GetHashCode();
			}
			if (PerkModelId != 0)
			{
				num ^= PerkModelId.GetHashCode();
			}
			if (Insert)
			{
				num ^= Insert.GetHashCode();
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
			if (ItemModelId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(ItemModelId);
			}
			if (SlotIndex != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt32(SlotIndex);
			}
			if (PerkModelId != 0)
			{
				output.WriteRawTag(24);
				output.WriteInt32(PerkModelId);
			}
			if (Insert)
			{
				output.WriteRawTag(32);
				output.WriteBool(Insert);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (ItemModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ItemModelId);
			}
			if (SlotIndex != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(SlotIndex);
			}
			if (PerkModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(PerkModelId);
			}
			if (Insert)
			{
				num += 2;
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(InsertPerkRequest other)
		{
			if (other != null)
			{
				if (other.ItemModelId != 0)
				{
					ItemModelId = other.ItemModelId;
				}
				if (other.SlotIndex != 0)
				{
					SlotIndex = other.SlotIndex;
				}
				if (other.PerkModelId != 0)
				{
					PerkModelId = other.PerkModelId;
				}
				if (other.Insert)
				{
					Insert = other.Insert;
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
					ItemModelId = input.ReadInt32();
					break;
				case 16u:
					SlotIndex = input.ReadInt32();
					break;
				case 24u:
					PerkModelId = input.ReadInt32();
					break;
				case 32u:
					Insert = input.ReadBool();
					break;
				}
			}
		}
	}
}
