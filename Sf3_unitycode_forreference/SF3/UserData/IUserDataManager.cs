using Google.Protobuf;

namespace SF3.UserData
{
	public interface IUserDataManager
	{
		void Initialize();

		void UpdateUserData(IMessage dataObject);
	}
}
