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
		public static double DrawString(this XGraphics gfx, List<string> list, XFont font, XBrush brush,double x, double y,double rowSize = 9)
		{
			double currentY = y;
			for (int i = 0; i < list.Count; i++)
			{
				gfx.DrawString(list[i], font, brush, x, currentY);
				currentY += rowSize;
			}
			return currentY;
		}
	}
}