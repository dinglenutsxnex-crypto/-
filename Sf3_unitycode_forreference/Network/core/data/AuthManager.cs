using UnityEngine;

namespace Network.core.data
{
	public class AuthManager
	{
		protected string login;

		protected bool UseGameCenter { get; set; }

		protected bool UseGooglePlay { get; set; }

		public virtual string Login
		{
			get
			{
				return login;
			}
			set
			{
				SetLogin(value);
			}
		}

		public virtual void Init()
		{
			login = GetLogin();
		}

		public virtual string GetLogin()
		{
			return ReadGuidFromKeyChain();
		}

		public virtual void SetLogin(string id)
		{
			if (IsRegisteredDevice())
			{
				WriteGuidToChain(id);
				login = id;
			}
		}

		private void WriteGuidToChain(string id)
		{
			KeyChainBinding.SetKeyChainData(Application.identifier, id);
		}

		private string ReadGuidFromKeyChain()
		{
			string applicationKey = string.Empty;
			string data = string.Empty;
			KeyChainBinding.GetKeyChainData(out applicationKey, out data);
			return data;
		}

		private bool IsRegisteredDevice()
		{
			return true;
		}

		public virtual void Clear()
		{
			login = string.Empty;
			KeyChainBinding.SetKeyChainData(Application.identifier, string.Empty);
		}
	}
}
