using CsvHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace BarcodeLabelPrinting.Models
{
	public class OrderItem
	{
		public string Name { get; set; }
		public string Article { get; set; }
		public string Ean { get; set; }
		public string Size { get; set; }
		public double Price { get; set; }
		public int Count { get; set; }
		public double TotalPrice => Price * Count;

		public static bool TryParseHttpFiles(HttpFileCollectionBase files, out List<OrderItem> orderItems)
		{
			orderItems = new List<OrderItem>();

			try
			{
				if (files.Count < 1)
					return false;

				for (int i = 0; i < files.Count; i++)
				{
					HttpPostedFileBase file = files[i];
					switch (Path.GetExtension(file.FileName))
					{
						default:
							return false;
						case ".csv":
							orderItems.AddRange(parseCsv(file));
							break;
							//case ".xmls" add new extension
					}
				}

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);//log
				return false;
			}
		}

		private static ICollection<OrderItem> parseCsv(HttpPostedFileBase file)
		{
			using (var reader = new StreamReader(file.InputStream))
			{
				using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
					return csv.GetRecords<OrderItem>().ToList();
				}
			}
		}
		public IEnumerator<string> GetEnumerator()
		{
			yield return Name;
			yield return Article;
			yield return Ean;
			yield return Size;
			yield return Price.ToString();
			yield return Count.ToString();
			yield return TotalPrice.ToString();
		}
		public override string ToString()
		{
			return $"{Name} {Article} {Ean} {Size} {Price} {Count} {TotalPrice}";
		}
	}
}