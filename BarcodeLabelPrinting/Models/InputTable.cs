using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Text;
using CsvHelper.Configuration;
using System.Configuration;

namespace BarcodeLabelPrinting.Models
{
	public class InputTableRow
	{
		public string AddresseesContact { get; set; }
		public string AddresseesAddress { get; set; }
		public string AddresseesTelephoneNumber { get; set; }
		public string Supplier { get; set; } = "LaRedoute";
		public string Carrier { get; set; }
		public string ParcelId { get; set; }
		public long ClientId { get; set; }
		public long OrderId { get; set; }
		public DateTime Date { get; set; }
		public long BillId { get; set; }
		public string BillType { get; set; }
		public string CurrencyType { get; set; }
		public double Nds { get; set; }

		public string ItemName { get; set; }
		public long ItemArticle { get; set; }
		public long ItemEan { get; set; }
		public string ItemSize { get; set; }
		public double ItemPrice { get; set; }
		public string ItemCount { get; set; }
		public double ServiceFee { get; set; }
		public double ItemDAD { get; set; }

		public static bool TryParseHttpFiles(HttpFileCollectionBase files, out List<Order> orders)
		{
			var rows = new List<InputTableRow>();
			orders = null;
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
							rows.AddRange(parseCsv(file));
							break;
							//case ".xmls" add new extension
					}
				}

				orders = RowsToOrders(rows);

				return true;
		}
		private static List<Order> RowsToOrders(List<InputTableRow> rows)
		{
			var orders = new List<Order>();
			foreach (var gr in rows.GroupBy(r => r.BillId))
			{
				var order = new Order();
				var row = gr.First();
				
				order.AddresseesContact = row.AddresseesContact;
				order.AddresseesAddress = row.AddresseesAddress;
				order.AddresseesTelephoneNumber = row.AddresseesTelephoneNumber;
				order.Carrier = row.Carrier;
				order.ParcelId = row.ParcelId;
				order.ClientId = row.ClientId;
				order.OrderId = row.OrderId;
				order.Date = row.Date;
				order.BillId = row.BillId;
				order.BillType = row.BillType;
				order.CurrencyType = row.CurrencyType;
				order.ServiceFee = row.ServiceFee;
				order.Nds = row.Nds;

				foreach (var item in gr)
				{
					var orderItem = new OrderItem();

					orderItem.Name = item.ItemName;
					orderItem.Article = item.ItemArticle;
					orderItem.Price = item.ItemPrice;
					orderItem.Ean = item.ItemEan;
					orderItem.Size = item.ItemSize;
					orderItem.Count = item.ItemCount;
					orderItem.ItemDAD = item.ItemDAD;

					order.Items.Add(orderItem);
				}
				orders.Add(order);
			}
			return orders;
		}
		private static ICollection<InputTableRow> parseCsv(HttpPostedFileBase file)
		{
			using (var reader = new StreamReader(file.InputStream,Encoding.GetEncoding("windows-1251")))
			{
				var config = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
				{
					Delimiter = ";",
					Encoding = Encoding.GetEncoding("windows-1251"),
				};
				using (var csv = new CsvReader(reader, config))
				{
					return csv.GetRecords<InputTableRow>().ToList();
				}
			}
		}

	}
}