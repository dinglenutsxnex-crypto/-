using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class BrawlerEnemy : IMessage<BrawlerEnemy>, IMessage, IEquatable<BrawlerEnemy>, IDeepCloneable<BrawlerEnemy>
	{
		private static readonly MessageParser<BrawlerEnemy> _parser = new MessageParser<BrawlerEnemy>(() => new BrawlerEnemy());

		public const int ShortPlayerFieldNumber = 1;

		private ShortPlayer shortPlayer_;

		public const int ItemsFieldNumber = 2;

		private static readonly FieldCodec<Item> _repeated_items_codec = FieldCodec.ForMessage(18u, Item.Parser);

		private readonly RepeatedField<Item> items_ = new RepeatedField<Item>();

		public const int PerksFieldNumber = 3;

		private static readonly FieldCodec<Perk> _repeated_perks_codec = FieldCodec.ForMessage(26u, Perk.Parser);

		private readonly RepeatedField<Perk> perks_ = new RepeatedField<Perk>();

		public const int AppearanceFieldNumber = 4;

		private Appearance appearance_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<BrawlerEnemy> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[44];
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
		public RepeatedField<Item> Items
		{
			get
			{
				return items_;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Perk> Perks
		{
			get
			{
				return perks_;
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
		public BrawlerEnemy()
		{
		}

		[DebuggerNonUserCode]
		public BrawlerEnemy(BrawlerEnemy other)
			: this()
		{
			ShortPlayer = ((other.shortPlayer_ == null) ? null : other.ShortPlayer.Clone());
			items_ = other.items_.Clone();
			perks_ = other.perks_.Clone();
			Appearance = ((other.appearance_ == null) ? null : other.Appearance.Clone());
		}

		[DebuggerNonUserCode]
		public BrawlerEnemy Clone()
		{
			return new BrawlerEnemy(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BrawlerEnemy);
		}

		[DebuggerNonUserCode]
		public bool Equals(BrawlerEnemy other)
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
			if (!items_.Equals(other.items_))
			{
				return false;
			}
			if (!perks_.Equals(other.perks_))
			{
				return false;
			}
			if (!object.Equals(Appearance, other.Appearance))
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
			num ^= items_.GetHashCode();
			num ^= perks_.GetHashCode();
			if (appearance_ != null)
			{
				num ^= Appearance.GetHashCode();
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
			items_.WriteTo(output, _repeated_items_codec);
			perks_.WriteTo(output, _repeated_perks_codec);
			if (appearance_ != null)
			{
				output.WriteRawTag(34);
				output.WriteMessage(Appearance);
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
			num += items_.CalculateSize(_repeated_items_codec);
			num += perks_.CalculateSize(_repeated_perks_codec);
			if (appearance_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Appearance);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BrawlerEnemy other)
		{
			if (other == null)
			{
				return;
			}
			if (other.shortPlayer_ != null)
			{
				if (shortPlayer_ == null)
				{
					shortPlayer_ = new ShortPlayer();
				}
				ShortPlayer.MergeFrom(other.ShortPlayer);
			}
			items_.Add(other.items_);
			perks_.Add(other.perks_);
			if (other.appearance_ != null)
			{
				if (appearance_ == null)
				{
					appearance_ = new Appearance();
				}
				Appearance.MergeFrom(other.Appearance);
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
					if (shortPlayer_ == null)
					{
						shortPlayer_ = new ShortPlayer();
					}
					input.ReadMessage(shortPlayer_);
					break;
				case 18u:
					items_.AddEntriesFrom(input, _repeated_items_codec);
					break;
				case 26u:
					perks_.AddEntriesFrom(input, _repeated_perks_codec);
					break;
				case 34u:
					if (appearance_ == null)
					{
						appearance_ = new Appearance();
					}
					input.ReadMessage(appearance_);
					break;
				}
			}
		}
	}
}
