using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class LogData : IMessage<LogData>, IMessage, IEquatable<LogData>, IDeepCloneable<LogData>
	{
		private static readonly MessageParser<LogData> _parser = new MessageParser<LogData>(() => new LogData());

		public const int AnalyticsLogLevelFieldNumber = 1;

		private int analyticsLogLevel_;

		public const int ClientLogIdFieldNumber = 2;

		private long clientLogId_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<LogData> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[11];
			}
		}

		[DebuggerNonUserCode]
		public int AnalyticsLogLevel
		{
			get
			{
				return analyticsLogLevel_;
			}
			set
			{
				analyticsLogLevel_ = value;
			}
		}

		[DebuggerNonUserCode]
		public long ClientLogId
		{
			get
			{
				return clientLogId_;
			}
			set
			{
				clientLogId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public LogData()
		{
		}

		[DebuggerNonUserCode]
		public LogData(LogData other)
			: this()
		{
			analyticsLogLevel_ = other.analyticsLogLevel_;
			clientLogId_ = other.clientLogId_;
		}

		[DebuggerNonUserCode]
		public LogData Clone()
		{
			return new LogData(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as LogData);
		}

		[DebuggerNonUserCode]
		public bool Equals(LogData other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (AnalyticsLogLevel != other.AnalyticsLogLevel)
			{
				return false;
			}
			if (ClientLogId != other.ClientLogId)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (AnalyticsLogLevel != 0)
			{
				num ^= AnalyticsLogLevel.GetHashCode();
			}
			if (ClientLogId != 0)
			{
				num ^= ClientLogId.GetHashCode();
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
			if (AnalyticsLogLevel != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(AnalyticsLogLevel);
			}
			if (ClientLogId != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt64(ClientLogId);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (AnalyticsLogLevel != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(AnalyticsLogLevel);
			}
			if (ClientLogId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(ClientLogId);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(LogData other)
		{
			if (other != null)
			{
				if (other.AnalyticsLogLevel != 0)
				{
					AnalyticsLogLevel = other.AnalyticsLogLevel;
				}
				if (other.ClientLogId != 0)
				{
					ClientLogId = other.ClientLogId;
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
					AnalyticsLogLevel = input.ReadInt32();
					break;
				case 16u:
					ClientLogId = input.ReadInt64();
					break;
				}
			}
		}
	}
}
