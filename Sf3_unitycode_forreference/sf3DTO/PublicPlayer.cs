using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class PublicPlayer : IMessage<PublicPlayer>, IMessage, IEquatable<PublicPlayer>, IDeepCloneable<PublicPlayer>
	{
		private static readonly MessageParser<PublicPlayer> _parser = new MessageParser<PublicPlayer>(() => new PublicPlayer());

		public const int ShortPlayerFieldNumber = 1;

		private ShortPlayer shortPlayer_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<PublicPlayer> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[17];
			}
		}

		[DebuggerNonUserCode]
		public ShortPlayer ShortPlayer
		{
			get
			{
				return shortPlayer_;
			}
			set
			{
				shortPlayer_ = value;
			}
		}

		[DebuggerNonUserCode]
		public PublicPlayer()
		{
		}

		[DebuggerNonUserCode]
		public PublicPlayer(PublicPlayer other)
			: this()
		{
			ShortPlayer = ((other.shortPlayer_ == null) ? null : other.ShortPlayer.Clone());
		}

		[DebuggerNonUserCode]
		public PublicPlayer Clone()
		{
			return new PublicPlayer(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as PublicPlayer);
		}

		[DebuggerNonUserCode]
		public bool Equals(PublicPlayer other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!object.Equals(ShortPlayer, other.ShortPlayer))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (shortPlayer_ != null)
			{
				num ^= ShortPlayer.GetHashCode();
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
			if (shortPlayer_ != null)
			{
				output.WriteRawTag(10);
				output.WriteMessage(ShortPlayer);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (shortPlayer_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(ShortPlayer);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(PublicPlayer other)
		{
			if (other != null && other.shortPlayer_ != null)
			{
				if (shortPlayer_ == null)
				{
					shortPlayer_ = new ShortPlayer();
				}
				ShortPlayer.MergeFrom(other.ShortPlayer);
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
					continue;
				}
				if (shortPlayer_ == null)
				{
					shortPlayer_ = new ShortPlayer();
				}
				input.ReadMessage(shortPlayer_);
			}
		}
	}
}
