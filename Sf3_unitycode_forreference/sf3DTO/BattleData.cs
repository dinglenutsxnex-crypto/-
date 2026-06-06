using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class BattleData : IMessage<BattleData>, IMessage, IEquatable<BattleData>, IDeepCloneable<BattleData>
	{
		private static readonly MessageParser<BattleData> _parser = new MessageParser<BattleData>(() => new BattleData());

		public const int BattlesFieldNumber = 1;

		private static readonly FieldCodec<Battle> _repeated_battles_codec = FieldCodec.ForMessage(10u, Battle.Parser);

		private readonly RepeatedField<Battle> battles_ = new RepeatedField<Battle>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<BattleData> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[33];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Battle> Battles
		{
			get
			{
				return battles_;
			}
		}

		[DebuggerNonUserCode]
		public BattleData()
		{
		}

		[DebuggerNonUserCode]
		public BattleData(BattleData other)
			: this()
		{
			battles_ = other.battles_.Clone();
		}

		[DebuggerNonUserCode]
		public BattleData Clone()
		{
			return new BattleData(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BattleData);
		}

		[DebuggerNonUserCode]
		public bool Equals(BattleData other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!battles_.Equals(other.battles_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			return num ^ battles_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			battles_.WriteTo(output, _repeated_battles_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			return num + battles_.CalculateSize(_repeated_battles_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BattleData other)
		{
			if (other != null)
			{
				battles_.Add(other.battles_);
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
				}
				else
				{
					battles_.AddEntriesFrom(input, _repeated_battles_codec);
				}
			}
		}
	}
}
