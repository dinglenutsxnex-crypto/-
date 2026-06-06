using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class PerkSlot : IMessage<PerkSlot>, IMessage, IEquatable<PerkSlot>, IDeepCloneable<PerkSlot>
	{
		private static readonly MessageParser<PerkSlot> _parser = new MessageParser<PerkSlot>(() => new PerkSlot());

		public const int SlotIndexFieldNumber = 1;

		private int slotIndex_;

		public const int PerkModelIdFieldNumber = 2;

		private int perkModelId_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<PerkSlot> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[8];
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
		public PerkSlot()
		{
		}

		[DebuggerNonUserCode]
		public PerkSlot(PerkSlot other)
			: this()
		{
			slotIndex_ = other.slotIndex_;
			perkModelId_ = other.perkModelId_;
		}

		[DebuggerNonUserCode]
		public PerkSlot Clone()
		{
			return new PerkSlot(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as PerkSlot);
		}

		[DebuggerNonUserCode]
		public bool Equals(PerkSlot other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (SlotIndex != other.SlotIndex)
			{
				return false;
			}
			if (PerkModelId != other.PerkModelId)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (SlotIndex != 0)
			{
				num ^= SlotIndex.GetHashCode();
			}
			if (PerkModelId != 0)
			{
				num ^= PerkModelId.GetHashCode();
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
			if (SlotIndex != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(SlotIndex);
			}
			if (PerkModelId != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt32(PerkModelId);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (SlotIndex != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(SlotIndex);
			}
			if (PerkModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(PerkModelId);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(PerkSlot other)
		{
			if (other != null)
			{
				if (other.SlotIndex != 0)
				{
					SlotIndex = other.SlotIndex;
				}
				if (other.PerkModelId != 0)
				{
					PerkModelId = other.PerkModelId;
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
					SlotIndex = input.ReadInt32();
					break;
				case 16u:
					PerkModelId = input.ReadInt32();
					break;
				}
			}
		}
	}
}
