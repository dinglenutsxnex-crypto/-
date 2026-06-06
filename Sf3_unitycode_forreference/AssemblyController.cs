using System.ComponentModel;
using System.Xml;

public class AssemblyController : ExtentionBehaviour
{
	public class Market
	{
		[DefaultValue(false)]
		public bool isSteam { get; private set; }

		[DefaultValue(false)]
		public bool isChina { get; private set; }

		[DefaultValue(false)]
		public bool isJapan { get; private set; }

		[DefaultValue(false)]
		public bool isKorea { get; private set; }

		[DefaultValue(false)]
		public bool isAmazon { get; private set; }

		[DefaultValue(false)]
		public bool isAmazonMobile { get; private set; }

		public void Parse(XmlNode node)
		{
			XmlElement xmlElement = node["China"];
			if (xmlElement != null)
			{
				isChina = bool.Parse(xmlElement.Attributes[0].InnerText);
			}
			xmlElement = node["Japan"];
			if (xmlElement != null)
			{
				isJapan = bool.Parse(xmlElement.Attributes[0].InnerText);
			}
			xmlElement = node["Korea"];
			if (xmlElement != null)
			{
				isKorea = bool.Parse(xmlElement.Attributes[0].InnerText);
			}
			xmlElement = node["Amazon"];
			if (xmlElement != null)
			{
				isAmazon = bool.Parse(xmlElement.Attributes[0].InnerText);
			}
			xmlElement = node["AmazonMobile"];
			if (xmlElement != null)
			{
				isAmazonMobile = bool.Parse(xmlElement.Attributes[0].InnerText);
			}
			xmlElement = node["Steam"];
			if (xmlElement != null)
			{
				isSteam = bool.Parse(xmlElement.Attributes[0].InnerText);
			}
		}
	}

	private static readonly Market _market = new Market();

	public static bool showIntro
	{
		get
		{
			return true;
		}
	}

	public static bool showController
	{
		get
		{
			return true;
		}
	}

	public static bool showPVP
	{
		get
		{
			return false;
		}
	}

	public static bool aiEnabled
	{
		get
		{
			return true;
		}
	}

	public static bool skipContentDownload
	{
		get
		{
			return false;
		}
	}

	public static bool skipPayment
	{
		get
		{
			return false;
		}
	}

	public static bool forge
	{
		get
		{
			return false;
		}
	}

	public static bool showSensetiveArea
	{
		get
		{
			return false;
		}
	}

	public static Market market
	{
		get
		{
			return _market;
		}
	}
}
