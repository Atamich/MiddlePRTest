using BarcodeLabelPrinring.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BarcodeLabelPrinring.Controllers
{
    public class UploadTableController : ApiController
    {
        private readonly Stream invoicePreset = new MemoryStream(Resources.Resource.InvoicePreset);

        [Route("api/uploadTable")]
        public HttpResponseMessage Post()
        {
            PdfDocument invoiceDoc = new PdfDocument(invoicePreset);
            PdfPage invoicePage = new PdfPage(invoiceDoc);
            XGraphics gfx = XGraphics.FromPdfPage(invoicePage);
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold);

            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            if(!Invoice.TryParseHttpFiles(httpRequest.Files, out var invoices))
                Request.CreateResponse(HttpStatusCode.BadRequest);

            foreach (var invoice in invoices)
            {
                // Draw the text
                gfx.DrawString($"{invoice.Id} {invoice.Name}", font, XBrushes.Black,
                  new XRect(0, 0, invoicePage.Width, invoicePage.Height),  XStringFormat.Center); //XRect(0, 0, invoicePage.Width, invoicePage.Height), вынести
            }
            invoiceDoc.Save("Inv.pdf");
            result = Request.CreateResponse(HttpStatusCode.OK);
            return result;
        }


    }
}
