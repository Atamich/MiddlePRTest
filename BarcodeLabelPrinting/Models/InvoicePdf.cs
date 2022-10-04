using BarcodeLabelPrinting.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BarcodeLabelPrinting.Models
{
    public class InvoicePdf : IFile
    {
        public byte[] FileBytes { get; private set; }
        public string FileName { get; set; } = "Invoice.pdf";

		readonly int[] colPositionsX = { 35, 120, 160, 215, 282, 340, 375 };

        const int StartY = 160;
        const int rowSize = 10;
        const int totalRecordsOnPage = 20;

        readonly XFont font = new XFont("Segoe WP", 8);

        private InvoicePdf() { }

        public InvoicePdf(List<OrderItem> orderItems)
        {
            PdfDocument invoiceDoc = new PdfDocument();
            PdfPage presetPage = GetInvoicePreset();
            invoiceDoc.AddPage(presetPage);
            PdfPage currentPage = invoiceDoc.Pages[0];

            XGraphics gfx = XGraphics.FromPdfPage(currentPage);
            int y = StartY;

            for (int i = 0; i < orderItems.Count; i++)
            {
                var orderItem = orderItems[i];

                int xi = 0;
                foreach (var propItem in orderItem) // step by every property and draw using position from const
                {
                    gfx.DrawString(propItem, font, XBrushes.Black,
                        colPositionsX[xi++],
                        y);
                }

                y += rowSize;

                if (i != 0 && i % totalRecordsOnPage == 0) //every 20 record adds new page
                {
                    y = StartY;

                    invoiceDoc.AddPage(presetPage);
                    currentPage = invoiceDoc.Pages[invoiceDoc.PageCount];

                    gfx = XGraphics.FromPdfPage(currentPage);
                }
            }

            using (var outputStream = new MemoryStream())
            {
                invoiceDoc.Save(outputStream);
                FileBytes = outputStream.ToArray();

            }
        }

        private static PdfPage GetInvoicePreset()
        {
            Stream invoicePreset = new MemoryStream(Resources.Resource.InvoicePreset);
            PdfDocument invoiceDoc = PdfReader.Open(invoicePreset, PdfDocumentOpenMode.Import);
            var page = invoiceDoc.Pages[0].Copy();
            return page;
        }
    }
}
