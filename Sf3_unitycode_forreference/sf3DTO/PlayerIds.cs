using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class PlayerIds : IMessage<PlayerIds>, IMessage, IEquatable<PlayerIds>, IDeepCloneable<PlayerIds>
	{
		private static readonly MessageParser<PlayerIds> _parser = new MessageParser<PlayerIds>(() => new PlayerIds());

		public const int ValuesFieldNumber = 1;

		private static readonly FieldCodec<long> _repeated_values_codec = FieldCodec.ForInt64(10u);

		private readonly RepeatedField<long> values_ = new RepeatedField<long>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<PlayerIds> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[21];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<long> Values
		{
			get
			{
				return values_;
			}
		}

		[DebuggerNonUserCode]
		public PlayerIds()
		{
		}

		[DebuggerNonUserCode]
		public PlayerIds(PlayerIds other)
			: this()
		{
			values_ = other.values_.Clone();
		}

		[DebuggerNonUserCode]
		public PlayerIds Clone()
		{
			return new PlayerIds(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as PlayerIds);
		}

		[DebuggerNonUserCode]
		public bool Equals(PlayerIds other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!values_.Equals(other.values_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			return num ^ values_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			values_.WriteTo(output, _repeated_values_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			return num + values_.CalculateSize(_repeated_values_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(PlayerIds other)
		{
			if (other != null)
			{
				values_.Add(other.values_);
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				if (num != 10 && num != 8)
				{
					input.SkipLastField();
				}
				else
				{
					values_.AddEntriesFrom(input, _repeated_values_codec);
				}
			}
		}
	}
}
