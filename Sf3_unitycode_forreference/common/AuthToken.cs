using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace common
{
	public sealed class AuthToken : IMessage<AuthToken>, IMessage, IEquatable<AuthToken>, IDeepCloneable<AuthToken>
	{
		private static readonly MessageParser<AuthToken> _parser = new MessageParser<AuthToken>(() => new AuthToken());

		public const int TypeFieldNumber = 1;

		private AuthType type_;

		public const int DataFieldNumber = 2;

		private string data_ = string.Empty;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<AuthToken> Parser
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
				return CommonReflection.Descriptor.MessageTypes[0];
			}
		}

		[DebuggerNonUserCode]
		public AuthType Type
		{
			get
			{
				return type_;
			}
			set
			{
				type_ = value;
			}
		}

		[DebuggerNonUserCode]
		public string Data
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
		public AuthToken()
		{
		}

		[DebuggerNonUserCode]
		public AuthToken(AuthToken other)
			: this()
		{
			type_ = other.type_;
			data_ = other.data_;
		}

		[DebuggerNonUserCode]
		public AuthToken Clone()
		{
			return new AuthToken(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as AuthToken);
		}

		[DebuggerNonUserCode]
		public bool Equals(AuthToken other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (Type != other.Type)
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
			if (Type != 0)
			{
				num ^= Type.GetHashCode();
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
			if (Type != 0)
			{
				output.WriteRawTag(8);
				output.WriteEnum((int)Type);
			}
			if (Data.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteString(Data);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Type != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)Type);
			}
			if (Data.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Data);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(AuthToken other)
		{
			if (other != null)
			{
				if (other.Type != 0)
				{
					Type = other.Type;
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
					type_ = (AuthType)input.ReadEnum();
					break;
				case 18u:
					Data = input.ReadString();
					break;
				}
			}
		}
	}
}
