using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace common
{
	public sealed class LoginRequest : IMessage<LoginRequest>, IMessage, IEquatable<LoginRequest>, IDeepCloneable<LoginRequest>
	{
		private static readonly MessageParser<LoginRequest> _parser = new MessageParser<LoginRequest>(() => new LoginRequest());

		public const int VersionFieldNumber = 1;

		private int version_;

		public const int PrimaryAuthTokenFieldNumber = 2;

		private AuthToken primaryAuthToken_;

		public const int SecondaryAuthTokenFieldNumber = 3;

		private static readonly FieldCodec<AuthToken> _repeated_secondaryAuthToken_codec = FieldCodec.ForMessage(26u, AuthToken.Parser);

		private readonly RepeatedField<AuthToken> secondaryAuthToken_ = new RepeatedField<AuthToken>();

		public const int ExtDataFieldNumber = 4;

		private string extData_ = string.Empty;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<LoginRequest> Parser
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
				return CommonReflection.Descriptor.MessageTypes[1];
			}
		}

		[DebuggerNonUserCode]
		public int Version
		{
			get
			{
				return version_;
			}
			set
			{
				version_ = value;
			}
		}

		[DebuggerNonUserCode]
		public AuthToken PrimaryAuthToken
		{
			get
			{
				return primaryAuthToken_;
			}
			set
			{
				primaryAuthToken_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<AuthToken> SecondaryAuthToken
		{
			get
			{
				return secondaryAuthToken_;
			}
		}

		[DebuggerNonUserCode]
		public string ExtData
		{
			get
			{
				return extData_;
			}
			set
			{
				extData_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public LoginRequest()
		{
		}

		[DebuggerNonUserCode]
		public LoginRequest(LoginRequest other)
			: this()
		{
			version_ = other.version_;
			PrimaryAuthToken = ((other.primaryAuthToken_ == null) ? null : other.PrimaryAuthToken.Clone());
			secondaryAuthToken_ = other.secondaryAuthToken_.Clone();
			extData_ = other.extData_;
		}

		[DebuggerNonUserCode]
		public LoginRequest Clone()
		{
			return new LoginRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as LoginRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(LoginRequest other)
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
			if (!object.Equals(PrimaryAuthToken, other.PrimaryAuthToken))
			{
				return false;
			}
			if (!secondaryAuthToken_.Equals(other.secondaryAuthToken_))
			{
				return false;
			}
			if (ExtData != other.ExtData)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (Version != 0)
			{
				num ^= Version.GetHashCode();
			}
			if (primaryAuthToken_ != null)
			{
				num ^= PrimaryAuthToken.GetHashCode();
			}
			num ^= secondaryAuthToken_.GetHashCode();
			if (ExtData.Length != 0)
			{
				num ^= ExtData.GetHashCode();
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
			if (Version != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(Version);
			}
			if (primaryAuthToken_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(PrimaryAuthToken);
			}
			secondaryAuthToken_.WriteTo(output, _repeated_secondaryAuthToken_codec);
			if (ExtData.Length != 0)
			{
				output.WriteRawTag(34);
				output.WriteString(ExtData);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Version != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(Version);
			}
			if (primaryAuthToken_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(PrimaryAuthToken);
			}
			num += secondaryAuthToken_.CalculateSize(_repeated_secondaryAuthToken_codec);
			if (ExtData.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(ExtData);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(LoginRequest other)
		{
			if (other == null)
			{
				return;
			}
			if (other.Version != 0)
			{
				Version = other.Version;
			}
			if (other.primaryAuthToken_ != null)
			{
				if (primaryAuthToken_ == null)
				{
					primaryAuthToken_ = new AuthToken();
				}
				PrimaryAuthToken.MergeFrom(other.PrimaryAuthToken);
			}
			secondaryAuthToken_.Add(other.secondaryAuthToken_);
			if (other.ExtData.Length != 0)
			{
				ExtData = other.ExtData;
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
					Version = input.ReadInt32();
					break;
				case 18u:
					if (primaryAuthToken_ == null)
					{
						primaryAuthToken_ = new AuthToken();
					}
					input.ReadMessage(primaryAuthToken_);
					break;
				case 26u:
					secondaryAuthToken_.AddEntriesFrom(input, _repeated_secondaryAuthToken_codec);
					break;
				case 34u:
					ExtData = input.ReadString();
					break;
				}
			}
		}
	}
}
