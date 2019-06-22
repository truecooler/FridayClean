using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace FridayClean.Client.ViewModels
{
	public class CleaningServiceDescriptionPageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;
		public ObservableCollection<ContentPage> Pages { get; private set; }

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			List<ContentPage> ContentPages = new List<ContentPage>();
			ContentPages.Add(new ContentPage());
			Pages = new ObservableCollection<ContentPage>(ContentPages);
		}
		public CleaningServiceDescriptionPageViewModel(INavigationService navigationService) : base(
			navigationService)
		{

		}
	}
}
