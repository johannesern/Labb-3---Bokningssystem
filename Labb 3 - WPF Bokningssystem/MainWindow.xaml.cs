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
	}
}
