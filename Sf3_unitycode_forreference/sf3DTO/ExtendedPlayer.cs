using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using common;

namespace sf3DTO
{
	public sealed class ExtendedPlayer : IMessage<ExtendedPlayer>, IMessage, IEquatable<ExtendedPlayer>, IDeepCloneable<ExtendedPlayer>
	{
		private static readonly MessageParser<ExtendedPlayer> _parser = new MessageParser<ExtendedPlayer>(() => new ExtendedPlayer());

		public const int PrimaryPlayerFieldNumber = 1;

		private Player primaryPlayer_;

		public const int SecondaryPlayerFieldNumber = 2;

		private Player secondaryPlayer_;

		public const int PrimaryAccountFieldNumber = 3;

		private Account primaryAccount_;

		public const int SecondaryAccountFieldNumber = 4;

		private Account secondaryAccount_;

		public const int TimestampFieldNumber = 5;

		private Timestamp timestamp_;

		public const int BrawlerFinishFieldNumber = 6;

		private BrawlerFinish brawlerFinish_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<ExtendedPlayer> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[19];
			}
		}

		[DebuggerNonUserCode]
		public Player PrimaryPlayer
		{
			get
			{
				return primaryPlayer_;
			}
			set
			{
				primaryPlayer_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Player SecondaryPlayer
		{
			get
			{
				return secondaryPlayer_;
			}
			set
			{
				secondaryPlayer_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Account PrimaryAccount
		{
			get
			{
				return primaryAccount_;
			}
			set
			{
				primaryAccount_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Account SecondaryAccount
		{
			get
			{
				return secondaryAccount_;
			}
			set
			{
				secondaryAccount_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Timestamp Timestamp
		{
			get
			{
				return timestamp_;
			}
			set
			{
				timestamp_ = value;
			}
		}

		[DebuggerNonUserCode]
		public BrawlerFinish BrawlerFinish
		{
			get
			{
				return brawlerFinish_;
			}
			set
			{
				brawlerFinish_ = value;
			}
		}

		[DebuggerNonUserCode]
		public ExtendedPlayer()
		{
		}

		[DebuggerNonUserCode]
		public ExtendedPlayer(ExtendedPlayer other)
			: this()
		{
			PrimaryPlayer = ((other.primaryPlayer_ == null) ? null : other.PrimaryPlayer.Clone());
			SecondaryPlayer = ((other.secondaryPlayer_ == null) ? null : other.SecondaryPlayer.Clone());
			PrimaryAccount = ((other.primaryAccount_ == null) ? null : other.PrimaryAccount.Clone());
			SecondaryAccount = ((other.secondaryAccount_ == null) ? null : other.SecondaryAccount.Clone());
			Timestamp = ((other.timestamp_ == null) ? null : other.Timestamp.Clone());
			BrawlerFinish = ((other.brawlerFinish_ == null) ? null : other.BrawlerFinish.Clone());
		}

		[DebuggerNonUserCode]
		public ExtendedPlayer Clone()
		{
			return new ExtendedPlayer(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as ExtendedPlayer);
		}

		[DebuggerNonUserCode]
		public bool Equals(ExtendedPlayer other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!object.Equals(PrimaryPlayer, other.PrimaryPlayer))
			{
				return false;
			}
			if (!object.Equals(SecondaryPlayer, other.SecondaryPlayer))
			{
				return false;
			}
			if (!object.Equals(PrimaryAccount, other.PrimaryAccount))
			{
				return false;
			}
			if (!object.Equals(SecondaryAccount, other.SecondaryAccount))
			{
				return false;
			}
			if (!object.Equals(Timestamp, other.Timestamp))
			{
				return false;
			}
			if (!object.Equals(BrawlerFinish, other.BrawlerFinish))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (primaryPlayer_ != null)
			{
				num ^= PrimaryPlayer.GetHashCode();
			}
			if (secondaryPlayer_ != null)
			{
				num ^= SecondaryPlayer.GetHashCode();
			}
			if (primaryAccount_ != null)
			{
				num ^= PrimaryAccount.GetHashCode();
			}
			if (secondaryAccount_ != null)
			{
				num ^= SecondaryAccount.GetHashCode();
			}
			if (timestamp_ != null)
			{
				num ^= Timestamp.GetHashCode();
			}
			if (brawlerFinish_ != null)
			{
				num ^= BrawlerFinish.GetHashCode();
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
			if (primaryPlayer_ != null)
			{
				output.WriteRawTag(10);
				output.WriteMessage(PrimaryPlayer);
			}
			if (secondaryPlayer_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(SecondaryPlayer);
			}
			if (primaryAccount_ != null)
			{
				output.WriteRawTag(26);
				output.WriteMessage(PrimaryAccount);
			}
			if (secondaryAccount_ != null)
			{
				output.WriteRawTag(34);
				output.WriteMessage(SecondaryAccount);
			}
			if (timestamp_ != null)
			{
				output.WriteRawTag(42);
				output.WriteMessage(Timestamp);
			}
			if (brawlerFinish_ != null)
			{
				output.WriteRawTag(50);
				output.WriteMessage(BrawlerFinish);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (primaryPlayer_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(PrimaryPlayer);
			}
			if (secondaryPlayer_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(SecondaryPlayer);
			}
			if (primaryAccount_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(PrimaryAccount);
			}
			if (secondaryAccount_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(SecondaryAccount);
			}
			if (timestamp_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Timestamp);
			}
			if (brawlerFinish_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(BrawlerFinish);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(ExtendedPlayer other)
		{
			if (other == null)
			{
				return;
			}
			if (other.primaryPlayer_ != null)
			{
				if (primaryPlayer_ == null)
				{
					primaryPlayer_ = new Player();
				}
				PrimaryPlayer.MergeFrom(other.PrimaryPlayer);
			}
			if (other.secondaryPlayer_ != null)
			{
				if (secondaryPlayer_ == null)
				{
					secondaryPlayer_ = new Player();
				}
				SecondaryPlayer.MergeFrom(other.SecondaryPlayer);
			}
			if (other.primaryAccount_ != null)
			{
				if (primaryAccount_ == null)
				{
					primaryAccount_ = new Account();
				}
				PrimaryAccount.MergeFrom(other.PrimaryAccount);
			}
			if (other.secondaryAccount_ != null)
			{
				if (secondaryAccount_ == null)
				{
					secondaryAccount_ = new Account();
				}
				SecondaryAccount.MergeFrom(other.SecondaryAccount);
			}
			if (other.timestamp_ != null)
			{
				if (timestamp_ == null)
				{
					timestamp_ = new Timestamp();
				}
				Timestamp.MergeFrom(other.Timestamp);
			}
			if (other.brawlerFinish_ != null)
			{
				if (brawlerFinish_ == null)
				{
					brawlerFinish_ = new BrawlerFinish();
				}
				BrawlerFinish.MergeFrom(other.BrawlerFinish);
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
					if (primaryPlayer_ == null)
					{
						primaryPlayer_ = new Player();
					}
					input.ReadMessage(primaryPlayer_);
					break;
				case 18u:
					if (secondaryPlayer_ == null)
					{
						secondaryPlayer_ = new Player();
					}
					input.ReadMessage(secondaryPlayer_);
					break;
				case 26u:
					if (primaryAccount_ == null)
					{
						primaryAccount_ = new Account();
					}
					input.ReadMessage(primaryAccount_);
					break;
				case 34u:
					if (secondaryAccount_ == null)
					{
						secondaryAccount_ = new Account();
					}
					input.ReadMessage(secondaryAccount_);
					break;
				case 42u:
					if (timestamp_ == null)
					{
						timestamp_ = new Timestamp();
					}
					input.ReadMessage(timestamp_);
					break;
				case 50u:
					if (brawlerFinish_ == null)
					{
						brawlerFinish_ = new BrawlerFinish();
					}
					input.ReadMessage(brawlerFinish_);
					break;
				}
			}
		}
	}
}
