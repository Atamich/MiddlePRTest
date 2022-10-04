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
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.XPath;

namespace BarcodeLabelPrinting.Models
{
    public class InvoicePdf : IFile
    {
        public byte[] FileBytes { get; private set; }
        public string FileName { get; set; } = "Invoice.pdf";

		readonly int[] colPositionsX = { 25, 120, 160, 215, 282, 340, 370 };

        const int StartY = 160;
        const int rowSize = 10;
        const int rowMargTop = 5;
		const int totalRecordsOnPage = 20;
        readonly XFont font = new XFont("Segoe WP", 7);

        private InvoicePdf() { }

        public InvoicePdf(List<Order> orders)
        {
            PdfDocument invoiceDoc = new PdfDocument();
			foreach (var order in orders)
			{
				PdfPage presetPage = GetInvoicePreset();
				invoiceDoc.AddPage(presetPage);
				PdfPage currentPage = invoiceDoc.Pages[invoiceDoc.PageCount-1];
				XGraphics gfx = XGraphics.FromPdfPage(currentPage);
				DrawOrderInfo(gfx, order);


				int y = StartY;

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
							gfx.DrawString(list[pr], font, XBrushes.Black,
							colPositionsX[xi],
							y+(pr*rowSize)+rowMargTop*i);
						}
						xi++;
					}

					y += rowSize* biggestPropRow;

