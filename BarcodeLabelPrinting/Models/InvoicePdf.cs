using BarcodeLabelPrinting.Extensions;
using BarcodeLabelPrinting.Interfaces;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Web;
using System.Xml.XPath;

namespace BarcodeLabelPrinting.Models
{
    public class InvoicePdf : IFile
    {
        public byte[] FileBytes { get; private set; }
        public string FileName { get; set; } = "Invoice.pdf";

		readonly int[] colPositionsX = { 35, 135, 176, 230, 298, 358, 386 };

        const int StartY = 180;
        const int rowSize = 10;
        const int rowMargTop = 5;
		const int totalRecordsOnPage = 15;
        XFont fontSmall = new XFont("Segoe WP", 7);
        XFont fontMedium = new XFont("Segoe WP", 9);

        private InvoicePdf() { }

		public InvoicePdf(Order order,string underInvoiceText)
		{
			PdfDocument invoiceDoc = new PdfDocument();
			DrawDocument(invoiceDoc, order, underInvoiceText);
			using (var outputStream = new MemoryStream())
			{
				invoiceDoc.Save(outputStream);
				FileBytes = outputStream.ToArray();
				invoiceDoc.Close();
				outputStream.Flush();
				outputStream.Close();
			}
		}
		public InvoicePdf(List<Order> orders, string underInvoiceText)
		{
			PdfDocument invoiceDoc = new PdfDocument();
			foreach (var order in orders)
				DrawDocument(invoiceDoc, order, underInvoiceText);

			using (var outputStream = new MemoryStream())
			{
				invoiceDoc.Save(outputStream);
				FileBytes = outputStream.ToArray();
				invoiceDoc.Close();
				outputStream.Flush();
				outputStream.Close();
			}
		}

		public static List<string> SaveInvoicesToTempFolder(List<Order> orders, string underInvoiceText, string tempFolderPath, bool mergeFiles)
		{
			var list = new List<string>();
			if (mergeFiles)
			{
				using (var file = new InvoicePdf(orders, underInvoiceText))
				{
					Directory.CreateDirectory($@"{tempFolderPath}/");
					file.FileName = $"{tempFolderPath}/Invoices.pdf";
					File.WriteAllBytes(file.FileName, file.FileBytes);

					list.Add(file.FileName);
				}

				GC.Collect();
			}
			else
			{
				foreach (var order in orders)
				{
					using (var file = new InvoicePdf(order, underInvoiceText))
					{
						Directory.CreateDirectory($@"{tempFolderPath}/{order.AddresseesContact}_{order.BillId}/");
						file.FileName = $"{tempFolderPath}/{order.AddresseesContact}_{order.BillId}/Invoice_{order.BillId}.pdf";
						File.WriteAllBytes(file.FileName, file.FileBytes);

						list.Add(file.FileName);
					}	

					GC.Collect();
				}
			}
			
			return list;
		}

		private void DrawDocument(PdfDocument invoiceDoc, Order order, string underInvoiceText)
		{
			PdfPage presetPage = GetInvoicePreset();
			invoiceDoc.AddPage(presetPage);
			PdfPage currentPage = invoiceDoc.Pages[invoiceDoc.PageCount - 1];
			XGraphics gfx = XGraphics.FromPdfPage(currentPage);

			DrawOrderInfo(gfx, order, underInvoiceText);

			int y = StartY;
			int additionalRowsCount = 0;
			for (int i = 0; i < order.Items.Count; i++)
			{
				var orderItem = order.Items[i];

				int xi = 0;
				int biggestPropRow = 1;
				foreach (var propItem in orderItem) // Отрисовка в разных позициях, реализация колонок
				{
					var list = propItem.WordWrap(25);
					if (list.Count > biggestPropRow) biggestPropRow = list.Count;

					for (int pr = 0; pr < list.Count; pr++)
					{
						gfx.DrawString(list[pr], fontSmall, XBrushes.Black,
						colPositionsX[xi],
						y + (pr * rowSize) + rowMargTop);
					}
					xi++;
				}
				additionalRowsCount += biggestPropRow;
				y += rowSize * biggestPropRow;

				if (totalRecordsOnPage - additionalRowsCount < 0) //every 20 record adds new page
				{
					y = StartY;
					additionalRowsCount = 0;
					invoiceDoc.AddPage(presetPage);
					currentPage = invoiceDoc.Pages[invoiceDoc.PageCount - 1];
					gfx = XGraphics.FromPdfPage(currentPage);
					DrawOrderInfo(gfx, order, underInvoiceText);

				}
			}
		}

		private void DrawOrderInfo(XGraphics gfx, Order order, string underInvoiceText)
		{
			gfx.DrawString(underInvoiceText.NewLinesToList(), fontMedium, XBrushes.Black, 30, 380, 9);

			gfx.DrawString(order.AddresseesAddress.WordWrap(25), fontSmall, XBrushes.Black, 30, 68);

			gfx.DrawString(order.ClientId.ToString(), fontSmall, XBrushes.Black, 366, 65);
			gfx.DrawString(order.OrderId.ToString(), fontSmall, XBrushes.Black, 366, 75);
			gfx.DrawString(order.Date.ToString("d"), fontSmall, XBrushes.Black, 366, 85);
			gfx.DrawString(order.ParcelId, fontSmall, XBrushes.Black, 366, 95);
			gfx.DrawString(order.BillType, fontSmall, XBrushes.Black, 366, 105);
			gfx.DrawString(order.CurrencyType, fontSmall, XBrushes.Black, 366, 115);

			gfx.DrawString(order.TotalPrice.ToString("0.##"), fontSmall, XBrushes.Black, 381, 358);
			gfx.DrawString(order.ServiceFee.ToString(), fontSmall, XBrushes.Black, 381, 368);

			var serviceFeeNds = Double.Parse(ConfigurationManager.AppSettings["serviceFeeNds"]);
			var ndsSum = (order.ServiceFee * serviceFeeNds) + (order.TotalPrice * (order.Nds / 100));

			var sum = order.TotalPrice + order.ServiceFee + ndsSum;

			gfx.DrawString(sum.ToString("0.##"), fontSmall, XBrushes.Black, 381, 417);
			gfx.DrawString(ndsSum.ToString("0.##"), fontSmall, XBrushes.Black, 381, 428);
		}

		private static PdfPage GetInvoicePreset()
        {
			using (Stream stream = new MemoryStream(Resources.Resource.InvoicePreset))
			{
				PdfDocument invoiceDoc = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
				var page = invoiceDoc.Pages[0].Copy();
				invoiceDoc.Close();
				invoiceDoc.Dispose();
				return page;
			}
        }

		public void Dispose()
		{
			fontSmall = null;
			fontMedium = null;
			FileBytes = null;
		}
	}
}
