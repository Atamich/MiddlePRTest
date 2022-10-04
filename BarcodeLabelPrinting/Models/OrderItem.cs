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
		public long Article { get; set; }
		public long Ean { get; set; }
		public string Size { get; set; }
		public double Price { get; set; }
		public int Count { get; set; }
		public double TotalPrice { get; set; }

		public IEnumerator<string> GetEnumerator()
		{
			yield return Name;
			yield return Article.ToString();
			yield return Ean.ToString();
			yield return Size;
			yield return Price.ToString("0.##");
			yield return Count.ToString();
			yield return TotalPrice.ToString("0.##");
		}
	}
}