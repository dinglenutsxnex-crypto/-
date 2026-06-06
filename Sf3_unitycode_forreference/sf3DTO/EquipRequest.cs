using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class EquipRequest : IMessage<EquipRequest>, IMessage, IEquatable<EquipRequest>, IDeepCloneable<EquipRequest>
	{
		private static readonly MessageParser<EquipRequest> _parser = new MessageParser<EquipRequest>(() => new EquipRequest());

		public const int ModelIdFieldNumber = 1;

		private int modelId_;

		public const int EquipFieldNumber = 2;

		private bool equip_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<EquipRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[13];
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
		public bool Equip
		{
			get
			{
				return equip_;
			}
			set
			{
				equip_ = value;
			}
		}

		[DebuggerNonUserCode]
		public EquipRequest()
		{
		}

		[DebuggerNonUserCode]
		public EquipRequest(EquipRequest other)
			: this()
		{
			modelId_ = other.modelId_;
			equip_ = other.equip_;
		}

		[DebuggerNonUserCode]
		public EquipRequest Clone()
		{
			return new EquipRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as EquipRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(EquipRequest other)
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
			if (Equip != other.Equip)
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
			if (Equip)
			{
				num ^= Equip.GetHashCode();
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
			if (ModelId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(ModelId);
			}
			if (Equip)
			{
				output.WriteRawTag(16);
				output.WriteBool(Equip);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (ModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ModelId);
			}
			if (Equip)
			{
				num += 2;
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(EquipRequest other)
		{
			if (other != null)
			{
				if (other.ModelId != 0)
				{
					ModelId = other.ModelId;
				}
				if (other.Equip)
				{
					Equip = other.Equip;
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
					ModelId = input.ReadInt32();
					break;
				case 16u:
					Equip = input.ReadBool();
					break;
				}
			}
		}
	}
}
