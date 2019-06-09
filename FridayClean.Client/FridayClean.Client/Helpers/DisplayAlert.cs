using System;
using System.Collections.Generic;
using System.Text;

namespace FridayClean.Client.Helpers
{
	partial class Utils
	{
		public static async void DisplayAlert(string title, string message, string accept = null, string cancel= null)
		{
			await App.Current.MainPage.DisplayAlert(title, message, accept, cancel);
		}
	}
}
