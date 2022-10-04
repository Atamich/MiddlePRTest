using BarcodeLabelPrinting.Interfaces;
using BarcodeLabelPrinting.Models;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BarcodeLabelPrinting.Controllers
{
    [RoutePrefix("upload")]
    [Authorize]
    public class UploadTableController : Controller
    {
        [Route("table")]
        [HttpPost]
        public ActionResult GetInvoices()
        {
            HttpFileCollectionBase files = Request.Files;
            var underInvoiceText = Request.Form.Get("underInvoiceText");
            bool mergeFiles = bool.Parse(Request.Form.Get("mergeFiles"));
            try
            {
                if (!InputTableRow.TryParseHttpFiles(files, out var orders))
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;

                if (orders.Count < 1)
                    throw new Exception("Не удалось прочитать ни одну строку в файле");

                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

                var guid = Guid.NewGuid();
                var tempFolderPath = ConfigurationManager.AppSettings["tempFolderPath"] + "/"+guid.ToString();
                Directory.CreateDirectory(tempFolderPath);
                List<string> responseFilesPaths = new List<string>();
                //save files to temp folder
                responseFilesPaths.AddRange(InvoicePdf.SaveInvoicesToTempFolder(orders, underInvoiceText, tempFolderPath, mergeFiles));
                responseFilesPaths.AddRange(LabelsPdf.SaveLabelsToTempFolder(orders, tempFolderPath, mergeFiles));

                using (var compressedFileStream = new MemoryStream())
                {
                    //Create an archive and store the stream in memory.
                    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                    {

                        foreach (var file in responseFilesPaths)
                        {
                            //Create a zip entry for each attachment
                            var zipEntry = zipArchive.CreateEntry(file.Replace(tempFolderPath + "/", ""));
                            //Get the stream of the attachment
                            using (var originalFileStream = new FileStream(file, FileMode.Open))
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                originalFileStream.CopyTo(zipEntryStream);
                                originalFileStream.Dispose();
                            }
                        }
                    }
                    responseFilesPaths.Clear();
                    responseFilesPaths = null;
                    files = null;
                    Directory.Delete(tempFolderPath, true);
                    GC.Collect();
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.Default;
                    return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = "Filename.zip" };
                }

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.ContentType = "application/json";

                var jsonError = JsonConvert.SerializeObject(ex,new JsonSerializerSettings() { MaxDepth = 10,ReferenceLoopHandling=ReferenceLoopHandling.Ignore});
                return Content(jsonError.ToString());
            }
        }
    }
}