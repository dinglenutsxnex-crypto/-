using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class OpenBoosterRequest : IMessage<OpenBoosterRequest>, IMessage, IEquatable<OpenBoosterRequest>, IDeepCloneable<OpenBoosterRequest>
	{
		private static readonly MessageParser<OpenBoosterRequest> _parser = new MessageParser<OpenBoosterRequest>(() => new OpenBoosterRequest());

		public const int InstanceIdFieldNumber = 1;

		private long instanceId_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<OpenBoosterRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[36];
			}
		}

		[DebuggerNonUserCode]
		public long InstanceId
		{
			get
			{
				return instanceId_;
			}
			set
			{
				instanceId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public OpenBoosterRequest()
		{
		}

		[DebuggerNonUserCode]
		public OpenBoosterRequest(OpenBoosterRequest other)
			: this()
		{
			instanceId_ = other.instanceId_;
		}

		[DebuggerNonUserCode]
		public OpenBoosterRequest Clone()
		{
			return new OpenBoosterRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as OpenBoosterRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(OpenBoosterRequest other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (InstanceId != other.InstanceId)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (InstanceId != 0)
			{
				num ^= InstanceId.GetHashCode();
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
			if (InstanceId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt64(InstanceId);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (InstanceId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(InstanceId);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(OpenBoosterRequest other)
		{
			if (other != null && other.InstanceId != 0)
			{
				InstanceId = other.InstanceId;
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
					InstanceId = input.ReadInt64();
				}
			}
		}
	}
}
