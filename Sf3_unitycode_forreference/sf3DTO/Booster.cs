using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Booster : IMessage<Booster>, IMessage, IEquatable<Booster>, IDeepCloneable<Booster>
	{
		private static readonly MessageParser<Booster> _parser = new MessageParser<Booster>(() => new Booster());

		public const int InstanceIdFieldNumber = 1;

		private long instanceId_;

		public const int ModelIdFieldNumber = 2;

		private int modelId_;

		public const int LootFieldNumber = 3;

		private Loot loot_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Booster> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[35];
			}
		}

		[DebuggerNonUserCode]
		public long InstanceId
		{
			get
			{
				return instanceId_;
			}
			set
			{
				instanceId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int ModelId
		{
			get
			{
				return modelId_;
			}
			set
			{
				modelId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Loot Loot
		{
			get
			{
				return loot_;
			}
			set
			{
				loot_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Booster()
		{
		}

		[DebuggerNonUserCode]
		public Booster(Booster other)
			: this()
		{
			instanceId_ = other.instanceId_;
			modelId_ = other.modelId_;
			Loot = ((other.loot_ == null) ? null : other.Loot.Clone());
		}

		[DebuggerNonUserCode]
		public Booster Clone()
		{
			return new Booster(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Booster);
		}

		[DebuggerNonUserCode]
		public bool Equals(Booster other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (InstanceId != other.InstanceId)
			{
				return false;
			}
			if (ModelId != other.ModelId)
			{
				return false;
			}
			if (!object.Equals(Loot, other.Loot))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (InstanceId != 0)
			{
				num ^= InstanceId.GetHashCode();
			}
			if (ModelId != 0)
			{
				num ^= ModelId.GetHashCode();
			}
			if (loot_ != null)
			{
				num ^= Loot.GetHashCode();
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
			if (InstanceId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt64(InstanceId);
			}
			if (ModelId != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt32(ModelId);
			}
			if (loot_ != null)
			{
				output.WriteRawTag(26);
				output.WriteMessage(Loot);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (InstanceId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(InstanceId);
			}
			if (ModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ModelId);
			}
			if (loot_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Loot);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Booster other)
		{
			if (other == null)
			{
				return;
			}
			if (other.InstanceId != 0)
			{
				InstanceId = other.InstanceId;
			}
			if (other.ModelId != 0)
			{
				ModelId = other.ModelId;
			}
			if (other.loot_ != null)
			{
				if (loot_ == null)
				{
					loot_ = new Loot();
				}
				Loot.MergeFrom(other.Loot);
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
					InstanceId = input.ReadInt64();
					break;
				case 16u:
					ModelId = input.ReadInt32();
					break;
				case 26u:
					if (loot_ == null)
					{
						loot_ = new Loot();
					}
					input.ReadMessage(loot_);
					break;
				}
			}
		}
	}
}
