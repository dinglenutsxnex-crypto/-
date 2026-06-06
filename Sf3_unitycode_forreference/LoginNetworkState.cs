using Network.core.events;
using Newtonsoft.Json.Linq;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

public class LoginNetworkState : TCPNetworkState
{
	private static readonly string EMAIL_TAG = "PlayerEmail";

	private Coroutine timeoutRoutine;

	private LoginEmailDialogController dialog;

	public override void TCPStart(object data)
	{
		NetworkConnection.current.addEventListener("login", OnLoginSuccess);
		NetworkConnection.current.addEventListener("login_error", OnLoginFail);
		if (ShouldCheckEmail())
		{
			dialog = LoginEmailDialogController.ShowDialog(OnDialogClosed);
		}
		else
		{
			DoLogin();
		}
	}

	private void OnDialogClosed(string email)
	{
		PlayerPrefs.SetString(EMAIL_TAG, email);
		PlayerPrefs.Save();
		DoLogin();
	}

	private bool ShouldCheckEmail()
	{
		return NetworkConnection.Settings.CheckEmail && UserManager.GetPlayerID() < 0;
	}

	private void DoLogin()
	{
		dialog = null;
		JObject jObject = null;
		if (PlayerPrefs.HasKey(EMAIL_TAG))
		{
			jObject = new JObject();
			jObject["cbtEmail"] = PlayerPrefs.GetString(EMAIL_TAG);
		}
		if (NetworkConnection.current.IsConnectionActive())
		{
			timeoutRoutine = Routiner.GoDelayed(delegate
			{
				OnFail("Timeout");
				timeoutRoutine = null;
			}, NetworkConnection.Settings.LoginTimeout.ToSeconds());
			NetworkConnection.current.RequestLogin(jObject);
		}
		else
		{
			OnFail("Connection not alive to login");
		}
	}

	public override void TCPCleanup()
	{
		NetworkConnection.current.removeEventListener("login", OnLoginSuccess);
		NetworkConnection.current.removeEventListener("login_error", OnLoginFail);
		Routiner.Stop(timeoutRoutine);
		if (dialog != null)
		{
			dialog.CloseDialog();
			dialog = null;
		}
	}

	private void OnLoginSuccess(NetworkEvent e)
	{
		Debug.Log(string.Format("OnLogin - Guid: {0} requestID: {1}", e.message, e.requestID));
		OnSuccess(typeof(JoinZoneNetworkState), null);
	}

	private void OnLoginFail(NetworkEvent e)
	{
		e.HandleErrorAsDialog(RequestErrorCode.CbtInvalidEmail);
		OnFail("Login Failed " + e.message);
	}

	public override void TCPStop()
	{
		Disconnect();
	}
}
