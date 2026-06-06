using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace common
{
	public sealed class PingResponse : IMessage<PingResponse>, IMessage, IEquatable<PingResponse>, IDeepCloneable<PingResponse>
	{
		private static readonly MessageParser<PingResponse> _parser = new MessageParser<PingResponse>(() => new PingResponse());

		public const int ClientTimestampFieldNumber = 1;

		private Timestamp clientTimestamp_;

		public const int ServerTimestampFieldNumber = 2;

		private Timestamp serverTimestamp_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<PingResponse> Parser
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
				return CommonReflection.Descriptor.MessageTypes[5];
			}
		}

		[DebuggerNonUserCode]
		public Timestamp ClientTimestamp
		{
			get
			{
				return clientTimestamp_;
			}
			set
			{
				clientTimestamp_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Timestamp ServerTimestamp
		{
			get
			{
				return serverTimestamp_;
			}
			set
			{
				serverTimestamp_ = value;
			}
		}

		[DebuggerNonUserCode]
		public PingResponse()
		{
		}

		[DebuggerNonUserCode]
		public PingResponse(PingResponse other)
			: this()
		{
			ClientTimestamp = ((other.clientTimestamp_ == null) ? null : other.ClientTimestamp.Clone());
			ServerTimestamp = ((other.serverTimestamp_ == null) ? null : other.ServerTimestamp.Clone());
		}

		[DebuggerNonUserCode]
		public PingResponse Clone()
		{
			return new PingResponse(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as PingResponse);
		}

		[DebuggerNonUserCode]
		public bool Equals(PingResponse other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!object.Equals(ClientTimestamp, other.ClientTimestamp))
			{
				return false;
			}
			if (!object.Equals(ServerTimestamp, other.ServerTimestamp))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (clientTimestamp_ != null)
			{
				num ^= ClientTimestamp.GetHashCode();
			}
			if (serverTimestamp_ != null)
			{
				num ^= ServerTimestamp.GetHashCode();
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
			if (clientTimestamp_ != null)
			{
				output.WriteRawTag(10);
				output.WriteMessage(ClientTimestamp);
			}
			if (serverTimestamp_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(ServerTimestamp);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (clientTimestamp_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(ClientTimestamp);
			}
			if (serverTimestamp_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(ServerTimestamp);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(PingResponse other)
		{
			if (other == null)
			{
				return;
			}
			if (other.clientTimestamp_ != null)
			{
				if (clientTimestamp_ == null)
				{
					clientTimestamp_ = new Timestamp();
				}
				ClientTimestamp.MergeFrom(other.ClientTimestamp);
			}
			if (other.serverTimestamp_ != null)
			{
				if (serverTimestamp_ == null)
				{
					serverTimestamp_ = new Timestamp();
				}
				ServerTimestamp.MergeFrom(other.ServerTimestamp);
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
				case 10u:
					if (clientTimestamp_ == null)
					{
						clientTimestamp_ = new Timestamp();
					}
					input.ReadMessage(clientTimestamp_);
					break;
				case 18u:
					if (serverTimestamp_ == null)
					{
						serverTimestamp_ = new Timestamp();
					}
					input.ReadMessage(serverTimestamp_);
					break;
				}
			}
		}
	}
}
