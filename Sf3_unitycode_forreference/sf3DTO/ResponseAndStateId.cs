using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class ResponseAndStateId : IMessage<ResponseAndStateId>, IMessage, IEquatable<ResponseAndStateId>, IDeepCloneable<ResponseAndStateId>
	{
		private static readonly MessageParser<ResponseAndStateId> _parser = new MessageParser<ResponseAndStateId>(() => new ResponseAndStateId());

		public const int OfflineStateIdFieldNumber = 1;

		private long offlineStateId_;

		public const int ResponseFieldNumber = 2;

		private ByteString response_ = ByteString.Empty;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<ResponseAndStateId> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[0];
			}
		}

		[DebuggerNonUserCode]
		public long OfflineStateId
		{
			get
			{
				return offlineStateId_;
			}
			set
			{
				offlineStateId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public ByteString Response
		{
			get
			{
				return response_;
			}
			set
			{
				response_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public ResponseAndStateId()
		{
		}

		[DebuggerNonUserCode]
		public ResponseAndStateId(ResponseAndStateId other)
			: this()
		{
			offlineStateId_ = other.offlineStateId_;
			response_ = other.response_;
		}

		[DebuggerNonUserCode]
		public ResponseAndStateId Clone()
		{
			return new ResponseAndStateId(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as ResponseAndStateId);
		}

		[DebuggerNonUserCode]
		public bool Equals(ResponseAndStateId other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (OfflineStateId != other.OfflineStateId)
			{
				return false;
			}
			if (Response != other.Response)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (OfflineStateId != 0)
			{
				num ^= OfflineStateId.GetHashCode();
			}
			if (Response.Length != 0)
			{
				num ^= Response.GetHashCode();
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
			if (OfflineStateId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt64(OfflineStateId);
			}
			if (Response.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteBytes(Response);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (OfflineStateId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(OfflineStateId);
			}
			if (Response.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeBytesSize(Response);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(ResponseAndStateId other)
		{
			if (other != null)
			{
				if (other.OfflineStateId != 0)
				{
					OfflineStateId = other.OfflineStateId;
				}
				if (other.Response.Length != 0)
				{
					Response = other.Response;
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
					OfflineStateId = input.ReadInt64();
					break;
				case 18u:
					Response = input.ReadBytes();
					break;
				}
			}
		}
	}
}
