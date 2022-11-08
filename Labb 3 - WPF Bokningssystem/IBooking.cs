using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
	internal interface IBooking
	{
		public string Name { get; set; }
		public string ChoosenDate { get; set; }
		public string TimeOfArrival { get; set; }
		public string BookingAsString { get; set; }
		public string ToString();
	}
}
