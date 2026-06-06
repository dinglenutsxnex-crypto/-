using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public sealed class OfflineRequestBatch : IMessage<OfflineRequestBatch>, IMessage, IEquatable<OfflineRequestBatch>, IDeepCloneable<OfflineRequestBatch>
	{
		private static readonly MessageParser<OfflineRequestBatch> _parser = new MessageParser<OfflineRequestBatch>(() => new OfflineRequestBatch());

		public const int RequestsFieldNumber = 1;

		private static readonly FieldCodec<OfflineRequest> _repeated_requests_codec = FieldCodec.ForMessage(10u, OfflineRequest.Parser);

		private readonly RepeatedField<OfflineRequest> requests_ = new RepeatedField<OfflineRequest>();

		public const int ClientStateFieldNumber = 2;

		private MutableOfflineState clientState_;

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public static MessageParser<OfflineRequestBatch> Parser
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
				return Sf3Reflection.Descriptor.MessageTypes[42];
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<OfflineRequest> Requests
		{
			get
			{
				return requests_;
			}
		}

		[DebuggerNonUserCode]
		public MutableOfflineState ClientState
		{
			get
			{
				return clientState_;
			}
			set
			{
				clientState_ = value;
			}
		}

		[DebuggerNonUserCode]
		public OfflineRequestBatch()
		{
		}

		[DebuggerNonUserCode]
		public OfflineRequestBatch(OfflineRequestBatch other)
			: this()
		{
			requests_ = other.requests_.Clone();
			ClientState = ((other.clientState_ == null) ? null : other.ClientState.Clone());
		}

		[DebuggerNonUserCode]
		public OfflineRequestBatch Clone()
		{
			return new OfflineRequestBatch(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as OfflineRequestBatch);
		}

		[DebuggerNonUserCode]
		public bool Equals(OfflineRequestBatch other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (!requests_.Equals(other.requests_))
			{
				return false;
			}
			if (!object.Equals(ClientState, other.ClientState))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			num ^= requests_.GetHashCode();
			if (clientState_ != null)
			{
				num ^= ClientState.GetHashCode();
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
			requests_.WriteTo(output, _repeated_requests_codec);
			if (clientState_ != null)
			{
				output.WriteRawTag(18);
				output.WriteMessage(ClientState);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			num += requests_.CalculateSize(_repeated_requests_codec);
			if (clientState_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(ClientState);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(OfflineRequestBatch other)
		{
			if (other == null)
			{
				return;
			}
			requests_.Add(other.requests_);
			if (other.clientState_ != null)
			{
				if (clientState_ == null)
				{
					clientState_ = new MutableOfflineState();
				}
				ClientState.MergeFrom(other.ClientState);
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
					requests_.AddEntriesFrom(input, _repeated_requests_codec);
					break;
				case 18u:
					if (clientState_ == null)
					{
						clientState_ = new MutableOfflineState();
					}
					input.ReadMessage(clientState_);
					break;
				}
			}
		}
	}
}
