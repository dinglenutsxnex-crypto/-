using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class Appearance : IMessage<Appearance>, IMessage, IEquatable<Appearance>, IDeepCloneable<Appearance>
	{
		private static readonly MessageParser<Appearance> _parser = new MessageParser<Appearance>(() => new Appearance());

		public const int GenderFieldNumber = 1;

		private Gender gender_;

		public const int HeadIdFieldNumber = 2;

		private int headId_;

		public const int HairColorFieldNumber = 3;

		private Color hairColor_;

		public const int SkinColorFieldNumber = 4;

		private Color skinColor_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<Appearance> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[4];
			}
		}

		[DebuggerNonUserCode]
		public Gender Gender
		{
			get
			{
				return gender_;
			}
			set
			{
				gender_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int HeadId
		{
			get
			{
				return headId_;
			}
			set
			{
				headId_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Color HairColor
		{
			get
			{
				return hairColor_;
			}
			set
			{
				hairColor_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Color SkinColor
		{
			get
			{
				return skinColor_;
			}
			set
			{
				skinColor_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Appearance()
		{
		}

		[DebuggerNonUserCode]
		public Appearance(Appearance other)
			: this()
		{
			gender_ = other.gender_;
			headId_ = other.headId_;
			HairColor = ((other.hairColor_ == null) ? null : other.HairColor.Clone());
			SkinColor = ((other.skinColor_ == null) ? null : other.SkinColor.Clone());
		}

		[DebuggerNonUserCode]
		public Appearance Clone()
		{
			return new Appearance(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Appearance);
		}

		[DebuggerNonUserCode]
		public bool Equals(Appearance other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (Gender != other.Gender)
			{
				return false;
			}
			if (HeadId != other.HeadId)
			{
				return false;
			}
			if (!object.Equals(HairColor, other.HairColor))
			{
				return false;
			}
			if (!object.Equals(SkinColor, other.SkinColor))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (Gender != 0)
			{
				num ^= Gender.GetHashCode();
			}
			if (HeadId != 0)
			{
				num ^= HeadId.GetHashCode();
			}
			if (hairColor_ != null)
			{
				num ^= HairColor.GetHashCode();
			}
			if (skinColor_ != null)
			{
				num ^= SkinColor.GetHashCode();
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
			if (Gender != 0)
			{
				output.WriteRawTag(8);
				output.WriteEnum((int)Gender);
			}
			if (HeadId != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt32(HeadId);
			}
			if (hairColor_ != null)
			{
				output.WriteRawTag(26);
				output.WriteMessage(HairColor);
			}
			if (skinColor_ != null)
			{
				output.WriteRawTag(34);
				output.WriteMessage(SkinColor);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Gender != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)Gender);
			}
			if (HeadId != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(HeadId);
			}
			if (hairColor_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(HairColor);
			}
			if (skinColor_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(SkinColor);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Appearance other)
		{
			if (other == null)
			{
				return;
			}
			if (other.Gender != 0)
			{
				Gender = other.Gender;
			}
			if (other.HeadId != 0)
			{
				HeadId = other.HeadId;
			}
			if (other.hairColor_ != null)
			{
				if (hairColor_ == null)
				{
					hairColor_ = new Color();
				}
				HairColor.MergeFrom(other.HairColor);
			}
			if (other.skinColor_ != null)
			{
				if (skinColor_ == null)
				{
					skinColor_ = new Color();
				}
				SkinColor.MergeFrom(other.SkinColor);
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
					gender_ = (Gender)input.ReadEnum();
					break;
				case 16u:
					HeadId = input.ReadInt32();
					break;
				case 26u:
					if (hairColor_ == null)
					{
						hairColor_ = new Color();
					}
					input.ReadMessage(hairColor_);
					break;
				case 34u:
					if (skinColor_ == null)
					{
						skinColor_ = new Color();
					}
					input.ReadMessage(skinColor_);
					break;
				}
			}
		}
	}
}
