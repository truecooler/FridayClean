using System;
using FridayClean.Common;
using Xamarin.Essentials;
using Xamarin.Forms;
using Prism.Navigation;
using Prism.Services;

namespace FridayClean.Client.Views
{
	public partial class TodoListPage : ContentPage
	{

		public TodoListPage ()
		{
			InitializeComponent ();
		}

		private void Button_OnClicked(object sender, EventArgs e)
		{
			SecureStorage.Remove(Constants.AccessTokenSecureStorageKey);
			Environment.Exit(0);
		}
	}
}