					if (i != 0 && i % totalRecordsOnPage == 0) //every 20 record adds new page
					{
						y = StartY;

						invoiceDoc.AddPage(presetPage);
						currentPage = invoiceDoc.Pages[invoiceDoc.PageCount];
						gfx = XGraphics.FromPdfPage(currentPage);
						DrawOrderInfo(gfx, order);

					}
				}
			}

            using (var outputStream = new MemoryStream())
            {
                invoiceDoc.Save(outputStream);
                FileBytes = outputStream.ToArray();
            }
        }

		private void DrawOrderInfo(XGraphics gfx, Order order)
		{
			gfx.DrawWrappedString(order.AddresseesAddress,14,48,25,font);

			gfx.DrawString(order.ClientId.ToString(), font, XBrushes.Black, 350, 45);
			gfx.DrawString(order.OrderId.ToString(), font, XBrushes.Black, 350, 55);
			gfx.DrawString(order.Date.ToString("d"), font, XBrushes.Black, 350, 65);
			gfx.DrawString(order.ParcelId, font, XBrushes.Black, 350, 75);
			gfx.DrawString(order.BillType, font, XBrushes.Black, 350, 85);
			gfx.DrawString(order.CurrencyType, font, XBrushes.Black, 350, 95);

			gfx.DrawString(order.TotalPrice.ToString("0.##"), font, XBrushes.Black, 365, 336);
			gfx.DrawString(order.ServiceFee.ToString(), font, XBrushes.Black, 365, 346);

			gfx.DrawString(order.TotalPrice.ToString("0.##"), font, XBrushes.Black, 365, 396);
			gfx.DrawString(order.Nds.ToString(), font, XBrushes.Black, 365, 407);
		}

		private static PdfPage GetInvoicePreset()
        {
            Stream invoicePreset = new MemoryStream(Resources.Resource.InvoicePreset);
            PdfDocument invoiceDoc = PdfReader.Open(invoicePreset, PdfDocumentOpenMode.Import);
            var page = invoiceDoc.Pages[0].Copy();
            return page;
        }

		#region migraDoc
		//      private void DefineStyles(Document document)
		//{
		//          // Get the predefined style Normal.
		//          Style style = document.Styles["Normal"];
		//          // Because all styles are derived from Normal, the next line changes the 
		//          // font of the whole document. Or, more exactly, it changes the font of
		//          // all styles and paragraphs that do not redefine the font.
		//          style.Font.Name = "Verdana";

		//          style = document.Styles[StyleNames.Header];
		//          style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

		//          style = document.Styles[StyleNames.Footer];
		//          style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

		//          // Create a new style called Table based on style Normal
		//          style = document.Styles.AddStyle("Table", "Normal");
		//          style.Font.Name = "Arial";
		//          style.Font.Name = "Arial";
		//          style.Font.Size = 9;

		//          // Create a new style called Reference based on style Normal
		//          style = document.Styles.AddStyle("Reference", "Normal");
		//          style.ParagraphFormat.SpaceBefore = "5mm";
		//          style.ParagraphFormat.SpaceAfter = "5mm";
		//          style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
		//      }

		//      void CreatePage(Document document)
		//      {
		//          // Each MigraDoc document needs at least one section.
		//          Section section = document.AddSection();

		//          // Put a logo in the header
		//          Image image = section.Headers.Primary.AddImage("../../PowerBooks.png");
		//          image.Height = "2.5cm";
		//          image.LockAspectRatio = true;
		//          image.RelativeVertical = RelativeVertical.Line;
		//          image.RelativeHorizontal = RelativeHorizontal.Margin;
		//          image.Top = ShapePosition.Top;
		//          image.Left = ShapePosition.Right;
		//          image.WrapFormat.Style = WrapStyle.Through;

		//          // Create footer
		//          Paragraph paragraph = section.Footers.Primary.AddParagraph();
		//          paragraph.AddText("PowerBooks Inc · Sample Street 42 · 56789 Cologne · Germany");
		//          paragraph.Format.Font.Size = 9;
		//          paragraph.Format.Alignment = ParagraphAlignment.Center;

		//          // Create the text frame for the address
		//          var addressFrame = section.AddTextFrame();
		//          addressFrame.Height = "3.0cm";
		//          addressFrame.Width = "7.0cm";
		//          addressFrame.Left = ShapePosition.Left;
		//          addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
		//          addressFrame.Top = "5.0cm";
		//          addressFrame.RelativeVertical = RelativeVertical.Page;

		//          // Put sender in address frame
		//          paragraph = addressFrame.AddParagraph("PowerBooks Inc · Sample Street 42 · 56789 Cologne");
		//          paragraph.Format.Font.Name = "Times New Roman";
		//          paragraph.Format.Font.Size = 7;
		//          paragraph.Format.SpaceAfter = 3;

		//          // Add the print date field
		//          paragraph = section.AddParagraph();
		//          paragraph.Format.SpaceBefore = "8cm";
		//          paragraph.Style = "Reference";
		//          paragraph.AddFormattedText("INVOICE", TextFormat.Bold);
		//          paragraph.AddTab();
		//          paragraph.AddText("Cologne, ");
		//          paragraph.AddDateField("dd.MM.yyyy");

		//          // Create the item table
		//          var table = section.AddTable();
		//          table.Style = "Table";
		//          table.Borders.Color = TableBorder;
		//          table.Borders.Width = 0.25;
		//          table.Borders.Left.Width = 0.5;
		//          table.Borders.Right.Width = 0.5;
		//          table.Rows.LeftIndent = 0;

		//          // Before you can add a row, you must define the columns
		//          Column column = table.AddColumn("1cm");
		//          column.Format.Alignment = ParagraphAlignment.Center;

		//          column = table.AddColumn("2.5cm");
		//          column.Format.Alignment = ParagraphAlignment.Right;

		//          column = table.AddColumn("3cm");
		//          column.Format.Alignment = ParagraphAlignment.Right;

		//          column = table.AddColumn("3.5cm");
		//          column.Format.Alignment = ParagraphAlignment.Right;

		//          column = table.AddColumn("2cm");
		//          column.Format.Alignment = ParagraphAlignment.Center;

		//          column = table.AddColumn("4cm");
		//          column.Format.Alignment = ParagraphAlignment.Right;

		//          // Create the header of the table
		//          Row row = table.AddRow();
		//          row.HeadingFormat = true;
		//          row.Format.Alignment = ParagraphAlignment.Center;
		//          row.Format.Font.Bold = true;
		//          row.Shading.Color = TableBlue;
		//          row.Cells[0].AddParagraph("Item");
		//          row.Cells[0].Format.Font.Bold = false;
		//          row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
		//          row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
		//          row.Cells[0].MergeDown = 1;
		//          row.Cells[1].AddParagraph("Title and Author");
		//          row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
		//          row.Cells[1].MergeRight = 3;
		//          row.Cells[5].AddParagraph("Extended Price");
		//          row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
		//          row.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
		//          row.Cells[5].MergeDown = 1;

		//          row = table.AddRow();
		//          row.HeadingFormat = true;
		//          row.Format.Alignment = ParagraphAlignment.Center;
		//          row.Format.Font.Bold = true;
		//          row.Shading.Color = TableBlue;
		//          row.Cells[1].AddParagraph("Quantity");
		//          row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
		//          row.Cells[2].AddParagraph("Unit Price");
		//          row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
		//          row.Cells[3].AddParagraph("Discount (%)");
		//          row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
		//          row.Cells[4].AddParagraph("Taxable");
		//          row.Cells[4].Format.Alignment = ParagraphAlignment.Left;

		//          table.SetEdge(0, 0, 6, 2, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
		//      }

		//      void FillContent()
		//      {
		//          // Fill address in address text frame
		//          XPathNavigator item = SelectItem("/invoice/to");
		//          Paragraph paragraph = addressFrame.AddParagraph();
		//          paragraph.AddText(GetValue(item, "name/singleName"));
		//          paragraph.AddLineBreak();
		//          paragraph.AddText(GetValue(item, "address/line1"));
		//          paragraph.AddLineBreak();
		//          paragraph.AddText(GetValue(item, "address/postalCode") + " " + GetValue(item, "address/city"));

		//          // Iterate the invoice items
		//          double totalExtendedPrice = 0;
		//          XPathNodeIterator iter = navigator.Select("/invoice/items/*");
		//          while (iter.MoveNext())
		//          {
		//              item = iter.Current;
		//              double quantity = GetValueAsDouble(item, "quantity");
		//              double price = GetValueAsDouble(item, "price");
		//              double discount = GetValueAsDouble(item, "discount");

		//              // Each item fills two rows
		//              Row row1 = table.AddRow();
		//              Row row2 = table.AddRow();
		//              row1.TopPadding = 1.5;
		//              row1.Cells[0].Shading.Color = TableGray;
		//              row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
		//              row1.Cells[0].MergeDown = 1;
		//              row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
		//              row1.Cells[1].MergeRight = 3;
		//              row1.Cells[5].Shading.Color = TableGray;
		//              row1.Cells[5].MergeDown = 1;

		//              row1.Cells[0].AddParagraph(GetValue(item, "itemNumber"));
		//              paragraph = row1.Cells[1].AddParagraph();
		//              paragraph.AddFormattedText(GetValue(item, "title"), TextFormat.Bold);
		//              paragraph.AddFormattedText(" by ", TextFormat.Italic);
		//              paragraph.AddText(GetValue(item, "author"));
		//              row2.Cells[1].AddParagraph(GetValue(item, "quantity"));
		//              row2.Cells[2].AddParagraph(price.ToString("0.00") + " €");
		//              row2.Cells[3].AddParagraph(discount.ToString("0.0"));
		//              row2.Cells[4].AddParagraph();
		//              row2.Cells[5].AddParagraph(price.ToString("0.00"));
		//              double extendedPrice = quantity * price;
		//              extendedPrice = extendedPrice * (100 - discount) / 100;
		//              row1.Cells[5].AddParagraph(extendedPrice.ToString("0.00") + " €");
		//              row1.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
		//              totalExtendedPrice += extendedPrice;

		//              table.SetEdge(0, table.Rows.Count - 2, 6, 2, Edge.Box, BorderStyle.Single, 0.75);
		//          }

		//          // Add an invisible row as a space line to the table
		//          Row row = this.table.AddRow();
		//          row.Borders.Visible = false;

		//          // Add the total price row
		//          row = table.AddRow();
		//          row.Cells[0].Borders.Visible = false;
		//          row.Cells[0].AddParagraph("Total Price");
		//          row.Cells[0].Format.Font.Bold = true;
		//          row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
		//          row.Cells[0].MergeRight = 4;
		//          row.Cells[5].AddParagraph(totalExtendedPrice.ToString("0.00") + " €");

		//          // Add the VAT row
		//          row = table.AddRow();
		//          row.Cells[0].Borders.Visible = false;
		//          row.Cells[0].AddParagraph("VAT (19%)");
		//          row.Cells[0].Format.Font.Bold = true;
		//          row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
		//          row.Cells[0].MergeRight = 4;
		//          row.Cells[5].AddParagraph((0.19 * totalExtendedPrice).ToString("0.00") + " €");

		//          // Add the additional fee row
		//          row = table.AddRow();
		//          row.Cells[0].Borders.Visible = false;
		//          row.Cells[0].AddParagraph("Shipping and Handling");
		//          row.Cells[5].AddParagraph(0.ToString("0.00") + " €");
		//          row.Cells[0].Format.Font.Bold = true;
		//          row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
		//          row.Cells[0].MergeRight = 4;

		//          // Add the total due row
		//          row = table.AddRow();
		//          row.Cells[0].AddParagraph("Total Due");
		//          row.Cells[0].Borders.Visible = false;
		//          row.Cells[0].Format.Font.Bold = true;
		//          row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
		//          row.Cells[0].MergeRight = 4;
		//          totalExtendedPrice += 0.19 * totalExtendedPrice;
		//          row.Cells[5].AddParagraph(totalExtendedPrice.ToString("0.00") + " €");

		//          // Set the borders of the specified cell range
		//          table.SetEdge(5, table.Rows.Count - 4, 1, 4, Edge.Box, BorderStyle.Single, 0.75);

		//          // Add the notes paragraph
		//          paragraph = document.LastSection.AddParagraph();
		//          paragraph.Format.SpaceBefore = "1cm";
		//          paragraph.Format.Borders.Width = 0.75;
		//          paragraph.Format.Borders.Distance = 3;
		//          paragraph.Format.Borders.Color = TableBorder;
		//          paragraph.Format.Shading.Color = TableGray;
		//          item = SelectItem("/invoice");
		//          paragraph.AddText(GetValue(item, "notes"));
		//      }
		#endregion
	}
}
