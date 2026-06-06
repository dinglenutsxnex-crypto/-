using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class GetPlayerRequest : IMessage<GetPlayerRequest>, IMessage, IEquatable<GetPlayerRequest>, IDeepCloneable<GetPlayerRequest>
	{
		private static readonly MessageParser<GetPlayerRequest> _parser = new MessageParser<GetPlayerRequest>(() => new GetPlayerRequest());

		public const int VersionFieldNumber = 1;

		private string version_ = string.Empty;

		public const int OfflineRequestBatchFieldNumber = 2;

		private OfflineRequestBatch offlineRequestBatch_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<GetPlayerRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[23];
			}
		}

		[DebuggerNonUserCode]
		public string Version
		{
			get
			{
				return version_;
			}
			set
			{
				version_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public OfflineRequestBatch OfflineRequestBatch
		{
			get
			{
				return offlineRequestBatch_;
			}
			set
			{
				offlineRequestBatch_ = value;
			}
		}

		[DebuggerNonUserCode]
		public GetPlayerRequest()
		{
		}

		[DebuggerNonUserCode]
		public GetPlayerRequest(GetPlayerRequest other)
			: this()
		{
			version_ = other.version_;
			OfflineRequestBatch = ((other.offlineRequestBatch_ == null) ? null : other.OfflineRequestBatch.Clone());
		}

		[DebuggerNonUserCode]
		public GetPlayerRequest Clone()
		{
			return new GetPlayerRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as GetPlayerRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(GetPlayerRequest other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (Version != other.Version)
			{
				return false;
			}
			if (!object.Equals(OfflineRequestBatch, other.OfflineRequestBatch))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (Version.Length != 0)
			{
				num ^= Version.GetHashCode();
			}
			if (offlineRequestBatch_ != null)
			{
				num ^= OfflineRequestBatch.GetHashCode();
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
			if (Version.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(Version);
			}
			if (offlineRequestBatch_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(OfflineRequestBatch);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Version.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Version);
			}
			if (offlineRequestBatch_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(OfflineRequestBatch);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(GetPlayerRequest other)
		{
			if (other == null)
			{
				return;
			}
			if (other.Version.Length != 0)
			{
				Version = other.Version;
			}
			if (other.offlineRequestBatch_ != null)
			{
				if (offlineRequestBatch_ == null)
				{
					offlineRequestBatch_ = new OfflineRequestBatch();
				}
				OfflineRequestBatch.MergeFrom(other.OfflineRequestBatch);
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
					Version = input.ReadString();
					break;
				case 18u:
					if (offlineRequestBatch_ == null)
					{
						offlineRequestBatch_ = new OfflineRequestBatch();
					}
					input.ReadMessage(offlineRequestBatch_);
					break;
				}
			}
		}
	}
}
