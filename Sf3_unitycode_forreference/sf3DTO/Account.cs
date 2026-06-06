using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using common;

namespace sf3DTO
{
	public sealed class Account : IMessage<Account>, IMessage, IEquatable<Account>, IDeepCloneable<Account>
	{
		private static readonly MessageParser<Account> _parser = new MessageParser<Account>(() => new Account());

		public const int LoginFieldNumber = 1;

		private string login_ = string.Empty;

		public const int AuthTypeFieldNumber = 2;

		private AuthType authType_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Account> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[1];
			}
		}

		[DebuggerNonUserCode]
		public string Login
		{
			get
			{
				return login_;
			}
			set
			{
				login_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public AuthType AuthType
		{
			get
			{
				return authType_;
			}
			set
			{
				authType_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Account()
		{
		}

		[DebuggerNonUserCode]
		public Account(Account other)
			: this()
		{
			login_ = other.login_;
			authType_ = other.authType_;
		}

		[DebuggerNonUserCode]
		public Account Clone()
		{
			return new Account(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Account);
		}

		[DebuggerNonUserCode]
		public bool Equals(Account other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (Login != other.Login)
			{
				return false;
			}
			if (AuthType != other.AuthType)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (Login.Length != 0)
			{
				num ^= Login.GetHashCode();
			}
			if (AuthType != 0)
			{
				num ^= AuthType.GetHashCode();
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
			if (Login.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(Login);
			}
			if (AuthType != 0)
			{
				output.WriteRawTag(16);
				output.WriteEnum((int)AuthType);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Login.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Login);
			}
			if (AuthType != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)AuthType);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Account other)
		{
			if (other != null)
			{
				if (other.Login.Length != 0)
				{
					Login = other.Login;
				}
				if (other.AuthType != 0)
				{
					AuthType = other.AuthType;
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
				case 10u:
					Login = input.ReadString();
					break;
				case 16u:
					authType_ = (AuthType)input.ReadEnum();
					break;
				}
			}
		}
	}
}
