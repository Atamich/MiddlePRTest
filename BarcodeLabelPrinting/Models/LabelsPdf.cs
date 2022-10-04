using BarcodeLabelPrinting.Extensions;
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

		const double cmToPoint = 28.3465; //Конф. сантиметров к поинтам

		const double labelHeightCm = 7; //Размеры в сантиметрах
		const double labelWIdthCm = 10;
		const double marginBase = 15;
		const double marginHorizntalCm = 0.3;
		const double marginVerticalCm = 0.6;

		const int labelsOnRow = 1; //Кол-во на одной строке
		const int rowsOnPage = 3; //Кол-во на одной странице

		private LabelsPdf() { }

		public LabelsPdf(Order order, XFont font ,XPen pen)
		{
			PdfDocument labelDoc = new PdfDocument();
			DrawDocument(order, labelDoc, font, pen);


			using (var outputStream = new MemoryStream())
			{
				labelDoc.Save(outputStream);
				FileBytes = outputStream.ToArray();
				labelDoc.Close();
				labelDoc.Dispose();
				outputStream.Flush();
				outputStream.Close();
				outputStream.Dispose();
			}
		}
		public LabelsPdf(List<Order> orders, XFont font, XPen pen)
		{
			PdfDocument labelDoc = new PdfDocument();
			foreach (var order in orders)
				DrawDocument(order, labelDoc, font, pen);


			using (var outputStream = new MemoryStream())
			{
				labelDoc.Save(outputStream);
				FileBytes = outputStream.ToArray();
				labelDoc.Close();
				labelDoc.Dispose();
				outputStream.Flush();
				outputStream.Close();
				outputStream.Dispose();
			}
		}

		private void DrawDocument(Order order, PdfDocument labelDoc, XFont font, XPen pen)
		{
			PdfPage currentPage = labelDoc.AddPage();
			XGraphics gfx = XGraphics.FromPdfPage(currentPage);

			int w = 0, h = 0;
			for (int i = 0; i < order.Items.Count; i++)
			{
				var orderItem = order.Items[i];
				DrawLabel(gfx, new XRect(
					marginBase + (w * (marginHorizntalCm * cmToPoint)) + (w * (labelWIdthCm * cmToPoint)), // позиция по горизонтали высчитывается тз текущего столбца - w, размера оступов, и наклеек
					marginBase + (h * (marginVerticalCm * cmToPoint)) + h * (labelHeightCm * cmToPoint), // Аналогично горизонтали высчитывается вертикальная позиция
					labelWIdthCm * cmToPoint,
					labelHeightCm * cmToPoint
					), orderItem, order, font, pen);

				//5 это отступ от начала страницы,
				//w - текущий столбец,
				//h - текущая строка,
				//marginCm - отступ между,
				//labelCm - Размер наклейки

				w++;

				if (i != 0 && i % labelsOnRow - 1 == 0 || labelsOnRow == 1) // new row
				{
					w = 0;
					h++;
				}
				if (h != 0 && h % rowsOnPage == 0 && i != order.Items.Count - 1) //new page
				{
					w = 0;
					h = 0;

					currentPage = labelDoc.AddPage();
					gfx = XGraphics.FromPdfPage(currentPage);
				}
			}
		}

		public static List<string> SaveLabelsToTempFolder(List<Order> orders, string tempFolderPath, bool mergeFiles)
		{
			var list = new List<string>();
			XFont font = new XFont("Segoe WP", 8, XFontStyle.Bold);
			XPen pen = new XPen(XColor.FromGrayScale(0.5), 0.5);
			if (mergeFiles)
			{
				using (var file = new LabelsPdf(orders, font, pen))
				{
					Directory.CreateDirectory($@"{tempFolderPath}/");
					file.FileName = $"{tempFolderPath}/Labels.pdf";
					File.WriteAllBytes(file.FileName, file.FileBytes);

					list.Add(file.FileName);
				}

				GC.Collect();
			}
			else
			{
				foreach (var order in orders)
				{
					using (var file = new LabelsPdf(order, font, pen))
					{
						Directory.CreateDirectory($@"{tempFolderPath}/{order.AddresseesContact}_{order.BillId}/");
						file.FileName = $"{tempFolderPath}/{order.AddresseesContact}_{order.BillId}/Labels_{order.BillId}.pdf";

						File.WriteAllBytes(file.FileName, file.FileBytes);
						list.Add(file.FileName);
					}

					GC.Collect();
				}
			}
			return list;
		}

		private void DrawLabel(XGraphics gfx, XRect rect, OrderItem orderItem, Order order,XFont font, XPen pen)
		{
			gfx.DrawRectangle(pen, rect);

			var marginLeft = rect.X + 12;
			var rowSize = rect.Height / 10;

			gfx.DrawString(order.AddresseesContact.ToUpper(), font, XBrushes.Black, marginLeft, rect.Y + rowSize * 2);
			gfx.DrawString(order.AddresseesAddress.ToUpper().WordWrap(60),font,XBrushes.Black, marginLeft, rect.Y + rowSize * 3);

			gfx.DrawString("Тел.: " + order.AddresseesTelephoneNumber, font, XBrushes.Black, marginLeft, rect.Y + rowSize * 5);
			gfx.DrawString("La Redoute", font, XBrushes.Black, marginLeft, rect.Y + rowSize * 6);

			gfx.DrawString(order.Carrier, font, XBrushes.Black, marginLeft, rect.Y + rowSize * 8);
			gfx.DrawString("EAN " + orderItem.Ean, font, XBrushes.Black, marginLeft, rect.Y + rowSize * 9);


			var barcode = BarCode.FromType(CodeType.Code3of9Standard, orderItem.Ean.ToString(), new XSize(4 * cmToPoint, rowSize * 2));
			barcode.TextLocation = TextLocation.Below;

			gfx.DrawBarCode(barcode, XBrushes.Black, font, new XPoint(rect.X + 5 * cmToPoint, rect.Y + rowSize * 5));
		}

		public void Dispose()
		{
			FileBytes = null;
		}
	}
}