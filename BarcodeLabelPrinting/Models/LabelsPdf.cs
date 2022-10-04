using BarcodeLabelPrinting.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Drawing.BarCodes;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace BarcodeLabelPrinting.Models
{
	public class LabelsPdf : IFile
	{
		// 1cm - 28.3465
		public byte[] FileBytes { get; private set; }
		public string FileName { get; set; } = "Labels.pdf";

		readonly XFont font = new XFont("Segoe WP", 8, XFontStyle.Bold);

		readonly XPen pen = new XPen(XColor.FromGrayScale(0.5), 0.5);

		const double cmToPoint = 28.3465; //Конф. сантиметров к поинтам

		const double labelHeightCm = 7; //Размеры в сантиметрах
		const double labelWIdthCm = 10;

		const double marginHorizntalCm = 0.3;
		const double marginVerticalCm = 0.6;

		const int labelsOnRow = 2; //Кол-во на одной строке
		const int rowsOnPage = 2; //Кол-во на одной странице

		private LabelsPdf() { }

		public LabelsPdf(List<Order> orders)
		{
			PdfDocument labelDoc = new PdfDocument();
			foreach (var order in orders)
			{
				PdfPage currentPage = labelDoc.AddPage();
				XGraphics gfx = XGraphics.FromPdfPage(currentPage);

				int w = 0, h = 0;
				for (int i = 0; i < order.Items.Count; i++)
				{
					var orderItem = order.Items[i];
					DrawLabel(gfx, new XRect(
						5+(w * (marginHorizntalCm * cmToPoint)) + (w * (labelWIdthCm * cmToPoint)), // позиция по горизонтали высчитывается тз текущего столбца - w, размера оступов, и наклеек
						5+(h * (marginVerticalCm * cmToPoint)) + h * (labelHeightCm * cmToPoint), // Аналогично горизонтали высчитывается вертикальная позиция
						labelWIdthCm * cmToPoint,
						labelHeightCm * cmToPoint
						), orderItem, order);

					//5 это отступ от начала страницы,
					//w - текущий столбец,
					//h - текущая строка,
					//marginCm - отступ между,
					//labelCm - Размер наклейки

					w++;

					if (i != 0 && i % labelsOnRow - 1 == 0) // new row
					{
						w = 0;
						h++;
					}
					if (h != 0 && h % rowsOnPage == 0) //new page
					{
						w = 0;
						h = 0;

						currentPage = labelDoc.AddPage();
						gfx = XGraphics.FromPdfPage(currentPage);
					}
				}

			}

			using (var outputStream = new MemoryStream())
			{
				labelDoc.Save(outputStream);
				FileBytes = outputStream.ToArray();
			}
		}

		private void DrawLabel(XGraphics gfx, XRect rect, OrderItem orderItem, Order order)
		{
			gfx.DrawRectangle(pen, rect);

			var marginLeft = rect.X + 12;
			var rowSize = rect.Height / 10;

			gfx.DrawString(order.AddresseesContact.ToUpper(), font, XBrushes.Black, marginLeft, rect.Y + rowSize * 2);
			gfx.DrawString(order.AddresseesAddress.ToUpper(), font, XBrushes.Black, marginLeft, rect.Y + rowSize * 3);

			gfx.DrawString("Тел.: " + order.AddresseesTelephoneNumber, font, XBrushes.Black, marginLeft, rect.Y + rowSize * 5);
			gfx.DrawString("La Redoute", font, XBrushes.Black, marginLeft, rect.Y + rowSize * 6);

			gfx.DrawString("PEC", font, XBrushes.Black, marginLeft, rect.Y + rowSize * 8);
			gfx.DrawString("EAN " + orderItem.Ean, font, XBrushes.Black, marginLeft, rect.Y + rowSize * 9);


			var barcode = BarCode.FromType(CodeType.Code3of9Standard, orderItem.Ean.ToString(), new XSize(4 * cmToPoint, rowSize * 2));
			barcode.TextLocation = TextLocation.Below;

			gfx.DrawBarCode(barcode, XBrushes.Black, font, new XPoint(rect.X + 5 * cmToPoint, rect.Y + rowSize * 5));

			//gfx.DrawString(orderItem.Ean.ToString(), font, XBrushes.Black,  barcode.Size.Width / 2 - gfx.MeasureString(orderItem.Ean.ToString(), font).Width / 2, rect.Y + rowSize * .2 +);
		}
	}
}