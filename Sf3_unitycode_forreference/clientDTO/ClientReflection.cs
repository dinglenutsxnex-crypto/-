using System;
using Google.Protobuf.Reflection;

namespace clientDTO
{
	public static class ClientReflection
	{
		private static FileDescriptor descriptor;

		public static FileDescriptor Descriptor
		{
			get
			{
				return descriptor;
			}
		}

		static ClientReflection()
		{
			byte[] descriptorData = Convert.FromBase64String("CgxjbGllbnQucHJvdG8SCWNsaWVudERUTyJaChJPZmZsaW5lUmVxdWVzdERh" + "dGESEQoJcmVxdWVzdElEGAEgASgDEgsKA2NtZBgCIAEoCRIUCgx2ZXJzaW9u" + "X2Z1bGwYAyABKAkSDgoGYmluYXJ5GAQgASgMQg5IA6oCCWNsaWVudERUT2IG" + "cHJvdG8z");
			descriptor = FileDescriptor.FromGeneratedCode(descriptorData, new FileDescriptor[0], new GeneratedClrTypeInfo(null, new GeneratedClrTypeInfo[1]
			{
				new GeneratedClrTypeInfo(typeof(OfflineRequestData), OfflineRequestData.Parser, new string[4] { "RequestID", "Cmd", "VersionFull", "Binary" }, null, null, null)
			}));
		}
	}
}
