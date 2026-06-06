using Sfs2X.Entities.Data;
using Sfs2X.Util;

namespace Network.core.data
{
	public class SFSProtocolResponse
	{
		protected int _code;

		protected string _message;

		protected ByteArray _byteArray;

		public int Code
		{
			get
			{
				return _code;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public ByteArray ByteArray
		{
			get
			{
				return _byteArray;
			}
		}

		public void Init(SFSObject eparams)
		{
			_code = eparams.GetInt("c");
			_message = eparams.GetUtfString("m");
			_byteArray = eparams.GetByteArray("b");
		}
	}
}
