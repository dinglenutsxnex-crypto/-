using System;
using Google.Protobuf.Reflection;

namespace common
{
	public static class CommonReflection
	{
		private static FileDescriptor descriptor;

		public static FileDescriptor Descriptor
		{
			get
			{
				return descriptor;
			}
		}

		static CommonReflection()
		{
			byte[] descriptorData = Convert.FromBase64String("Cgxjb21tb24ucHJvdG8SBmNvbW1vbiI5CglBdXRoVG9rZW4SHgoEdHlwZRgB" + "IAEoDjIQLmNvbW1vbi5BdXRoVHlwZRIMCgRkYXRhGAIgASgJIpEBCgxMb2dp" + "blJlcXVlc3QSDwoHdmVyc2lvbhgBIAEoBRItChJwcmltYXJ5X2F1dGhfdG9r" + "ZW4YAiABKAsyES5jb21tb24uQXV0aFRva2VuEi8KFHNlY29uZGFyeV9hdXRo" + "X3Rva2VuGAMgAygLMhEuY29tbW9uLkF1dGhUb2tlbhIQCghleHRfZGF0YRgE" + "IAEoCSJbCglLaWNrRXZlbnQSIgoGcmVhc29uGAEgASgOMhIuY29tbW9uLktp" + "Y2tSZWFzb24SEQoJaW5pdGlhdG9yGAIgASgJEhcKD2Jhbl9taWxsaXNfbGVm" + "dBgDIAEoAyImCg1Kb2luWm9uZUV2ZW50EhUKDXBsYXllcl9leGlzdHMYASAB" + "KAgiGgoJVGltZXN0YW1wEg0KBXZhbHVlGAEgASgDImYKDFBpbmdSZXNwb25z" + "ZRIqCg9jbGllbnRUaW1lc3RhbXAYASABKAsyES5jb21tb24uVGltZXN0YW1w" + "EioKD3NlcnZlclRpbWVzdGFtcBgCIAEoCzIRLmNvbW1vbi5UaW1lc3RhbXAi" + "KAoRVXBkYXRlQ29uZmlnRXZlbnQSEwoLY29uZmlnX3NpZ24YASABKAwq/QEK" + "EFJlcXVlc3RFcnJvckNvZGUSBgoCT0sQABIJCgVFUlJPUhABEhoKFklOVkFM" + "SURfU0VTU0lPTl9TVEFUVVMQZBIgChtTRVNTSU9OX0FMUkVBRFlfU1RBUlRf" + "TE9HSU4QyAESHwoaUExBWUVSX0FMUkVBRFlfU1RBUlRfTE9HSU4QyQESKwom" + "SU5WQUxJRF9DTElFTlRfU0VSVkVSX1BST1RPQ09MX1ZFUlNJT04QygESHgoZ" + "SU5WQUxJRF9MT0dJTl9PUl9QQVNTV09SRBDLARIaChVJTlZBTElEX1NFQ1VS" + "SVRZX0lORk8QzAESDgoJTUFYX1ZBTFVFEI9OKlIKCEF1dGhUeXBlEggKBE5P" + "TkUQABIKCgZERVZJQ0UQARIPCgtHQU1FX0NFTlRFUhACEg8KC0dPT0dMRV9Q" + "TEFZEAMSBgoCVksQBBIGCgJGQhAFKl8KCktpY2tSZWFzb24SGQoVQU5PVEhF" + "Ul9TRVNTSU9OX0xPR0lOEAASBwoDQkFOEAESEwoPSU5WQUxJRF9WRVJTSU9O" + "EAISGAoUTE9DS19GT1JfTUFJTlRFTkFOQ0UQAyoeCglTb3J0T3JkZXISBwoD" + "QVNDEAASCAoEREVTQxABQjEKGXJ1Lm5la2tpLmNvbW1vbnMucHJvdG9idWZC" + "CUNvbW1vbkRUT0gDqgIGY29tbW9uYgZwcm90bzM=");
			descriptor = FileDescriptor.FromGeneratedCode(descriptorData, new FileDescriptor[0], new GeneratedClrTypeInfo(new Type[4]
			{
				typeof(RequestErrorCode),
				typeof(AuthType),
				typeof(KickReason),
				typeof(SortOrder)
			}, new GeneratedClrTypeInfo[7]
			{
				new GeneratedClrTypeInfo(typeof(AuthToken), AuthToken.Parser, new string[2] { "Type", "Data" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(LoginRequest), LoginRequest.Parser, new string[4] { "Version", "PrimaryAuthToken", "SecondaryAuthToken", "ExtData" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(KickEvent), KickEvent.Parser, new string[3] { "Reason", "Initiator", "BanMillisLeft" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(JoinZoneEvent), JoinZoneEvent.Parser, new string[1] { "PlayerExists" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Timestamp), Timestamp.Parser, new string[1] { "Value" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(PingResponse), PingResponse.Parser, new string[2] { "ClientTimestamp", "ServerTimestamp" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(UpdateConfigEvent), UpdateConfigEvent.Parser, new string[1] { "ConfigSign" }, null, null, null)
			}));
		}
	}
}
