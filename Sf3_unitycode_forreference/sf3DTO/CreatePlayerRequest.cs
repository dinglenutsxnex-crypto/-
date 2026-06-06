using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class CreatePlayerRequest : IMessage<CreatePlayerRequest>, IMessage, IEquatable<CreatePlayerRequest>, IDeepCloneable<CreatePlayerRequest>
	{
		private static readonly MessageParser<CreatePlayerRequest> _parser = new MessageParser<CreatePlayerRequest>(() => new CreatePlayerRequest());

		public const int DisplayNameFieldNumber = 1;

		private string displayName_ = string.Empty;

		public const int AppearanceFieldNumber = 2;

		private Appearance appearance_;

		public const int VersionFieldNumber = 3;

		private string version_ = string.Empty;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<CreatePlayerRequest> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[22];
			}
		}

		[DebuggerNonUserCode]
		public string DisplayName
		{
			get
			{
				return displayName_;
			}
			set
			{
				displayName_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public Appearance Appearance
		{
			get
			{
				return appearance_;
			}
			set
			{
				appearance_ = value;
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
		public CreatePlayerRequest()
		{
		}

		[DebuggerNonUserCode]
		public CreatePlayerRequest(CreatePlayerRequest other)
			: this()
		{
			displayName_ = other.displayName_;
			Appearance = ((other.appearance_ == null) ? null : other.Appearance.Clone());
			version_ = other.version_;
		}

		[DebuggerNonUserCode]
		public CreatePlayerRequest Clone()
		{
			return new CreatePlayerRequest(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as CreatePlayerRequest);
		}

		[DebuggerNonUserCode]
		public bool Equals(CreatePlayerRequest other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (DisplayName != other.DisplayName)
			{
				return false;
			}
			if (!object.Equals(Appearance, other.Appearance))
			{
				return false;
			}
			if (Version != other.Version)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (DisplayName.Length != 0)
			{
				num ^= DisplayName.GetHashCode();
			}
			if (appearance_ != null)
			{
				num ^= Appearance.GetHashCode();
			}
			if (Version.Length != 0)
			{
				num ^= Version.GetHashCode();
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
			if (DisplayName.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(DisplayName);
			}
			if (appearance_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(Appearance);
			}
			if (Version.Length != 0)
			{
				output.WriteRawTag(26);
				output.WriteString(Version);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (DisplayName.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(DisplayName);
			}
			if (appearance_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Appearance);
			}
			if (Version.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Version);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CreatePlayerRequest other)
		{
			if (other == null)
			{
				return;
			}
			if (other.DisplayName.Length != 0)
			{
				DisplayName = other.DisplayName;
			}
			if (other.appearance_ != null)
			{
				if (appearance_ == null)
				{
					appearance_ = new Appearance();
				}
				Appearance.MergeFrom(other.Appearance);
			}
			if (other.Version.Length != 0)
			{
				Version = other.Version;
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
					DisplayName = input.ReadString();
					break;
				case 18u:
					if (appearance_ == null)
					{
						appearance_ = new Appearance();
					}
					input.ReadMessage(appearance_);
					break;
				case 26u:
					Version = input.ReadString();
					break;
				}
			}
		}
	}
}
