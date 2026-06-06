using System;
using Google.Protobuf.Reflection;
using common;

namespace sf3DTO
{
	public static class Sf3Reflection
	{
		private static FileDescriptor descriptor;

		public static FileDescriptor Descriptor
		{
			get
			{
				return descriptor;
			}
		}

		static Sf3Reflection()
		{
			byte[] descriptorData = Convert.FromBase64String("CglzZjMucHJvdG8SBnNmM0RUTxoMY29tbW9uLnByb3RvIkAKElJlc3BvbnNl" + "QW5kU3RhdGVJZBIYChBvZmZsaW5lX3N0YXRlX2lkGAEgASgDEhAKCHJlc3Bv" + "bnNlGAIgASgMIjwKB0FjY291bnQSDQoFbG9naW4YASABKAkSIgoIYXV0aFR5" + "cGUYAiABKA4yEC5jb21tb24uQXV0aFR5cGUiRgoIQ3VycmVuY3kSKwoNY3Vy" + "cmVuY3lfdHlwZRgBIAEoDjIULnNmM0RUTy5DdXJyZW5jeVR5cGUSDQoFdmFs" + "dWUYAiABKAMiKAoFQ29sb3ISEAoIY29sb3JfaWQYASABKAUSDQoFdmFsdWUY" + "AiABKAEigwEKCkFwcGVhcmFuY2USHgoGZ2VuZGVyGAEgASgOMg4uc2YzRFRP" + "LkdlbmRlchIPCgdoZWFkX2lkGAIgASgFEiEKCmhhaXJfY29sb3IYAyABKAsy" + "DS5zZjNEVE8uQ29sb3ISIQoKc2tpbl9jb2xvchgEIAEoCzINLnNmM0RUTy5D" + "b2xvciJJCghTaG9wSXRlbRIQCghtb2RlbF9pZBgBIAEoBRITCgtzdGFja19s" + "ZXZlbBgCIAEoARIWCg5wdXJjaGFzZV9jb3VudBgDIAEoBSJYCgRTaG9wEh8K" + "BWl0ZW1zGAEgAygLMhAuc2YzRFRPLlNob3BJdGVtEi8KFGxhc3RfZ2VuZXJh" + "dGlvbl90aW1lGAIgASgLMhEuY29tbW9uLlRpbWVzdGFtcCItCgRQZXJrEhAK" + "CG1vZGVsX2lkGAEgASgFEhMKC3N0YWNrX2xldmVsGAIgASgBIjUKCFBlcmtT" + "bG90EhIKCnNsb3RfaW5kZXgYASABKAUSFQoNcGVya19tb2RlbF9pZBgCIAEo" + "BSJgCgRJdGVtEhAKCG1vZGVsX2lkGAEgASgFEhMKC3N0YWNrX2xldmVsGAIg" + "ASgBEhAKCGVxdWlwcGVkGAMgASgIEh8KBXBlcmtzGAQgAygLMhAuc2YzRFRP" + "LlBlcmtTbG90ImgKCUludmVudG9yeRIbCgVpdGVtcxgBIAMoCzIMLnNmM0RU" + "Ty5JdGVtEhsKBXBlcmtzGAIgAygLMgwuc2YzRFRPLlBlcmsSIQoIYm9vc3Rl" + "cnMYAyADKAsyDy5zZjNEVE8uQm9vc3RlciI9CgdMb2dEYXRhEhsKE2FuYWx5" + "dGljc19sb2dfbGV2ZWwYASABKAUSFQoNY2xpZW50X2xvZ19pZBgCIAEoAyJG" + "Cg5CdXlJdGVtUmVxdWVzdBIQCghtb2RlbF9pZBgBIAEoBRIiCghjdXJyZW5j" + "eRgCIAEoCzIQLnNmM0RUTy5DdXJyZW5jeSIvCgxFcXVpcFJlcXVlc3QSEAoI" + "bW9kZWxfaWQYASABKAUSDQoFZXF1aXAYAiABKAgiZQoRSW5zZXJ0UGVya1Jl" + "cXVlc3QSFQoNaXRlbV9tb2RlbF9pZBgBIAEoBRISCgpzbG90X2luZGV4GAIg" + "ASgFEhUKDXBlcmtfbW9kZWxfaWQYAyABKAUSDgoGaW5zZXJ0GAQgASgIIlcK" + "C1Nob3J0UGxheWVyEhEKCXBsYXllcl9pZBgBIAEoAxIQCghuaWNrbmFtZRgC" + "IAEoCRIUCgxkaXNwbGF5X25hbWUYAyABKAkSDQoFbGV2ZWwYBCABKAUiMwoM" + "U2hvcnRQbGF5ZXJzEiMKBnZhbHVlcxgBIAMoCzITLnNmM0RUTy5TaG9ydFBs" + "YXllciI5CgxQdWJsaWNQbGF5ZXISKQoMc2hvcnRfcGxheWVyGAEgASgLMhMu" + "c2YzRFRPLlNob3J0UGxheWVyIrUDCgZQbGF5ZXISKwoNcHVibGljX3BsYXll" + "chgBIAEoCzIULnNmM0RUTy5QdWJsaWNQbGF5ZXISEwoLcGVybWlzc2lvbnMY" + "AiABKAUSEgoKZXhwZXJpZW5jZRgDIAEoAxIkCgpjdXJyZW5jaWVzGAQgAygL" + "MhAuc2YzRFRPLkN1cnJlbmN5EiYKCmFwcGVhcmFuY2UYBSABKAsyEi5zZjNE" + "VE8uQXBwZWFyYW5jZRIaCgRzaG9wGAYgASgLMgwuc2YzRFRPLlNob3ASJAoJ" + "aW52ZW50b3J5GAcgASgLMhEuc2YzRFRPLkludmVudG9yeRInCgtiYXR0bGVf" + "ZGF0YRgIIAEoCzISLnNmM0RUTy5CYXR0bGVEYXRhEhIKCmNoYXB0ZXJfaWQY" + "CSABKAUSGAoQb2ZmbGluZV9zdGF0ZV9pZBgKIAEoAxIOCgZhYl90YWcYCyAB" + "KAkSIQoIbG9nX2RhdGEYDCABKAsyDy5zZjNEVE8uTG9nRGF0YRIOCgZyYXRp" + "bmcYDSABKAESKwoNYnJhd2xlcl9maWdodBgOIAEoCzIULnNmM0RUTy5CcmF3" + "bGVyRmlnaHQijQIKDkV4dGVuZGVkUGxheWVyEiYKDnByaW1hcnlfcGxheWVy" + "GAEgASgLMg4uc2YzRFRPLlBsYXllchIoChBzZWNvbmRhcnlfcGxheWVyGAIg" + "ASgLMg4uc2YzRFRPLlBsYXllchIoCg9wcmltYXJ5X2FjY291bnQYAyABKAsy" + "Dy5zZjNEVE8uQWNjb3VudBIqChFzZWNvbmRhcnlfYWNjb3VudBgEIAEoCzIP" + "LnNmM0RUTy5BY2NvdW50EiQKCXRpbWVzdGFtcBgFIAEoCzIRLmNvbW1vbi5U" + "aW1lc3RhbXASLQoOYnJhd2xlcl9maW5pc2gYBiABKAsyFS5zZjNEVE8uQnJh" + "d2xlckZpbmlzaCIZCghQbGF5ZXJJZBINCgV2YWx1ZRgBIAEoAyIfCglQbGF5" + "ZXJJZHMSEgoGdmFsdWVzGAEgAygDQgIQASJkChNDcmVhdGVQbGF5ZXJSZXF1" + "ZXN0EhQKDGRpc3BsYXlfbmFtZRgBIAEoCRImCgphcHBlYXJhbmNlGAIgASgL" + "MhIuc2YzRFRPLkFwcGVhcmFuY2USDwoHdmVyc2lvbhgDIAEoCSJfChBHZXRQ" + "bGF5ZXJSZXF1ZXN0Eg8KB3ZlcnNpb24YASABKAkSOgoVb2ZmbGluZV9yZXF1" + "ZXN0X2JhdGNoGAIgASgLMhsuc2YzRFRPLk9mZmxpbmVSZXF1ZXN0QmF0Y2gi" + "ogEKBExvb3QSJAoKY3VycmVuY2llcxgBIAMoCzIQLnNmM0RUTy5DdXJyZW5j" + "eRISCgpleHBlcmllbmNlGAIgASgDEiAKCmVxdWlwbWVudHMYAyADKAsyDC5z" + "ZjNEVE8uSXRlbRIbCgVwZXJrcxgEIAMoCzIMLnNmM0RUTy5QZXJrEiEKCGJv" + "b3N0ZXJzGAUgAygLMg8uc2YzRFRPLkJvb3N0ZXIiIQoNV2Fycmlvckl0ZW1J" + "ZBIQCghtb2RlbF9pZBgBIAEoBSLHAQoHV2FycmlvchINCgVhbGlhcxgBIAEo" + "CRIeCgZnZW5kZXIYAiABKA4yDi5zZjNEVE8uR2VuZGVyEhUKDWFwcGVhcmFu" + "Y2VfaWQYAyABKAUSHwoHYWlfbW9kZRgEIAEoDjIOLnNmM0RUTy5BaU1vZGUS" + "DQoFcG93ZXIYBSABKAESKQoKZXF1aXBtZW50cxgGIAMoCzIVLnNmM0RUTy5X" + "YXJyaW9ySXRlbUlkEhsKBXBlcmtzGAcgAygLMgwuc2YzRFRPLlBlcmsiOQoS" + "Um91bmRSdWxlQXR0cmlidXRlEg8KB2F0dHJfaWQYASABKAUSEgoKYXR0cl92" + "YWx1ZRgCIAEoCSJHCglSb3VuZFJ1bGUSDwoHcnVsZV9pZBgBIAEoBRIpCgVh" + "dHRycxgCIAMoCzIaLnNmM0RUTy5Sb3VuZFJ1bGVBdHRyaWJ1dGUiVAoOR2Vu" + "ZXJhdGVkUm91bmQSIAoFcnVsZXMYASADKAsyES5zZjNEVE8uUm91bmRSdWxl" + "EiAKB3dhcnJpb3IYAiABKAsyDy5zZjNEVE8uV2FycmlvciJlCg5HZW5lcmF0" + "ZWRGaWdodBImCgZyb3VuZHMYASADKAsyFi5zZjNEVE8uR2VuZXJhdGVkUm91" + "bmQSKwoVcmV3YXJkc19ieV9yb3VuZF93aW5zGAIgAygLMgwuc2YzRFRPLkxv" + "b3QiSwoPR2VuZXJhdGVkQmF0dGxlEhAKCG1vZGVsX2lkGAEgASgFEiYKBmZp" + "Z2h0cxgCIAMoCzIWLnNmM0RUTy5HZW5lcmF0ZWRGaWdodCK/AQoGQmF0dGxl" + "EigKB2JhdHRsZXMYASADKAsyFy5zZjNEVE8uR2VuZXJhdGVkQmF0dGxlEhYK" + "DmJhdHRsZV9jb3VudGVyGAIgASgFEhsKE2N1cnJlbnRfZmlnaHRfaW5kZXgY" + "AyABKAUSIwoIZ2VuX3RpbWUYBCABKAsyES5jb21tb24uVGltZXN0YW1wEjEK" + "Fmxhc3RfZmlnaHRfZmluaXNoX3RpbWUYBSABKAsyES5jb21tb24uVGltZXN0" + "YW1wIi0KCkJhdHRsZURhdGESHwoHYmF0dGxlcxgBIAMoCzIOLnNmM0RUTy5C" + "YXR0bGUikwIKEkZpbmlzaEZpZ2h0UmVxdWVzdBIXCg9iYXR0bGVfbW9kZWxf" + "aWQYASABKAUSFgoOYmF0dGxlX2NvdW50ZXIYAiABKAUSGwoTY3VycmVudF9m" + "aWdodF9pbmRleBgDIAEoBRIjCgZyZXN1bHQYBCABKA4yEy5zZjNEVE8uRmln" + "aHRSZXN1bHQSEgoKd29uX3JvdW5kcxgFIAEoBRImCgtmaW5pc2hfdGltZRgG" + "IAEoCzIRLmNvbW1vbi5UaW1lc3RhbXASFAoMdG90YWxfcm91bmRzGAcgASgF" + "EjgKC211bHRpcGxpZXJzGAggAygLMiMuc2YzRFRPLkZpbmlzaEZpZ2h0UmV3" + "YXJkTXVsdGlwbGllciJMCgdCb29zdGVyEhMKC2luc3RhbmNlX2lkGAEgASgD" + "EhAKCG1vZGVsX2lkGAIgASgFEhoKBGxvb3QYAyABKAsyDC5zZjNEVE8uTG9v" + "dCIpChJPcGVuQm9vc3RlclJlcXVlc3QSEwoLaW5zdGFuY2VfaWQYASABKAMi" + "VgoRQnV5Qm9vc3RlclJlcXVlc3QSHQoVc2hvcF9ib29zdGVyX21vZGVsX2lk" + "GAEgASgFEiIKCGN1cnJlbmN5GAIgASgLMhAuc2YzRFRPLkN1cnJlbmN5IloK" + "EkJ1eUJvb3N0ZXJSZXNwb25zZRIgCgdib29zdGVyGAEgASgLMg8uc2YzRFRP" + "LkJvb3N0ZXISIgoIY3VycmVuY3kYAiADKAsyEC5zZjNEVE8uQ3VycmVuY3ki" + "RAobRmluaXNoRmlnaHRSZXdhcmRNdWx0aXBsaWVyEhUKDW11bHRpcGxpZXJf" + "aWQYASABKAUSDgoGYW1vdW50GAIgASgFIpYBChNNdXRhYmxlT2ZmbGluZVN0" + "YXRlEhAKCHN0YXRlX2lkGAEgASgDEhIKCmV4cGVyaWVuY2UYAiABKAMSDQoF" + "bGV2ZWwYAyABKAUSJAoKY3VycmVuY2llcxgEIAMoCzIQLnNmM0RUTy5DdXJy" + "ZW5jeRIkCglpbnZlbnRvcnkYBSABKAsyES5zZjNEVE8uSW52ZW50b3J5IlkK" + "Dk9mZmxpbmVSZXF1ZXN0EhQKDG5ld19zdGF0ZV9pZBgBIAEoAxILCgNjbWQY" + "AiABKAkSFgoOY29uZmlnX3ZlcnNpb24YAyABKAkSDAoEZGF0YRgEIAEoDCJy" + "ChNPZmZsaW5lUmVxdWVzdEJhdGNoEigKCHJlcXVlc3RzGAEgAygLMhYuc2Yz" + "RFRPLk9mZmxpbmVSZXF1ZXN0EjEKDGNsaWVudF9zdGF0ZRgCIAEoCzIbLnNm" + "M0RUTy5NdXRhYmxlT2ZmbGluZVN0YXRlIhwKCkxvZ1JlcXVlc3QSDgoGZXZl" + "bnRzGAEgAygJIpsBCgxCcmF3bGVyRW5lbXkSKQoMc2hvcnRfcGxheWVyGAEg" + "ASgLMhMuc2YzRFRPLlNob3J0UGxheWVyEhsKBWl0ZW1zGAIgAygLMgwuc2Yz" + "RFRPLkl0ZW0SGwoFcGVya3MYAyADKAsyDC5zZjNEVE8uUGVyaxImCgphcHBl" + "YXJhbmNlGAQgASgLMhIuc2YzRFRPLkFwcGVhcmFuY2UiWwoMQnJhd2xlckZp" + "Z2h0EiMKBWVuZW15GAEgASgLMhQuc2YzRFRPLkJyYXdsZXJFbmVteRImCgtl" + "eHBpcmVfdGltZRgCIAEoCzIRLmNvbW1vbi5UaW1lc3RhbXAisAEKFEJyYXds" + "ZXJGaW5pc2hSZXF1ZXN0EiMKBWVuZW15GAEgASgLMhQuc2YzRFRPLkJyYXds" + "ZXJFbmVteRIjCgZyZXN1bHQYAiABKA4yEy5zZjNEVE8uRmlnaHRSZXN1bHQS" + "FAoMdG90YWxfcm91bmRzGAMgASgFEjgKC211bHRpcGxpZXJzGAQgAygLMiMu" + "c2YzRFRPLkZpbmlzaEZpZ2h0UmV3YXJkTXVsdGlwbGllciJDCg1CcmF3bGVy" + "RmluaXNoEhwKBnJld2FyZBgBIAEoCzIMLnNmM0RUTy5Mb290EhQKDHJhdGlu" + "Z19kZWx0YRgCIAEoASqoAwoQUmVxdWVzdEVycm9yQ29kZRIhCh1ERVBSRUNB" + "VEVEX1JFUVVFU1RfRVJST1JfQ09ERRAAEhUKEFBMQVlFUl9OT1RfRk9VTkQQ" + "kE4SGwoWSU5WQUxJRF9DT05GSUdfVkVSU0lPThCRThIaChRJTlZBTElEX0RJ" + "U1BMQVlfTkFNRRCgnAESJwohUkVDT1ZFUkFCTEVfT0ZGTElORV9SRVFVRVNU" + "X0VSUk9SELDqARIpCiNVTlJFQ09WRVJBQkxFX09GRkxJTkVfUkVRVUVTVF9F" + "UlJPUhCx6gESFwoRSU5WQUxJRF9MT0dfRVZFTlQQwLgCEh8KGUJSQVdMRVJf" + "Q0FOTk9UX0ZJTkRfRU5FTVkQ0IYDEhoKFEJSQVdMRVJfRklHSFRfTUlTU0VE" + "ENGGAxIbChVCUkFXTEVSX0lOVkFMSURfRU5FTVkQ0oYDEh0KF0JSQVdMRVJf" + "QUxSRUFEWV9TVEFSVEVEENOGAxIiChxCUkFXTEVSX0lOVkFMSURfVE9UQUxf" + "Uk9VTkRTENSGAxIXChFDQlRfSU5WQUxJRF9FTUFJTBDg1AMqeAoJQ29uc3Rh" + "bnRzEhcKE0RFUFJFQ0FURURfQ09OU1RBTlQQABITCg9DVVJSRU5UX1ZFUlNJ" + "T04QAhIZChVNSU5fU1VQUE9SVEVEX1ZFUlNJT04QAhIeChpNQVhfUkVRVUVT" + "VEVEX1BMQVlFUl9DT1VOVBBkGgIQASpKCgxDdXJyZW5jeVR5cGUSGQoVVU5L" + "Tk9XTl9DVVJSRU5DWV9UWVBFEAASCAoEQ09JThABEgkKBUJPTlVTEAISCgoG" + "U0hBRE9XEAMqRAoHRmFjdGlvbhITCg9VTktOT1dOX0ZBQ1RJT04QABIKCgZM" + "RUdJT04QARILCgdIRVJBTERTEAISCwoHRFlOQVNUWRADKksKBlJhcml0eRIS" + "Cg5VTktOT1dOX1JBUklUWRAAEgoKBkNPTU1PThABEggKBFJBUkUQAhIICgRF" + "UElDEAMSDQoJTEVHRU5EQVJZEAQqWwoISXRlbVR5cGUSFQoRVU5LTk9XTl9J" + "VEVNX1RZUEUQABIKCgZIRUxNRVQQARIJCgVBUk1PUhACEgoKBldFQVBPThAD" + "EgoKBlJBTkdFRBAEEgkKBU1BR0lDEAUqRgoIUGVya1R5cGUSFQoRVU5LTk9X" + "Tl9QRVJLX1RZUEUQABIICgRQRVJLEAESDwoLRU5DSEFOVE1FTlQQAhIICgRN" + "T1ZFEAMqMgoGR2VuZGVyEhIKDlVOS05PV05fR0VOREVSEAASCAoETUFMRRAB" + "EgoKBkZFTUFMRRACKl4KBkFpTW9kZRITCg9VTktOT1dOX0FJX01PREUQABIQ" + "CgxSRUdVTEFSX01PREUQARINCglOT05FX01PREUQAhIPCgtTRU5TRUlfTU9E" + "RRADEg0KCURPSk9fTU9ERRAEKo0BCgpCYXR0bGVUeXBlEhcKE1VOS05PV05f" + "QkFUVExFX1RZUEUQABIICgRNQUlOEAESCAoEU0lERRACEgwKCFNVUlZJVkFM" + "EAMSCQoFREFJTFkQBBILCgdNSVNTSU9OEAUSCwoHQlJBV0xFUhALEggKBFRF" + "U1QQZBIKCgVMT0NBTBDIARIJCgRET0pPEKwCKkkKC0ZpZ2h0UmVzdWx0EhgK" + "FFVOS05PV05fRklHSFRfUkVTVUxUEAASBwoDV0lOEAESCAoETE9TUxACEg0K" + "CVNVUlJFTkRFUhADKicKClBlcm1pc3Npb24SCAoETk9ORRAAEg8KB0NIRUFU" + "RVIQgICAgARCLgoccnUubmVra2kuc2YzLnNoYXJlZC5wcm90b2J1ZkIDRFRP" + "SAOqAgZzZjNEVE9iBnByb3RvMw==");
			descriptor = FileDescriptor.FromGeneratedCode(descriptorData, new FileDescriptor[1] { CommonReflection.Descriptor }, new GeneratedClrTypeInfo(new Type[12]
			{
				typeof(RequestErrorCode),
				typeof(Constants),
				typeof(CurrencyType),
				typeof(Faction),
				typeof(Rarity),
				typeof(ItemType),
				typeof(PerkType),
				typeof(Gender),
				typeof(AiMode),
				typeof(BattleType),
				typeof(FightResult),
				typeof(Permission)
			}, new GeneratedClrTypeInfo[48]
			{
				new GeneratedClrTypeInfo(typeof(ResponseAndStateId), ResponseAndStateId.Parser, new string[2] { "OfflineStateId", "Response" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Account), Account.Parser, new string[2] { "Login", "AuthType" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Currency), Currency.Parser, new string[2] { "CurrencyType", "Value" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Color), Color.Parser, new string[2] { "ColorId", "Value" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Appearance), Appearance.Parser, new string[4] { "Gender", "HeadId", "HairColor", "SkinColor" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(ShopItem), ShopItem.Parser, new string[3] { "ModelId", "StackLevel", "PurchaseCount" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Shop), Shop.Parser, new string[2] { "Items", "LastGenerationTime" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Perk), Perk.Parser, new string[2] { "ModelId", "StackLevel" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(PerkSlot), PerkSlot.Parser, new string[2] { "SlotIndex", "PerkModelId" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Item), Item.Parser, new string[4] { "ModelId", "StackLevel", "Equipped", "Perks" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Inventory), Inventory.Parser, new string[3] { "Items", "Perks", "Boosters" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(LogData), LogData.Parser, new string[2] { "AnalyticsLogLevel", "ClientLogId" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(BuyItemRequest), BuyItemRequest.Parser, new string[2] { "ModelId", "Currency" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(EquipRequest), EquipRequest.Parser, new string[2] { "ModelId", "Equip" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(InsertPerkRequest), InsertPerkRequest.Parser, new string[4] { "ItemModelId", "SlotIndex", "PerkModelId", "Insert" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(ShortPlayer), ShortPlayer.Parser, new string[4] { "PlayerId", "Nickname", "DisplayName", "Level" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(ShortPlayers), ShortPlayers.Parser, new string[1] { "Values" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(PublicPlayer), PublicPlayer.Parser, new string[1] { "ShortPlayer" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Player), Player.Parser, new string[14]
				{
					"PublicPlayer", "Permissions", "Experience", "Currencies", "Appearance", "Shop", "Inventory", "BattleData", "ChapterId", "OfflineStateId",
					"AbTag", "LogData", "Rating", "BrawlerFight"
				}, null, null, null),
				new GeneratedClrTypeInfo(typeof(ExtendedPlayer), ExtendedPlayer.Parser, new string[6] { "PrimaryPlayer", "SecondaryPlayer", "PrimaryAccount", "SecondaryAccount", "Timestamp", "BrawlerFinish" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(PlayerId), PlayerId.Parser, new string[1] { "Value" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(PlayerIds), PlayerIds.Parser, new string[1] { "Values" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(CreatePlayerRequest), CreatePlayerRequest.Parser, new string[3] { "DisplayName", "Appearance", "Version" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(GetPlayerRequest), GetPlayerRequest.Parser, new string[2] { "Version", "OfflineRequestBatch" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Loot), Loot.Parser, new string[5] { "Currencies", "Experience", "Equipments", "Perks", "Boosters" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(WarriorItemId), WarriorItemId.Parser, new string[1] { "ModelId" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Warrior), Warrior.Parser, new string[7] { "Alias", "Gender", "AppearanceId", "AiMode", "Power", "Equipments", "Perks" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(RoundRuleAttribute), RoundRuleAttribute.Parser, new string[2] { "AttrId", "AttrValue" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(RoundRule), RoundRule.Parser, new string[2] { "RuleId", "Attrs" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(GeneratedRound), GeneratedRound.Parser, new string[2] { "Rules", "Warrior" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(GeneratedFight), GeneratedFight.Parser, new string[2] { "Rounds", "RewardsByRoundWins" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(GeneratedBattle), GeneratedBattle.Parser, new string[2] { "ModelId", "Fights" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Battle), Battle.Parser, new string[5] { "Battles", "BattleCounter", "CurrentFightIndex", "GenTime", "LastFightFinishTime" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(BattleData), BattleData.Parser, new string[1] { "Battles" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(FinishFightRequest), FinishFightRequest.Parser, new string[8] { "BattleModelId", "BattleCounter", "CurrentFightIndex", "Result", "WonRounds", "FinishTime", "TotalRounds", "Multipliers" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(Booster), Booster.Parser, new string[3] { "InstanceId", "ModelId", "Loot" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(OpenBoosterRequest), OpenBoosterRequest.Parser, new string[1] { "InstanceId" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(BuyBoosterRequest), BuyBoosterRequest.Parser, new string[2] { "ShopBoosterModelId", "Currency" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(BuyBoosterResponse), BuyBoosterResponse.Parser, new string[2] { "Booster", "Currency" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(FinishFightRewardMultiplier), FinishFightRewardMultiplier.Parser, new string[2] { "MultiplierId", "Amount" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(MutableOfflineState), MutableOfflineState.Parser, new string[5] { "StateId", "Experience", "Level", "Currencies", "Inventory" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(OfflineRequest), OfflineRequest.Parser, new string[4] { "NewStateId", "Cmd", "ConfigVersion", "Data" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(OfflineRequestBatch), OfflineRequestBatch.Parser, new string[2] { "Requests", "ClientState" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(LogRequest), LogRequest.Parser, new string[1] { "Events" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(BrawlerEnemy), BrawlerEnemy.Parser, new string[4] { "ShortPlayer", "Items", "Perks", "Appearance" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(BrawlerFight), BrawlerFight.Parser, new string[2] { "Enemy", "ExpireTime" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(BrawlerFinishRequest), BrawlerFinishRequest.Parser, new string[4] { "Enemy", "Result", "TotalRounds", "Multipliers" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(BrawlerFinish), BrawlerFinish.Parser, new string[2] { "Reward", "RatingDelta" }, null, null, null)
			}));
		}
	}
}
