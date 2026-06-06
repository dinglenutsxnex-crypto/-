using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class ShortPlayers : IMessage<ShortPlayers>, IMessage, IEquatable<ShortPlayers>, IDeepCloneable<ShortPlayers>
	{
		private static readonly MessageParser<ShortPlayers> _parser = new MessageParser<ShortPlayers>(() => new ShortPlayers());

		public const int ValuesFieldNumber = 1;

		private static readonly FieldCodec<ShortPlayer> _repeated_values_codec = FieldCodec.ForMessage(10u, ShortPlayer.Parser);

		private readonly RepeatedField<ShortPlayer> values_ = new RepeatedField<ShortPlayer>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<ShortPlayers> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[16];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<ShortPlayer> Values
		{
			get
			{
				return values_;
			}
		}

		[DebuggerNonUserCode]
		public ShortPlayers()
		{
		}

		[DebuggerNonUserCode]
		public ShortPlayers(ShortPlayers other)
			: this()
		{
			values_ = other.values_.Clone();
		}

		[DebuggerNonUserCode]
		public ShortPlayers Clone()
		{
			return new ShortPlayers(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as ShortPlayers);
		}

		[DebuggerNonUserCode]
		public bool Equals(ShortPlayers other)
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
		public void MergeFrom(ShortPlayers other)
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
				if (num != 10)
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
