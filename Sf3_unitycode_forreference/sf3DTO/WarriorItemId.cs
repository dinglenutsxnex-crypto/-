using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class WarriorItemId : IMessage<WarriorItemId>, IMessage, IEquatable<WarriorItemId>, IDeepCloneable<WarriorItemId>
	{
		private static readonly MessageParser<WarriorItemId> _parser = new MessageParser<WarriorItemId>(() => new WarriorItemId());

		public const int ModelIdFieldNumber = 1;

		private int modelId_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<WarriorItemId> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[25];
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
		public WarriorItemId()
		{
		}

		[DebuggerNonUserCode]
		public WarriorItemId(WarriorItemId other)
			: this()
		{
			modelId_ = other.modelId_;
		}

		[DebuggerNonUserCode]
		public WarriorItemId Clone()
		{
			return new WarriorItemId(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as WarriorItemId);
		}

		[DebuggerNonUserCode]
		public bool Equals(WarriorItemId other)
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
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (ModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ModelId);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(WarriorItemId other)
		{
			if (other != null && other.ModelId != 0)
			{
				ModelId = other.ModelId;
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				if (num != 8)
				{
					input.SkipLastField();
				}
				else
				{
					ModelId = input.ReadInt32();
				}
			}
		}
	}
}
