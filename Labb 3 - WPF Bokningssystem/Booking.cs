using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
	public class Booking : IBooking
	{
		public string Name { get; set; }
		public int Table { get; set; }
		public int NumberOfGuests { get; set; }
		public string ChoosenDate { get; set; }
		public string TimeOfArrival { get; set; }
		public string DateOfBooking { get; set; }
		public string BookingAsString { get; set; }

		public Booking(string name, int table, int numberOfGuests, string choosenDate, string timeOfArrival)
		{
			Name = name;
			Table = table;
			NumberOfGuests = numberOfGuests;
			ChoosenDate = choosenDate;
			TimeOfArrival = timeOfArrival;
			DateOfBooking = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			BookingAsString = $"{TimeOfArrival}\t{NumberOfGuests}\t{Table}\t{DateOfBooking}\t{Name}";
		}

		public override string ToString()
		{
			return $"{TimeOfArrival}\t{NumberOfGuests}\t{Table}\t{DateOfBooking}\t{Name}";
		}
	}
}
