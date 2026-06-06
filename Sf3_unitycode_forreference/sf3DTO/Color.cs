using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Color : IMessage<Color>, IMessage, IEquatable<Color>, IDeepCloneable<Color>
	{
		private static readonly MessageParser<Color> _parser = new MessageParser<Color>(() => new Color());

		public const int ColorIdFieldNumber = 1;

		private int colorId_;

		public const int ValueFieldNumber = 2;

		private double value_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Color> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[3];
			}
		}

		[DebuggerNonUserCode]
		public int ColorId
		{
			get
			{
				return colorId_;
			}
			set
			{
				colorId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public double Value
		{
			get
			{
				return value_;
			}
			set
			{
				value_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Color()
		{
		}

		[DebuggerNonUserCode]
		public Color(Color other)
			: this()
		{
			colorId_ = other.colorId_;
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public Color Clone()
		{
			return new Color(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Color);
		}

		[DebuggerNonUserCode]
		public bool Equals(Color other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (ColorId != other.ColorId)
			{
				return false;
			}
			if (Value != other.Value)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (ColorId != 0)
			{
				num ^= ColorId.GetHashCode();
			}
			if (Value != 0.0)
			{
				num ^= Value.GetHashCode();
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
			if (ColorId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(ColorId);
			}
			if (Value != 0.0)
			{
				output.WriteRawTag(17);
				output.WriteDouble(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (ColorId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ColorId);
			}
			if (Value != 0.0)
			{
				num += 9;
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Color other)
		{
			if (other != null)
			{
				if (other.ColorId != 0)
				{
					ColorId = other.ColorId;
				}
				if (other.Value != 0.0)
				{
					Value = other.Value;
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
					ColorId = input.ReadInt32();
					break;
				case 17u:
					Value = input.ReadDouble();
					break;
				}
			}
		}
	}
}
