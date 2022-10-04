using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarcodeLabelPrinting.Models
{
	public class Order
	{
		public List<OrderItem> Items { get; set; } = new List<OrderItem>();
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
		public double TotalPrice => Items.Sum(i=>i.ItemDAD);
		public double Nds { get; set; }
		public double ServiceFee { get; set; }
	}
}