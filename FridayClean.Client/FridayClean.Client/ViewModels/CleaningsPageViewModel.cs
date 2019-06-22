using FridayClean.Client.Api;
using FridayClean.Client.Api.Exceptions;
using FridayClean.Common;
using Plugin.Toast;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FridayClean.Client.ViewModels
{
	public class CleaningsPageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;
		private IFridayCleanApi _api;
		public DelegateCommand ItemTappedCommand { get; set; }

		public DelegateCommand OrderNewCleaningCommand { get; set; }
		public DelegateCommand OpenOrderedCleaningPageCommand { get; set; }

		private bool _isBusy = true;

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		private bool _isCleaner = false;

		public bool IsCleaner
		{
			get => _isCleaner;
			set =>
				SetProperty(ref _isCleaner, value);
		}


		private bool _isCustomer = false;

		public bool IsCustomer
		{
			get => _isCustomer;
			set =>
				SetProperty(ref _isCustomer, value);
		}

		private void OnOrderNewCleaningCommand()
		{
			IsBusy = true;
			_navigationService.NavigateAsync("OrderCleaningPage");
		}

		private void OnOpenOrderedCleaningPageCommand()
		{
			IsBusy = true;
			var parameters = new NavigationParameters();
			parameters.Add("order", SelectedItem);
			_navigationService.NavigateAsync("OrderedCleaningInfoPage", parameters);
		}

		private OrderedCleaning _selectedItem = null;
		public OrderedCleaning SelectedItem
		{
			get => _selectedItem;
			set
			{
				SetProperty(ref _selectedItem,value);
				OpenOrderedCleaningPageCommand.RaiseCanExecuteChanged();
			}
		}
		public ObservableCollection<OrderedCleaning> OrderedCleanings { get; }
		public CleaningsPageViewModel(IFridayCleanApi api, INavigationService navigationService) : base(
			navigationService)
		{
			OrderedCleanings = new ObservableCollection<OrderedCleaning>();
			OrderNewCleaningCommand = new DelegateCommand(OnOrderNewCleaningCommand);
			OpenOrderedCleaningPageCommand = new DelegateCommand(OnOpenOrderedCleaningPageCommand,()=> SelectedItem!=null);
			_api = api;
			_navigationService = navigationService;
		}

		public async override void OnNavigatedTo(INavigationParameters parameters)
		{
			IsBusy = true;

			GetOrderedCleaningsResponse response = null;
			GetProfileInfoResponse profile = null;
			try
			{
				response = await _api.GetOrderedCleaningsAsync(new GetOrderedCleaningsRequest(new GetOrderedCleaningsRequest()));
				profile = await _api.GetProfileInfoAsync(new GetProfileInfoRequest());
				IsCleaner = profile.UserRole == "Cleaner";
				IsCustomer = profile.UserRole == "Customer";
			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
				IsBusy = false;
				return;
			}
			OrderedCleanings.Clear();
			foreach (var orderedCleaning in response.OrderedCleanings)
			{

				OrderedCleanings.Add(orderedCleaning);
			}
			IsBusy = false;
		}
	}
}
