using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace common
{
	public sealed class UpdateConfigEvent : IMessage<UpdateConfigEvent>, IMessage, IEquatable<UpdateConfigEvent>, IDeepCloneable<UpdateConfigEvent>
	{
		private static readonly MessageParser<UpdateConfigEvent> _parser = new MessageParser<UpdateConfigEvent>(() => new UpdateConfigEvent());

		public const int ConfigSignFieldNumber = 1;

		private ByteString configSign_ = ByteString.Empty;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<UpdateConfigEvent> Parser
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
				return CommonReflection.Descriptor.MessageTypes[6];
			}
		}

		[DebuggerNonUserCode]
		public ByteString ConfigSign
		{
			get
			{
				return configSign_;
			}
			set
			{
				configSign_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public UpdateConfigEvent()
		{
		}

		[DebuggerNonUserCode]
		public UpdateConfigEvent(UpdateConfigEvent other)
			: this()
		{
			configSign_ = other.configSign_;
		}

		[DebuggerNonUserCode]
		public UpdateConfigEvent Clone()
		{
			return new UpdateConfigEvent(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as UpdateConfigEvent);
		}

		[DebuggerNonUserCode]
		public bool Equals(UpdateConfigEvent other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (ConfigSign != other.ConfigSign)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (ConfigSign.Length != 0)
			{
				num ^= ConfigSign.GetHashCode();
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
			if (ConfigSign.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteBytes(ConfigSign);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (ConfigSign.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeBytesSize(ConfigSign);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(UpdateConfigEvent other)
		{
			if (other != null && other.ConfigSign.Length != 0)
			{
				ConfigSign = other.ConfigSign;
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
					ConfigSign = input.ReadBytes();
				}
			}
		}
	}
}
