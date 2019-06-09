using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using Prism.Navigation;
using Prism.Commands;
using System.Collections.ObjectModel;
using FridayClean.Client.Modeles;
using Xamarin.Forms;
using FridayClean.Client.Views;

namespace FridayClean.Client.ViewModels
{
	class DashboardPageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;
		public ObservableCollection<DashboardPageMenuItem> MenuItems { get; set; }

		private DashboardPageMenuItem selectedMenuItem;
		public DashboardPageMenuItem SelectedMenuItem
		{
			get => selectedMenuItem;
			set => SetProperty(ref selectedMenuItem, value);
		}

		public DelegateCommand NavigateCommand { get; private set; }
		public DashboardPageViewModel(INavigationService navigationService) : base(navigationService)
		{
			_navigationService = navigationService;

			MenuItems = new ObservableCollection<DashboardPageMenuItem>();

			MenuItems.Add(new DashboardPageMenuItem()
			{
				Icon = "ic_viewa",
				PageName = nameof(ContactsPage),
				Title = "ContactsPage"
			});

			MenuItems.Add(new DashboardPageMenuItem()
			{
				Icon = "ic_viewb",
				PageName = nameof(ReminderPage),
				Title = "ReminderPage"
			});

			MenuItems.Add(new DashboardPageMenuItem()
			{
				Icon = "ic_viewb",
				PageName = nameof(TodoListPage),
				Title = "TodoListPage"
			});

			NavigateCommand = new DelegateCommand(Navigate);
		}

		async void Navigate()
		{
			await _navigationService.NavigateAsync(nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
		}

	}
}
