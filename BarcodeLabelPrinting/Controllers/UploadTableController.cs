using BarcodeLabelPrinting.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BarcodeLabelPrinting.Controllers
{
    [RoutePrefix("api")]
    public class UploadTableController : Controller
    {
        [Route("uploadtable")]
        [HttpPost]
        public ActionResult GetInvoices()
        {
            HttpFileCollectionBase files = Request.Files;
            try
            {
                if (!OrderItem.TryParseHttpFiles(files, out var invoices))
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;

                var invoiceFile = new InvoicePdf(invoices);

                return File(invoiceFile.FileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, "Invoice.pdf");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);//log
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }


    }
}