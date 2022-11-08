using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
	public class Filehandler
	{
		private List<Booking> bookingsList = new List<Booking>();
		public List<Booking> BookingsList
		{
			get { return bookingsList; }
			set { bookingsList = value; }
		}
		public Filehandler()
		{
		}

		public void ConfirmBooking(string dateFromCalender, int comboTable, string comboTimeOfArrival, string txtBoxName, int comboNumberOfGuests)
		{
			VerifyInput newVerifier = new VerifyInput();
			bool InputIsOk = newVerifier.CheckInput(dateFromCalender, comboTable, comboTimeOfArrival, txtBoxName, comboNumberOfGuests);
			if (InputIsOk)
			{
				if (CheckingForFreeSlotsAtSpecificTimeOption(comboTimeOfArrival, comboTable))
				{
					try
					{
						Booking newBooking = new Booking(txtBoxName, comboTable, comboNumberOfGuests, dateFromCalender, comboTimeOfArrival);
						AddToListSortListByTime(newBooking);
						SaveListToFile(dateFromCalender);
					}
					catch (Exception ex)
					{
						ErrorLog.SaveErrorToLog(new ErrorLog(ex));
						MessageBox.Show("Felaktig inmatning!", "Inmatningsfel!", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show($"Alla fält måste fyllas i och datumet får tidigast vara {DateTime.Today.ToString("yyyy-MM-dd")}", "Inmatningsfel!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void AddToListSortListByTime(Booking newBooking)
		{
			BookingsList.Add(newBooking);
			BookingsList = BookingsList.OrderBy(booking => booking.TimeOfArrival).ToList();
		}

		private bool CheckingForFreeSlotsAtSpecificTimeOption(string comboTimeOfArrival, int comboTable)
		{
			bool isNotADoubleBooking = CheckForDoubleBooking(comboTimeOfArrival, comboTable);
			if (isNotADoubleBooking)
			{
				//Skapa lista efter användarens valda datum
				List<Booking> tmpBookingsList = new List<Booking>();
				BookingsList.Where(x => x.TimeOfArrival == comboTimeOfArrival).ToList().ForEach(x => tmpBookingsList.Add(x));

				if (tmpBookingsList.Count() < 5)
				{
					//There is room for some more!
					return true;
				}
				else
				{
					//No more room sorry...
					MessageBox.Show("Alla tider uppbokade", "Fullbokat!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return false;
				}
			}
			else
			{
				MessageBox.Show("Bordet är redan bokat denna tidpunkten...", "Fullbokat!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return false;
			}
		}

		private bool CheckForDoubleBooking(string comboTimeOfArrival, int comboTable)
		{
			if (BookingsList.Count > 0)
			{
				var bookingToCompare = from booking in BookingsList
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

		public async void SaveListToFile(string date)
		{
			string filename = date + ".json";
			//Spara filen till den bokade dagens datum
			if (File.Exists(filename))
			{
				File.Delete(filename);
				await SerializeListToFile(filename);
			}
			else
			{
				await SerializeListToFile(filename);
			}
		}

		private async Task SerializeListToFile(string filename)
		{
			try
			{
				using (FileStream fileStream = File.Create(filename))
				{
					await JsonSerializer.SerializeAsync(fileStream, BookingsList);
					fileStream.Dispose();
				}
			}
			catch (Exception ex)
			{
				SaveException(ex);
				MessageBox.Show("Fel vid sparande av fil, kontakta systemadmin.", "Fel vid utläsning!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public void DeserializeFileToList(string date)
		{
			BookingsList.Clear();
			string filename = date + ".json";
			if (File.Exists(filename))
			{
				try
				{
					using (FileStream readStream = File.OpenRead(filename))
					{
						BookingsList = JsonSerializer.Deserialize<List<Booking>>(readStream);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Fel vid inläsning av fil, kontakta systemadmin.", "Inläsningsfel!", MessageBoxButton.OK, MessageBoxImage.Error);
					SaveException(ex);
				}
			}
		}

		public void RemoveBooking(int listboxSelectedIndex, string dateFromCalender)
		{
			try
			{
				BookingsList.RemoveAt(listboxSelectedIndex);
				SaveListToFile(dateFromCalender);
			}
			catch (Exception ex)
			{
				SaveException(ex);
				MessageBox.Show("Bokningen finns inte.", "Varning!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public void DeleteFile(string dateToDelete)
		{
			string filename = dateToDelete + ".json";
			try
			{
				File.Delete(filename);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Fel vid borttagning av fil, kontakta systemadmin.\nProgrammet går att använda som vanligt.", "Fel!", MessageBoxButton.OK, MessageBoxImage.Error);
				SaveException(ex);
			}
		}

		private void SaveException(Exception ex)
		{
			ErrorLog.SaveErrorToLog(new ErrorLog(ex));
		}
	}
}
