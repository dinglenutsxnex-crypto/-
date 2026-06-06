using System;
using Google.Protobuf;
using Network.core.events;

public delegate int SendDelegate(string cmd, IMessage e, float timeout, Action<NetworkEvent> callback, object data);
