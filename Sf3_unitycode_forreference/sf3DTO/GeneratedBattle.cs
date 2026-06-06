using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class GeneratedBattle : IMessage<GeneratedBattle>, IMessage, IEquatable<GeneratedBattle>, IDeepCloneable<GeneratedBattle>
	{
		private static readonly MessageParser<GeneratedBattle> _parser = new MessageParser<GeneratedBattle>(() => new GeneratedBattle());

		public const int ModelIdFieldNumber = 1;

		private int modelId_;

		public const int FightsFieldNumber = 2;

		private static readonly FieldCodec<GeneratedFight> _repeated_fights_codec = FieldCodec.ForMessage(18u, GeneratedFight.Parser);

		private readonly RepeatedField<GeneratedFight> fights_ = new RepeatedField<GeneratedFight>();

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<GeneratedBattle> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[31];
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
		public RepeatedField<GeneratedFight> Fights
		{
			get
			{
				return fights_;
			}
		}

		[DebuggerNonUserCode]
		public GeneratedBattle()
		{
		}

		[DebuggerNonUserCode]
		public GeneratedBattle(GeneratedBattle other)
			: this()
		{
			modelId_ = other.modelId_;
			fights_ = other.fights_.Clone();
		}

		[DebuggerNonUserCode]
		public GeneratedBattle Clone()
		{
			return new GeneratedBattle(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as GeneratedBattle);
		}

		[DebuggerNonUserCode]
		public bool Equals(GeneratedBattle other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (ModelId != other.ModelId)
			{
				return false;
			}
			if (!fights_.Equals(other.fights_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (ModelId != 0)
			{
				num ^= ModelId.GetHashCode();
			}
			return num ^ fights_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (ModelId != 0)
			{
				output.WriteRawTag(8);
				output.WriteInt32(ModelId);
			}
			fights_.WriteTo(output, _repeated_fights_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (ModelId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(ModelId);
			}
			return num + fights_.CalculateSize(_repeated_fights_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(GeneratedBattle other)
		{
			if (other != null)
			{
				if (other.ModelId != 0)
				{
					ModelId = other.ModelId;
				}
				fights_.Add(other.fights_);
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
					ModelId = input.ReadInt32();
					break;
				case 18u:
					fights_.AddEntriesFrom(input, _repeated_fights_codec);
					break;
				}
			}
		}
	}
}
