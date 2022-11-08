using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace WpfApp1
{
	public partial class MainWindow : Window
	{
		Filehandler filehandler = new Filehandler();
		public string? DateFromCalender { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			UpdateOnStart();
			ListViewUpdate();
		}
		private void btnConfirmBooking_Click(object sender, RoutedEventArgs e)
		{
			filehandler.ConfirmBooking(DateFromCalender, comboTable.SelectedIndex + 1, comboTimeOfArrival.Text, txtBoxName.Text, comboNumberOfGuests.SelectedIndex + 1);
			ResetFields();
			if (DateFromCalender != String.Empty)
			{
				ListViewUpdate();
			}
		}

		private void ResetFields()
		{
			comboTimeOfArrival.SelectedIndex = -1;
			comboNumberOfGuests.SelectedIndex = -1;
			comboTable.SelectedIndex = -1;
			txtBoxName.Text = "Skriv in ett namn...";
		}

		private void txtBoxName_GotFocus(object sender, RoutedEventArgs e)
		{
			txtBoxName.Clear();
		}

		private void calender_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{
			DateFromCalender = GetCorrectDateFormat();
			ListViewUpdate();
		}

		private void btnCancelBooking_Click(object sender, RoutedEventArgs e)
		{
			bool userMadeAChoiceAndTheListContainsSomething = listView.SelectedIndex != -1 && filehandler.BookingsList.Count > 0;
			if (userMadeAChoiceAndTheListContainsSomething)
			{
				var result = MessageBox.Show("Är du säker på att du vill avboka?", "Avbokning", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					filehandler.RemoveBooking(listView.SelectedIndex, DateFromCalender);
					ListViewUpdate();
					if (filehandler.BookingsList.Count == 0)
					{
						filehandler.DeleteFile(DateFromCalender);
					}
				}
			}
		}

		private string GetCorrectDateFormat()
		{
			try
			{
				DateOnly dateIsNowInCorrectFormat = DateOnly.FromDateTime((DateTime)calender.SelectedDate);
				return dateIsNowInCorrectFormat.ToString();
			}
			catch (Exception ex)
			{
				ErrorLog.SaveErrorToLog(new ErrorLog(ex));
				MessageBox.Show("Inget datum valt", "Datumfel!", MessageBoxButton.OK, MessageBoxImage.Error);
				return String.Empty;
			}
		}

		private void UpdateOnStart()
		{
			string today = DateTime.Today.ToString("yyyy-MM-dd");
			DateFromCalender = today;
		}

		private void ListViewUpdate()
		{
			filehandler.DeserializeFileToList(DateFromCalender);
			listView.ItemsSource = null;
			listView.ItemsSource = filehandler.BookingsList;
		}
		private void listView_MouseEnter(object sender, MouseEventArgs e)
		{
			listView.Focus();
		}
		private void calender_MouseEnter(object sender, MouseEventArgs e)
		{
			calender.Focus();
		}


		//==================================================================================================================
		//Koden nedanför är bara till för att lägga till en stor mängd objekt snabbt och är ej skriven för att vara lättläst
		private void btnFillCalender_Click(object sender, RoutedEventArgs e)
		{
			AutoCalFiller();
		}
		private void AutoCalFiller()
		{
			Random random = new Random();

			string today = DateTime.Now.ToString("dd");
			int k = random.Next(int.Parse(today), 31);
			for (int i = 0; i < 30; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					FillCalenderWithRandomBookings();
				}
			}
		}
		private void FillCalenderWithRandomBookings()
		{
			string[] names = { "Kalle", "Pelle", "Lisa", "Kajsa", "Thomas", "Anna", "Sven", "Björn", "Malin", "Mary", "Matilda" };
			string[] times = { "12:00", "15:00", "16:00", "18:00", "20:00", "22:00" };
			Random random = new Random();
			string name = names[random.Next(names.Length)];
			string time = times[random.Next(times.Length)];
			int table = random.Next(1, 7);
			int guests = random.Next(1, 10);

			string dt = DateTime.Now.ToString("yyyy-MM-dd");
			string today = DateTime.Now.ToString("dd");
			int todayINT = int.Parse(today);
			int j = random.Next(todayINT, 31);
			if (j < 10)
			{
				dt = dt.Substring(0, 9) + j.ToString();
			}
			else
			{
				dt = dt.Substring(0, 8) + j.ToString();
			}
			bool doNotSkip = filter(time, table);
			if (doNotSkip)
			{
				if (filehandler.BookingsList.Count < 11)
				{
					AddToListSortListByTime(new Booking(name, table, guests, dt, time));
					filehandler.SaveListToFile(dt);
				}
				filehandler.DeserializeFileToList(dt);
				ListViewUpdate();
			}
		}
		private void AddToListSortListByTime(Booking newBooking)
		{
			filehandler.BookingsList.Add(newBooking);
			filehandler.BookingsList = filehandler.BookingsList.OrderBy(booking => booking.TimeOfArrival).ToList();
		}
		private bool filter(string comboTimeOfArrival, int comboTable)
		{
			bool isNotADoubleBooking = filter2(comboTimeOfArrival, comboTable);
			if (isNotADoubleBooking)
			{
				//Skapa lista efter användarens valda datum
				List<Booking> tmpBookingsList = new List<Booking>();
				filehandler.BookingsList.Where(x => x.TimeOfArrival == comboTimeOfArrival).ToList().ForEach(x => tmpBookingsList.Add(x));

				if (tmpBookingsList.Count() < 5)
				{
					//There is room for some more!
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		private bool filter2(string comboTimeOfArrival, int comboTable)
		{
			if (filehandler.BookingsList.Count > 0)
			{
				var bookingToCompare = from booking in filehandler.BookingsList
									   where booking.TimeOfArrival == comboTimeOfArrival && booking.Table == comboTable
									   select booking;

				List<Booking> dateAndTimeAlreadyBooked = bookingToCompare.ToList();
				bool isTheTimeAndTableFree = true;
				if (dateAndTimeAlreadyBooked.Count > 0)
				{
					isTheTimeAndTableFree = false;
				}
				return isTheTimeAndTableFree;
			}
			else
			{
				return true;
			}
		}
				
	}
}
