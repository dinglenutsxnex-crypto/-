using System.Globalization;
using System.Text;

namespace SF3
{
	public class StringWrapper
	{
		private readonly StringBuilder _builder;

		private readonly string _separator = "--------------------";

		private bool _autoEndWrap = true;

		public StringWrapperPurpose WrapperType { get; private set; }

		private StringWrapper(StringWrapperPurpose purpose)
		{
			WrapperType = purpose;
			_builder = new StringBuilder();
			Fill();
		}

		public static StringWrapper Create(object o)
		{
			StringWrapper stringWrapper = Create();
			stringWrapper.Head(o.GetType().Name);
			return stringWrapper;
		}

		public static StringWrapper Create()
		{
			return new StringWrapper(StringWrapperPurpose.NoDecoration);
		}

		public static StringWrapper Create(StringWrapperPurpose purpose)
		{
			return new StringWrapper(purpose);
		}

		public static StringWrapper Create(StringWrapperPurpose purpose, string msg)
		{
			StringWrapper stringWrapper = new StringWrapper(purpose);
			stringWrapper.Wrap(msg);
			return stringWrapper;
		}

		private void Fill()
		{
			switch (WrapperType)
			{
			case StringWrapperPurpose.Log:
				Wrap(GetStringColored("LOG ", "green"));
				break;
			case StringWrapperPurpose.Warning:
				Wrap(GetStringColored("WARNING ", "yellow"));
				break;
			case StringWrapperPurpose.Error:
				Wrap(GetStringColored("ERROR ", "red"));
				break;
			}
		}

		public void Head(string sentence)
		{
			_builder.Append("[" + sentence + "]:\n");
		}

		public void Wrap(string name, bool value)
		{
			Wrap(name, value.ToString());
		}

		public void Wrap(string name, double value)
		{
			Wrap(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public void Wrap(string sentence)
		{
			_builder.Append(sentence);
		}

		public void Wrap(string name, long value)
		{
			Wrap(name, value.ToString());
		}

		public void Wrap(string name, string value)
		{
			_builder.Append("<" + name + " : " + value + ">\n");
		}

		public void Separator()
		{
			_autoEndWrap = false;
			_builder.Append(_separator + '\n');
		}

		public void Add(string sentence)
		{
			_builder.Append(sentence);
		}

		public void End()
		{
			_autoEndWrap = false;
			_builder.Append('\n');
		}

		public override string ToString()
		{
			if (_autoEndWrap)
			{
				End();
			}
			return _builder.ToString();
		}

		public static string GetStringColored(string sentence, string color)
		{
			return string.Format("<color={0}>{1}</color>", color, sentence);
		}
	}
}
