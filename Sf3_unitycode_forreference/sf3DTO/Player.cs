using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Player : IMessage<Player>, IMessage, IEquatable<Player>, IDeepCloneable<Player>
	{
		private static readonly MessageParser<Player> _parser = new MessageParser<Player>(() => new Player());

		public const int PublicPlayerFieldNumber = 1;

		private PublicPlayer publicPlayer_;

		public const int PermissionsFieldNumber = 2;

		private int permissions_;

		public const int ExperienceFieldNumber = 3;

		private long experience_;

		public const int CurrenciesFieldNumber = 4;

		private static readonly FieldCodec<Currency> _repeated_currencies_codec = FieldCodec.ForMessage(34u, Currency.Parser);

		private readonly RepeatedField<Currency> currencies_ = new RepeatedField<Currency>();

		public const int AppearanceFieldNumber = 5;

		private Appearance appearance_;

		public const int ShopFieldNumber = 6;

		private Shop shop_;

		public const int InventoryFieldNumber = 7;

		private Inventory inventory_;

		public const int BattleDataFieldNumber = 8;

		private BattleData battleData_;

		public const int ChapterIdFieldNumber = 9;

		private int chapterId_;

		public const int OfflineStateIdFieldNumber = 10;

		private long offlineStateId_;

		public const int AbTagFieldNumber = 11;

		private string abTag_ = string.Empty;

		public const int LogDataFieldNumber = 12;

		private LogData logData_;

		public const int RatingFieldNumber = 13;

		private double rating_;

		public const int BrawlerFightFieldNumber = 14;

		private BrawlerFight brawlerFight_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Player> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[18];
			}
		}

		[DebuggerNonUserCode]
		public PublicPlayer PublicPlayer
		{
			get
			{
				return publicPlayer_;
			}
			set
			{
				publicPlayer_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int Permissions
		{
			get
			{
				return permissions_;
			}
			set
			{
				permissions_ = value;
			}
		}

		[DebuggerNonUserCode]
		public long Experience
		{
			get
			{
				return experience_;
			}
			set
			{
				experience_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Currency> Currencies
		{
			get
			{
				return currencies_;
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
		public Shop Shop
		{
			get
			{
				return shop_;
			}
			set
			{
				shop_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Inventory Inventory
		{
			get
			{
				return inventory_;
			}
			set
			{
				inventory_ = value;
			}
		}

		[DebuggerNonUserCode]
		public BattleData BattleData
		{
			get
			{
				return battleData_;
			}
			set
			{
				battleData_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int ChapterId
		{
			get
			{
				return chapterId_;
			}
			set
			{
				chapterId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public long OfflineStateId
		{
			get
			{
				return offlineStateId_;
			}
			set
			{
				offlineStateId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public string AbTag
		{
			get
			{
				return abTag_;
			}
			set
			{
				abTag_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public LogData LogData
		{
			get
			{
				return logData_;
			}
			set
			{
				logData_ = value;
			}
		}

		[DebuggerNonUserCode]
		public double Rating
		{
			get
			{
				return rating_;
			}
			set
			{
				rating_ = value;
			}
		}

		[DebuggerNonUserCode]
		public BrawlerFight BrawlerFight
		{
			get
			{
				return brawlerFight_;
			}
			set
			{
				brawlerFight_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Player()
		{
		}

		[DebuggerNonUserCode]
		public Player(Player other)
			: this()
		{
			PublicPlayer = ((other.publicPlayer_ == null) ? null : other.PublicPlayer.Clone());
			permissions_ = other.permissions_;
			experience_ = other.experience_;
			currencies_ = other.currencies_.Clone();
			Appearance = ((other.appearance_ == null) ? null : other.Appearance.Clone());
			Shop = ((other.shop_ == null) ? null : other.Shop.Clone());
			Inventory = ((other.inventory_ == null) ? null : other.Inventory.Clone());
			BattleData = ((other.battleData_ == null) ? null : other.BattleData.Clone());
			chapterId_ = other.chapterId_;
			offlineStateId_ = other.offlineStateId_;
			abTag_ = other.abTag_;
			LogData = ((other.logData_ == null) ? null : other.LogData.Clone());
			rating_ = other.rating_;
			BrawlerFight = ((other.brawlerFight_ == null) ? null : other.BrawlerFight.Clone());
		}

		[DebuggerNonUserCode]
		public Player Clone()
		{
			return new Player(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Player);
		}

		[DebuggerNonUserCode]
		public bool Equals(Player other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!object.Equals(PublicPlayer, other.PublicPlayer))
			{
				return false;
			}
			if (Permissions != other.Permissions)
			{
				return false;
			}
			if (Experience != other.Experience)
			{
				return false;
			}
			if (!currencies_.Equals(other.currencies_))
			{
				return false;
			}
			if (!object.Equals(Appearance, other.Appearance))
			{
				return false;
			}
			if (!object.Equals(Shop, other.Shop))
			{
				return false;
			}
			if (!object.Equals(Inventory, other.Inventory))
			{
				return false;
			}
			if (!object.Equals(BattleData, other.BattleData))
			{
				return false;
			}
			if (ChapterId != other.ChapterId)
			{
				return false;
			}
			if (OfflineStateId != other.OfflineStateId)
			{
				return false;
			}
			if (AbTag != other.AbTag)
			{
				return false;
			}
			if (!object.Equals(LogData, other.LogData))
			{
				return false;
			}
			if (Rating != other.Rating)
			{
				return false;
			}
			if (!object.Equals(BrawlerFight, other.BrawlerFight))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (publicPlayer_ != null)
			{
				num ^= PublicPlayer.GetHashCode();
			}
			if (Permissions != 0)
			{
				num ^= Permissions.GetHashCode();
			}
			if (Experience != 0)
			{
				num ^= Experience.GetHashCode();
			}
			num ^= currencies_.GetHashCode();
			if (appearance_ != null)
			{
				num ^= Appearance.GetHashCode();
			}
			if (shop_ != null)
			{
				num ^= Shop.GetHashCode();
			}
			if (inventory_ != null)
			{
				num ^= Inventory.GetHashCode();
			}
			if (battleData_ != null)
			{
				num ^= BattleData.GetHashCode();
			}
			if (ChapterId != 0)
			{
				num ^= ChapterId.GetHashCode();
			}
			if (OfflineStateId != 0)
			{
				num ^= OfflineStateId.GetHashCode();
			}
			if (AbTag.Length != 0)
			{
				num ^= AbTag.GetHashCode();
			}
			if (logData_ != null)
			{
				num ^= LogData.GetHashCode();
			}
			if (Rating != 0.0)
			{
				num ^= Rating.GetHashCode();
			}
			if (brawlerFight_ != null)
			{
				num ^= BrawlerFight.GetHashCode();
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
			if (publicPlayer_ != null)
			{
				output.WriteRawTag(10);
				output.WriteMessage(PublicPlayer);
			}
			if (Permissions != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt32(Permissions);
			}
			if (Experience != 0)
			{
				output.WriteRawTag(24);
				output.WriteInt64(Experience);
			}
			currencies_.WriteTo(output, _repeated_currencies_codec);
			if (appearance_ != null)
			{
				output.WriteRawTag(42);
				output.WriteMessage(Appearance);
			}
			if (shop_ != null)
			{
				output.WriteRawTag(50);
				output.WriteMessage(Shop);
			}
			if (inventory_ != null)
			{
				output.WriteRawTag(58);
				output.WriteMessage(Inventory);
			}
			if (battleData_ != null)
			{
				output.WriteRawTag(66);
				output.WriteMessage(BattleData);
			}
			if (ChapterId != 0)
			{
				output.WriteRawTag(72);
				output.WriteInt32(ChapterId);
			}
			if (OfflineStateId != 0)
			{
				output.WriteRawTag(80);
				output.WriteInt64(OfflineStateId);
			}
			if (AbTag.Length != 0)
			{
				output.WriteRawTag(90);
				output.WriteString(AbTag);
			}
			if (logData_ != null)
			{
				output.WriteRawTag(98);
				output.WriteMessage(LogData);
			}
			if (Rating != 0.0)
			{
				output.WriteRawTag(105);
				output.WriteDouble(Rating);
			}
			if (brawlerFight_ != null)
			{
				output.WriteRawTag(114);
				output.WriteMessage(BrawlerFight);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (publicPlayer_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(PublicPlayer);
			}
			if (Permissions != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(Permissions);
			}
			if (Experience != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(Experience);
			}
			num += currencies_.CalculateSize(_repeated_currencies_codec);
			if (appearance_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Appearance);
			}
			if (shop_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Shop);
			}
			if (inventory_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Inventory);
			}
			if (battleData_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(BattleData);
			}
			if (ChapterId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ChapterId);
			}
			if (OfflineStateId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(OfflineStateId);
			}
			if (AbTag.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(AbTag);
			}
			if (logData_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(LogData);
			}
			if (Rating != 0.0)
			{
				num += 9;
			}
			if (brawlerFight_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(BrawlerFight);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Player other)
		{
			if (other == null)
			{
				return;
			}
			if (other.publicPlayer_ != null)
			{
				if (publicPlayer_ == null)
				{
					publicPlayer_ = new PublicPlayer();
				}
				PublicPlayer.MergeFrom(other.PublicPlayer);
			}
			if (other.Permissions != 0)
			{
				Permissions = other.Permissions;
			}
			if (other.Experience != 0)
			{
				Experience = other.Experience;
			}
			currencies_.Add(other.currencies_);
			if (other.appearance_ != null)
			{
				if (appearance_ == null)
				{
					appearance_ = new Appearance();
				}
				Appearance.MergeFrom(other.Appearance);
			}
			if (other.shop_ != null)
			{
				if (shop_ == null)
				{
					shop_ = new Shop();
				}
				Shop.MergeFrom(other.Shop);
			}
			if (other.inventory_ != null)
			{
				if (inventory_ == null)
				{
					inventory_ = new Inventory();
				}
				Inventory.MergeFrom(other.Inventory);
			}
			if (other.battleData_ != null)
			{
				if (battleData_ == null)
				{
					battleData_ = new BattleData();
				}
				BattleData.MergeFrom(other.BattleData);
			}
			if (other.ChapterId != 0)
			{
				ChapterId = other.ChapterId;
			}
			if (other.OfflineStateId != 0)
			{
				OfflineStateId = other.OfflineStateId;
			}
			if (other.AbTag.Length != 0)
			{
				AbTag = other.AbTag;
			}
			if (other.logData_ != null)
			{
				if (logData_ == null)
				{
					logData_ = new LogData();
				}
				LogData.MergeFrom(other.LogData);
			}
			if (other.Rating != 0.0)
			{
				Rating = other.Rating;
			}
			if (other.brawlerFight_ != null)
			{
				if (brawlerFight_ == null)
				{
					brawlerFight_ = new BrawlerFight();
				}
				BrawlerFight.MergeFrom(other.BrawlerFight);
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
					if (publicPlayer_ == null)
					{
						publicPlayer_ = new PublicPlayer();
					}
					input.ReadMessage(publicPlayer_);
					break;
				case 16u:
					Permissions = input.ReadInt32();
					break;
				case 24u:
					Experience = input.ReadInt64();
					break;
				case 34u:
					currencies_.AddEntriesFrom(input, _repeated_currencies_codec);
					break;
				case 42u:
					if (appearance_ == null)
					{
						appearance_ = new Appearance();
					}
					input.ReadMessage(appearance_);
					break;
				case 50u:
					if (shop_ == null)
					{
						shop_ = new Shop();
					}
					input.ReadMessage(shop_);
					break;
				case 58u:
					if (inventory_ == null)
					{
						inventory_ = new Inventory();
					}
					input.ReadMessage(inventory_);
					break;
				case 66u:
					if (battleData_ == null)
					{
						battleData_ = new BattleData();
					}
					input.ReadMessage(battleData_);
					break;
				case 72u:
					ChapterId = input.ReadInt32();
					break;
				case 80u:
					OfflineStateId = input.ReadInt64();
					break;
				case 90u:
					AbTag = input.ReadString();
					break;
				case 98u:
					if (logData_ == null)
					{
						logData_ = new LogData();
					}
					input.ReadMessage(logData_);
					break;
				case 105u:
					Rating = input.ReadDouble();
					break;
				case 114u:
					if (brawlerFight_ == null)
					{
						brawlerFight_ = new BrawlerFight();
					}
					input.ReadMessage(brawlerFight_);
					break;
				}
			}
		}
	}
}
