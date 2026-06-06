using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;

namespace Network.core.events
{
	public class EventType
	{
		private List<string> tags;

		public System.Type sendType { get; private set; }

		public System.Type responseType { get; private set; }

		public bool isStateChanger
		{
			get
			{
				return tags.Contains("state_change");
			}
		}

		public List<string> PossibleErrors { get; private set; }

		public EventType(System.Type _responseType)
		{
			sendType = typeof(Empty);
			responseType = _responseType;
			tags = new List<string>();
			PossibleErrors = new List<string>();
		}

		public EventType(System.Type _sendType, System.Type _responseType, List<string> _tags, List<string> _possibleErrors)
		{
			sendType = _sendType;
			responseType = _responseType;
			tags = _tags;
			PossibleErrors = _possibleErrors;
		}
	}
}
