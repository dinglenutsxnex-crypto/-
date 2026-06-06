using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Nekki.GamingService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prime31;
using Sfs2X.Entities.Data;
using Sfs2X.Util;
using UnityEngine;
using common;

namespace Network.core.data
{
	public class SFSProtocol
	{
		protected System.Random _randGen;

		private static int currentID;

		public SFSProtocol()
		{
			int millisecond = DateTime.Now.Millisecond;
			_randGen = new System.Random(millisecond);
		}

		protected void SetID(ref SFSObject obj)
		{
			obj.PutInt("id", GetNextRequestID());
		}

		private int GetNextRequestID()
		{
			if (currentID == int.MaxValue)
			{
				currentID = int.MinValue;
			}
			return ++currentID;
		}

		protected void PackDataAsBytes(ref SFSObject outObj, IMessage data)
		{
			outObj.PutByteArray("b", new Sfs2X.Util.ByteArray(data.ToBinary()));
			SetID(ref outObj);
		}

		public SFSObject PackObject(IMessage data)
		{
			SFSObject outObj = new SFSObject();
			PackDataAsBytes(ref outObj, data);
			return outObj;
		}

		public common.Timestamp GetTimestamp()
		{
			common.Timestamp timestamp = new common.Timestamp();
			timestamp.Value = (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
			return timestamp;
		}

		public SFSObject GetCommonInt64(long value)
		{
			SFSObject outObj = new SFSObject();
			Int64Value int64Value = new Int64Value();
			int64Value.Value = value;
			PackDataAsBytes(ref outObj, int64Value);
			return outObj;
		}

		public SFSObject GetCommonInt32(int value)
		{
			SFSObject outObj = new SFSObject();
			Int32Value int32Value = new Int32Value();
			int32Value.Value = value;
			Debug.Log("param.value = " + int32Value.Value);
			PackDataAsBytes(ref outObj, int32Value);
			return outObj;
		}

		public SFSObject GetCommonStr(string value)
		{
			SFSObject outObj = new SFSObject();
			StringValue stringValue = new StringValue();
			stringValue.Value = value;
			Debug.Log("param.value = " + stringValue.Value);
			PackDataAsBytes(ref outObj, stringValue);
			return outObj;
		}

		public SFSObject GetEmpty()
		{
			SFSObject outObj = new SFSObject();
			Empty data = new Empty();
			PackDataAsBytes(ref outObj, data);
			return outObj;
		}

		public virtual SFSObject GetLoginRequest(string sessionToken, string deviceID, int version, JObject extraData = null)
		{
			SFSObject outObj = new SFSObject();
			LoginRequest loginRequest = new LoginRequest();
			loginRequest.Version = version;
			loginRequest.PrimaryAuthToken = getPrimaryAuthData(deviceID, PasswordUtil.MD5Password(sessionToken + deviceID));
			AuthType secondaryAuthType = getSecondaryAuthType();
			string verificationToken = GamingService.Instance.getVerificationToken();
			if (secondaryAuthType != 0 && !string.IsNullOrEmpty(verificationToken))
			{
				AuthToken authToken = new AuthToken();
				authToken.Type = secondaryAuthType;
				authToken.Data = verificationToken;
				loginRequest.SecondaryAuthToken.Add(authToken);
			}
			if (extraData == null)
			{
				extraData = new JObject();
			}
			extraData.Add("platform", getPlatform());
			loginRequest.ExtData = extraData.ToString(Formatting.None);
			PackDataAsBytes(ref outObj, loginRequest);
			return outObj;
		}

		public static string getPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				return "MacOS";
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.MetroPlayerX86:
			case RuntimePlatform.MetroPlayerX64:
				return "WinStore";
			case RuntimePlatform.MetroPlayerARM:
				return "WinPhone";
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.Android:
			case RuntimePlatform.SamsungTVPlayer:
				return "Android";
			default:
				return Application.platform.ToString();
			}
		}

		private AuthType getSecondaryAuthType()
		{
			return AuthType.GooglePlay;
		}

		public SFSProtocolResponse DecodeResponse(SFSObject sfsObj)
		{
			SFSProtocolResponse sFSProtocolResponse = new SFSProtocolResponse();
			sFSProtocolResponse.Init(sfsObj);
			return sFSProtocolResponse;
		}

		private AuthToken getPrimaryAuthData(string login, string password)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("login", login);
			dictionary.Add("password", password);
			AuthToken authToken = new AuthToken();
			authToken.Type = AuthType.Device;
			authToken.Data = Json.encode(dictionary);
			return authToken;
		}
	}
}
