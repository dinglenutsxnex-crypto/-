using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using common;

namespace sf3DTO
{
	public sealed class BrawlerFight : IMessage<BrawlerFight>, IMessage, IEquatable<BrawlerFight>, IDeepCloneable<BrawlerFight>
	{
		private static readonly MessageParser<BrawlerFight> _parser = new MessageParser<BrawlerFight>(() => new BrawlerFight());

		public const int EnemyFieldNumber = 1;

		private BrawlerEnemy enemy_;

		public const int ExpireTimeFieldNumber = 2;

		private Timestamp expireTime_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<BrawlerFight> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[45];
			}
		}

		[DebuggerNonUserCode]
		public BrawlerEnemy Enemy
		{
			get
			{
				return enemy_;
			}
			set
			{
				enemy_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Timestamp ExpireTime
		{
			get
			{
				return expireTime_;
			}
			set
			{
				expireTime_ = value;
			}
		}

		[DebuggerNonUserCode]
		public BrawlerFight()
		{
		}

		[DebuggerNonUserCode]
		public BrawlerFight(BrawlerFight other)
			: this()
		{
			Enemy = ((other.enemy_ == null) ? null : other.Enemy.Clone());
			ExpireTime = ((other.expireTime_ == null) ? null : other.ExpireTime.Clone());
		}

		[DebuggerNonUserCode]
		public BrawlerFight Clone()
		{
			return new BrawlerFight(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BrawlerFight);
		}

		[DebuggerNonUserCode]
		public bool Equals(BrawlerFight other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!object.Equals(Enemy, other.Enemy))
			{
				return false;
			}
			if (!object.Equals(ExpireTime, other.ExpireTime))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (enemy_ != null)
			{
				num ^= Enemy.GetHashCode();
			}
			if (expireTime_ != null)
			{
				num ^= ExpireTime.GetHashCode();
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
			if (enemy_ != null)
			{
				output.WriteRawTag(10);
				output.WriteMessage(Enemy);
			}
			if (expireTime_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(ExpireTime);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (enemy_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Enemy);
			}
			if (expireTime_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(ExpireTime);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BrawlerFight other)
		{
			if (other == null)
			{
				return;
			}
			if (other.enemy_ != null)
			{
				if (enemy_ == null)
				{
					enemy_ = new BrawlerEnemy();
				}
				Enemy.MergeFrom(other.Enemy);
			}
			if (other.expireTime_ != null)
			{
				if (expireTime_ == null)
				{
					expireTime_ = new Timestamp();
				}
				ExpireTime.MergeFrom(other.ExpireTime);
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
					if (enemy_ == null)
					{
						enemy_ = new BrawlerEnemy();
					}
					input.ReadMessage(enemy_);
					break;
				case 18u:
					if (expireTime_ == null)
					{
						expireTime_ = new Timestamp();
					}
					input.ReadMessage(expireTime_);
					break;
				}
			}
		}
	}
}
