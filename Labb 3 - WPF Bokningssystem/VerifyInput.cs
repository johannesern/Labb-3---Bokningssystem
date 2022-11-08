using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
	internal class VerifyInput
	{
		public VerifyInput()
		{
		}

		public bool CheckInput(string date, int table, string time, string name, int numberOfGuests)
		{
			return DateCheck(date) && TableCheck(table) && NameCheck(name) && NumberOfGuestsCheck(numberOfGuests) && TimeOfArrivalCheck(time);
		}

		private bool DateCheck(string date)
		{
			bool dateHasValue = !String.IsNullOrEmpty(date);
			DateTime tmpDateTime = DateTime.Parse(date);
			bool dateIsTodayOrFuture = tmpDateTime >= DateTime.Today;
			if (dateHasValue && dateIsTodayOrFuture)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool TableCheck(int table)
		{
			bool userHasPickedATable = !(table - 1 < 0);
			return userHasPickedATable;
		}

		private bool NameCheck(string name)
		{
			bool nameIsOfCorrectFormat = !(String.IsNullOrWhiteSpace(name) || name.Length < 2 || name == "Skriv in ert namn...");
			return nameIsOfCorrectFormat;

		}
		private bool NumberOfGuestsCheck(int numberOfGuests)
		{
			bool moreThanZeroGuests = !(numberOfGuests <= 0);
			return moreThanZeroGuests;
		}
		private bool TimeOfArrivalCheck(string time)
		{
			bool userHasPickedATimeOption = !String.IsNullOrWhiteSpace(time);
			return userHasPickedATimeOption;
		}
	}
}
