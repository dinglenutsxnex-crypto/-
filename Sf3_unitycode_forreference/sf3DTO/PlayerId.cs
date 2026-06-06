using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class PlayerId : IMessage<PlayerId>, IMessage, IEquatable<PlayerId>, IDeepCloneable<PlayerId>
	{
		private static readonly MessageParser<PlayerId> _parser = new MessageParser<PlayerId>(() => new PlayerId());

		public const int ValueFieldNumber = 1;

		private long value_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<PlayerId> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[20];
			}
		}

		[DebuggerNonUserCode]
		public long Value
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
		public PlayerId()
		{
		}

		[DebuggerNonUserCode]
		public PlayerId(PlayerId other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public PlayerId Clone()
		{
			return new PlayerId(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as PlayerId);
		}

		[DebuggerNonUserCode]
		public bool Equals(PlayerId other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
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
			if (Value != 0)
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
			if (Value != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt64(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(Value);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(PlayerId other)
		{
			if (other != null && other.Value != 0)
			{
				Value = other.Value;
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
					Value = input.ReadInt64();
				}
			}
		}
	}
}
