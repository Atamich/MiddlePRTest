using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarcodeLabelPrinting.Extensions
{
	public static class StringExtestion
	{
		public static List<string> WordWrap(this string text, int maxLineLength)
		{
			var list = new List<string>();
			int currentIndex;
			var lastWrap = 0;
			var whitespace = new[] { ' ', '\r', '\n', '\t' };
			do
			{
				currentIndex = lastWrap + maxLineLength > text.Length ? text.Length : (text.LastIndexOfAny(new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' }, Math.Min(text.Length - 1, lastWrap + maxLineLength)) + 1);
				if (currentIndex <= lastWrap)
					currentIndex = Math.Min(lastWrap + maxLineLength, text.Length);
				list.Add(text.Substring(lastWrap, currentIndex - lastWrap).Trim(whitespace));
				lastWrap = currentIndex;
			} while (currentIndex < text.Length);

			return list;
		}

		public static List<string> NewLinesToList(this string text)
		{
			text = text.Replace("\r", String.Empty);
			return text.Split("\n".ToCharArray()).ToList();
		}
	}
}