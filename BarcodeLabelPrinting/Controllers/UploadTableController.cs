using BarcodeLabelPrinting.Interfaces;
using BarcodeLabelPrinting.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BarcodeLabelPrinting.Controllers
{
    [RoutePrefix("upload")]
    public class UploadTableController : Controller
    {
        [Route("table")]
        [HttpPost]
        public ActionResult GetInvoices()
        {
            HttpFileCollectionBase files = Request.Files;
            try
            {
                if (!InputTableRow.TryParseHttpFiles(files, out var orders))
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;


                List<IFile> responseFiles = new List<IFile> { new InvoicePdf(orders), new LabelsPdf(orders) };

                using (var compressedFileStream = new MemoryStream())
                {
                    //Create an archive and store the stream in memory.
                    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                    {
                        foreach (var file in responseFiles)
                        {
                            //Create a zip entry for each attachment
                            var zipEntry = zipArchive.CreateEntry(file.FileName);

                            //Get the stream of the attachment
                            using (var originalFileStream = new MemoryStream(file.FileBytes))
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                        }
                    }

                    return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = "Filename.zip" };
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);//log
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }

        [Route("samplefile")]
        [HttpGet]
        public ActionResult GetSampleFile()
        {
            try
            {
                var file = Resources.Resource.Example;
                byte[] fileBytes = Encoding.UTF8.GetBytes(file);
                return File(fileBytes, "text/csv","Example.csv");

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