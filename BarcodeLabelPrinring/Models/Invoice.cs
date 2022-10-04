using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace BarcodeLabelPrinring.Models
{
	public class Invoice
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public static bool TryParseHttpFiles(HttpFileCollection httpFiles, out List<Invoice> invoices)
		{
			invoices = new List<Invoice>();

			try
			{
				if (httpFiles.Count < 1)
					return false;

				for (int i = 0; i < httpFiles.Count; i++)
				{
					HttpPostedFile file = httpFiles[i];
					switch (Path.GetExtension(file.FileName))
					{
						default:
							return false;
						case ".csv":
							invoices.AddRange(parseCsv(file));
							break;
							//case ".xmls" add new extension
					}
				}

				return true;
			}
			catch (Exception e)
			{
				//logger.log(e.message)
				return false;
			}
		}

		private static IEnumerable<Invoice> parseCsv(HttpPostedFile file)
		{
			using (var reader = new StreamReader(file.InputStream))
			{
				using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
					return csv.GetRecords<Invoice>();
				}
			}
		}
	}
}