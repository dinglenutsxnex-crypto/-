using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class LogRequest : IMessage<LogRequest>, IMessage, IEquatable<LogRequest>, IDeepCloneable<LogRequest>
	{
		private static readonly MessageParser<LogRequest> _parser = new MessageParser<LogRequest>(() => new LogRequest());

		public const int EventsFieldNumber = 1;

		private static readonly FieldCodec<string> _repeated_events_codec = FieldCodec.ForString(10u);

		private readonly RepeatedField<string> events_ = new RepeatedField<string>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<LogRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[43];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<string> Events
		{
			get
			{
				return events_;
			}
		}

		[DebuggerNonUserCode]
		public LogRequest()
		{
		}

		[DebuggerNonUserCode]
		public LogRequest(LogRequest other)
			: this()
		{
			events_ = other.events_.Clone();
		}

		[DebuggerNonUserCode]
		public LogRequest Clone()
		{
			return new LogRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as LogRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(LogRequest other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!events_.Equals(other.events_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			return num ^ events_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			events_.WriteTo(output, _repeated_events_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			return num + events_.CalculateSize(_repeated_events_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(LogRequest other)
		{
			if (other != null)
			{
				events_.Add(other.events_);
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
					events_.AddEntriesFrom(input, _repeated_events_codec);
				}
			}
		}
	}
}
