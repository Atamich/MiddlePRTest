using BarcodeLabelPrinting.Models;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarcodeLabelPrinting.Extensions
{
	public static class PdfSharpExtension
	{
		public static void DrawWrappedString(this XGraphics gfx, string str, double x, double yStarted, int charsOnLine, XFont font) {
			List<string> list = str.WordWrap(charsOnLine);
			for (int i = 0; i < list.Count; i++)
			{
				gfx.DrawString(list[i], font, XBrushes.Black, x, yStarted + i * (font.Size+2));
			}
		}
		}
	}