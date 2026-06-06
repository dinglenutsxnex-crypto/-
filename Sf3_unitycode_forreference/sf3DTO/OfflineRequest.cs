using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class OfflineRequest : IMessage<OfflineRequest>, IMessage, IEquatable<OfflineRequest>, IDeepCloneable<OfflineRequest>
	{
		private static readonly MessageParser<OfflineRequest> _parser = new MessageParser<OfflineRequest>(() => new OfflineRequest());

		public const int NewStateIdFieldNumber = 1;

		private long newStateId_;

		public const int CmdFieldNumber = 2;

		private string cmd_ = string.Empty;

		public const int ConfigVersionFieldNumber = 3;

		private string configVersion_ = string.Empty;

		public const int DataFieldNumber = 4;

		private ByteString data_ = ByteString.Empty;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<OfflineRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[41];
			}
		}

		[DebuggerNonUserCode]
		public long NewStateId
		{
			get
			{
				return newStateId_;
			}
			set
			{
				newStateId_ = value;
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
		public string ConfigVersion
		{
			get
			{
				return configVersion_;
			}
			set
			{
				configVersion_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public ByteString Data
		{
			get
			{
				return data_;
			}
			set
			{
				data_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public OfflineRequest()
		{
		}

		[DebuggerNonUserCode]
		public OfflineRequest(OfflineRequest other)
			: this()
		{
			newStateId_ = other.newStateId_;
			cmd_ = other.cmd_;
			configVersion_ = other.configVersion_;
			data_ = other.data_;
		}

		[DebuggerNonUserCode]
		public OfflineRequest Clone()
		{
			return new OfflineRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as OfflineRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(OfflineRequest other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (NewStateId != other.NewStateId)
			{
				return false;
			}
			if (Cmd != other.Cmd)
			{
				return false;
			}
			if (ConfigVersion != other.ConfigVersion)
			{
				return false;
			}
			if (Data != other.Data)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (NewStateId != 0)
			{
				num ^= NewStateId.GetHashCode();
			}
			if (Cmd.Length != 0)
			{
				num ^= Cmd.GetHashCode();
			}
			if (ConfigVersion.Length != 0)
			{
				num ^= ConfigVersion.GetHashCode();
			}
			if (Data.Length != 0)
			{
				num ^= Data.GetHashCode();
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
			if (NewStateId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt64(NewStateId);
			}
			if (Cmd.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteString(Cmd);
			}
			if (ConfigVersion.Length != 0)
			{
				output.WriteRawTag(26);
				output.WriteString(ConfigVersion);
			}
			if (Data.Length != 0)
			{
				output.WriteRawTag(34);
				output.WriteBytes(Data);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (NewStateId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(NewStateId);
			}
			if (Cmd.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Cmd);
			}
			if (ConfigVersion.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(ConfigVersion);
			}
			if (Data.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeBytesSize(Data);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(OfflineRequest other)
		{
			if (other != null)
			{
				if (other.NewStateId != 0)
				{
					NewStateId = other.NewStateId;
				}
				if (other.Cmd.Length != 0)
				{
					Cmd = other.Cmd;
				}
				if (other.ConfigVersion.Length != 0)
				{
					ConfigVersion = other.ConfigVersion;
				}
				if (other.Data.Length != 0)
				{
					Data = other.Data;
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
					NewStateId = input.ReadInt64();
					break;
				case 18u:
					Cmd = input.ReadString();
					break;
				case 26u:
					ConfigVersion = input.ReadString();
					break;
				case 34u:
					Data = input.ReadBytes();
					break;
				}
			}
		}
	}
}
