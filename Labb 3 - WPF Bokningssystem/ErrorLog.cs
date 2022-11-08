using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
	internal class ErrorLog
	{
		public string UpperDivider { get; set; }
		public string DateOfError { get; set; }
		public string ErrorMessage { get; set; }
		public string LowerDivider { get; set; }

		public ErrorLog(Exception ex)
		{
			UpperDivider = "ErrorStart";
			DateOfError = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			ErrorMessage = ex.ToString();
			LowerDivider = "ErrorEnd";
		}

		public static void SaveErrorToLog(ErrorLog newError)
		{
			//Spara felmeddelandet till en logfil			
			FileInfo fileInfo = new FileInfo(@"ErrorLog.txt");
			FileStream fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
			StreamWriter streamWrite = new StreamWriter(fileStream);
			var properties = newError.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (var property in properties)
			{
				var value = property.GetValue(newError);
				streamWrite.WriteLine(value);
			}
			streamWrite.Close();
			fileStream.Close();
		}
	}
}
