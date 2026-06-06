using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class ShortPlayer : IMessage<ShortPlayer>, IMessage, IEquatable<ShortPlayer>, IDeepCloneable<ShortPlayer>
	{
		private static readonly MessageParser<ShortPlayer> _parser = new MessageParser<ShortPlayer>(() => new ShortPlayer());

		public const int PlayerIdFieldNumber = 1;

		private long playerId_;

		public const int NicknameFieldNumber = 2;

		private string nickname_ = string.Empty;

		public const int DisplayNameFieldNumber = 3;

		private string displayName_ = string.Empty;

		public const int LevelFieldNumber = 4;

		private int level_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<ShortPlayer> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[15];
			}
		}

		[DebuggerNonUserCode]
		public long PlayerId
		{
			get
			{
				return playerId_;
			}
			set
			{
				playerId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public string Nickname
		{
			get
			{
				return nickname_;
			}
			set
			{
				nickname_ = ProtoPreconditions.CheckNotNull(value, "value");
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
		public int Level
		{
			get
			{
				return level_;
			}
			set
			{
				level_ = value;
			}
		}

		[DebuggerNonUserCode]
		public ShortPlayer()
		{
		}

		[DebuggerNonUserCode]
		public ShortPlayer(ShortPlayer other)
			: this()
		{
			playerId_ = other.playerId_;
			nickname_ = other.nickname_;
			displayName_ = other.displayName_;
			level_ = other.level_;
		}

		[DebuggerNonUserCode]
		public ShortPlayer Clone()
		{
			return new ShortPlayer(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as ShortPlayer);
		}

		[DebuggerNonUserCode]
		public bool Equals(ShortPlayer other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (PlayerId != other.PlayerId)
			{
				return false;
			}
			if (Nickname != other.Nickname)
			{
				return false;
			}
			if (DisplayName != other.DisplayName)
			{
				return false;
			}
			if (Level != other.Level)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (PlayerId != 0)
			{
				num ^= PlayerId.GetHashCode();
			}
			if (Nickname.Length != 0)
			{
				num ^= Nickname.GetHashCode();
			}
			if (DisplayName.Length != 0)
			{
				num ^= DisplayName.GetHashCode();
			}
			if (Level != 0)
			{
				num ^= Level.GetHashCode();
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
			if (PlayerId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt64(PlayerId);
			}
			if (Nickname.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteString(Nickname);
			}
			if (DisplayName.Length != 0)
			{
				output.WriteRawTag(26);
				output.WriteString(DisplayName);
			}
			if (Level != 0)
			{
				output.WriteRawTag(32);
				output.WriteInt32(Level);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (PlayerId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(PlayerId);
			}
			if (Nickname.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Nickname);
			}
			if (DisplayName.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(DisplayName);
			}
			if (Level != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(Level);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(ShortPlayer other)
		{
			if (other != null)
			{
				if (other.PlayerId != 0)
				{
					PlayerId = other.PlayerId;
				}
				if (other.Nickname.Length != 0)
				{
					Nickname = other.Nickname;
				}
				if (other.DisplayName.Length != 0)
				{
					DisplayName = other.DisplayName;
				}
				if (other.Level != 0)
				{
					Level = other.Level;
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
					PlayerId = input.ReadInt64();
					break;
				case 18u:
					Nickname = input.ReadString();
					break;
				case 26u:
					DisplayName = input.ReadString();
					break;
				case 32u:
					Level = input.ReadInt32();
					break;
				}
			}
		}
	}
}
