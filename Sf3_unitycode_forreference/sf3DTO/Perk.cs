using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Perk : IMessage<Perk>, IMessage, IEquatable<Perk>, IDeepCloneable<Perk>
	{
		private static readonly MessageParser<Perk> _parser = new MessageParser<Perk>(() => new Perk());

		public const int ModelIdFieldNumber = 1;

		private int modelId_;

		public const int StackLevelFieldNumber = 2;

		private double stackLevel_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Perk> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[7];
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
		public Perk()
		{
		}

		[DebuggerNonUserCode]
		public Perk(Perk other)
			: this()
		{
			modelId_ = other.modelId_;
			stackLevel_ = other.stackLevel_;
		}

		[DebuggerNonUserCode]
		public Perk Clone()
		{
			return new Perk(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Perk);
		}

		[DebuggerNonUserCode]
		public bool Equals(Perk other)
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
			if (StackLevel != 0.0)
			{
				output.WriteRawTag(17);
				output.WriteDouble(StackLevel);
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
			if (StackLevel != 0.0)
			{
				num += 9;
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Perk other)
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
				}
			}
		}
	}
}
