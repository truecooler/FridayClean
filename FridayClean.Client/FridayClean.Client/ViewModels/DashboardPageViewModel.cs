using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using Prism.Navigation;
using Prism.Commands;
using System.Collections.ObjectModel;
using FridayClean.Client.Api;
using FridayClean.Client.Modeles;
using Xamarin.Forms;
using FridayClean.Client.Views;

namespace FridayClean.Client.ViewModels
{
	class DashboardPageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;

		private IFridayCleanApi _api;
		public ObservableCollection<DashboardPageMenuItem> MenuItems { get; set; }

		private DashboardPageMenuItem selectedMenuItem;
		public DashboardPageMenuItem SelectedMenuItem
		{
			get => selectedMenuItem;
			set => SetProperty(ref selectedMenuItem, value);
		}

		public DelegateCommand NavigateCommand { get; private set; }
		public DashboardPageViewModel(IFridayCleanApi api, INavigationService navigationService) : base(navigationService)
		{
			_navigationService = navigationService;

			_api = api;

			MenuItems = new ObservableCollection<DashboardPageMenuItem>();

			MenuItems.Add(new DashboardPageMenuItem()
			{
				Icon = "profile_logo.png",
				PageName = nameof(ProfilePage),
				Title = "Профиль"
			});

			MenuItems.Add(new DashboardPageMenuItem()
			{
				Icon = "broom_logo.png",
				PageName = nameof(CleaningPage),
				Title = "Уборки"
			});

			MenuItems.Add(new DashboardPageMenuItem()
			{
				Icon = "rating_logo.png",
				PageName = nameof(RatingPage),
				Title = "Рейтинг"
			});

			NavigateCommand = new DelegateCommand(Navigate);
		}

		async void Navigate()
		{
			await _navigationService.NavigateAsync(nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
		}

	}
}
