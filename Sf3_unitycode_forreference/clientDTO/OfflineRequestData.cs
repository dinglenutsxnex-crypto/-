using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace clientDTO
{
	public sealed class OfflineRequestData : IMessage<OfflineRequestData>, IMessage, IEquatable<OfflineRequestData>, IDeepCloneable<OfflineRequestData>
	{
		private static readonly MessageParser<OfflineRequestData> _parser = new MessageParser<OfflineRequestData>(() => new OfflineRequestData());

		public const int RequestIDFieldNumber = 1;

		private long requestID_;

		public const int CmdFieldNumber = 2;

		private string cmd_ = string.Empty;

		public const int VersionFullFieldNumber = 3;

		private string versionFull_ = string.Empty;

		public const int BinaryFieldNumber = 4;

		private ByteString binary_ = ByteString.Empty;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<OfflineRequestData> Parser
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
				return ClientReflection.Descriptor.MessageTypes[0];
			}
		}

		[DebuggerNonUserCode]
		public long RequestID
		{
			get
			{
				return requestID_;
			}
			set
			{
				requestID_ = value;
			}
		}

		[DebuggerNonUserCode]
		public string Cmd
		{
			get
			{
				return cmd_;
			}
			set
			{
				cmd_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public string VersionFull
		{
			get
			{
				return versionFull_;
			}
			set
			{
				versionFull_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public ByteString Binary
		{
			get
			{
				return binary_;
			}
			set
			{
				binary_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public OfflineRequestData()
		{
		}

		[DebuggerNonUserCode]
		public OfflineRequestData(OfflineRequestData other)
			: this()
		{
			requestID_ = other.requestID_;
			cmd_ = other.cmd_;
			versionFull_ = other.versionFull_;
			binary_ = other.binary_;
		}

		[DebuggerNonUserCode]
		public OfflineRequestData Clone()
		{
			return new OfflineRequestData(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as OfflineRequestData);
		}

		[DebuggerNonUserCode]
		public bool Equals(OfflineRequestData other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (RequestID != other.RequestID)
			{
				return false;
			}
			if (Cmd != other.Cmd)
			{
				return false;
			}
			if (VersionFull != other.VersionFull)
			{
				return false;
			}
			if (Binary != other.Binary)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (RequestID != 0)
			{
				num ^= RequestID.GetHashCode();
			}
			if (Cmd.Length != 0)
			{
				num ^= Cmd.GetHashCode();
			}
			if (VersionFull.Length != 0)
			{
				num ^= VersionFull.GetHashCode();
			}
			if (Binary.Length != 0)
			{
				num ^= Binary.GetHashCode();
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
			if (RequestID != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt64(RequestID);
			}
			if (Cmd.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteString(Cmd);
			}
			if (VersionFull.Length != 0)
			{
				output.WriteRawTag(26);
				output.WriteString(VersionFull);
			}
			if (Binary.Length != 0)
			{
				output.WriteRawTag(34);
				output.WriteBytes(Binary);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (RequestID != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(RequestID);
			}
			if (Cmd.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Cmd);
			}
			if (VersionFull.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(VersionFull);
			}
			if (Binary.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeBytesSize(Binary);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(OfflineRequestData other)
		{
			if (other != null)
			{
				if (other.RequestID != 0)
				{
					RequestID = other.RequestID;
				}
				if (other.Cmd.Length != 0)
				{
					Cmd = other.Cmd;
				}
				if (other.VersionFull.Length != 0)
				{
					VersionFull = other.VersionFull;
				}
				if (other.Binary.Length != 0)
				{
					Binary = other.Binary;
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
					RequestID = input.ReadInt64();
					break;
				case 18u:
					Cmd = input.ReadString();
					break;
				case 26u:
					VersionFull = input.ReadString();
					break;
				case 34u:
					Binary = input.ReadBytes();
					break;
				}
			}
		}
	}
}
