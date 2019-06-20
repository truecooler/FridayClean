using FridayClean.Client.Api;
using FridayClean.Client.Api.Exceptions;
using FridayClean.Common;
using Plugin.Toast;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FridayClean.Client.ViewModels
{
	public class OrderCleaningPage2ViewModel : ViewModelBase
	{
		private INavigationService _navigationService;
		private IFridayCleanApi _api;
		private IPageDialogService _dialogService;

		public DelegateCommand NextStepCommand { get; set; }

		public DelegateCommand ItemSelectedCommand { get; set; }

		private bool _isBusy = true;

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		public Cleaner SelectedItem { get; set; }

		private async void OnNextStepCommand()
		{
			IsBusy = true;
			_orderNewCleaningRequest.CleanerPhone = SelectedItem.Phone;

			try
			{
				 await _api.OrderNewCleaningAsync(_orderNewCleaningRequest);
			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
				IsBusy = false;
				return;
			}

			CrossToastPopUp.Current.ShowToastSuccess("Заявка на уборку успешно создана");
			await _navigationService.NavigateAsync("/DashboardPage/NavigationPage/CleaningsPage");
		}

		private void OnItemSelectedCommand()
		{
			NextStepCommand.RaiseCanExecuteChanged();
		}

		public OrderCleaningPage2ViewModel(IFridayCleanApi api, INavigationService navigationService, IPageDialogService dialogService) : base(
			navigationService)
		{
			NextStepCommand = new DelegateCommand(OnNextStepCommand);
			ItemSelectedCommand = new DelegateCommand(OnItemSelectedCommand);
			_api = api;
			_navigationService = navigationService;
			_dialogService = dialogService;
			Cleaners = new ObservableCollection<Cleaner>();
		}

		public ObservableCollection<Cleaner> Cleaners { get; }

		private OrderNewCleaningRequest _orderNewCleaningRequest;
		public async override void OnNavigatedTo(INavigationParameters parameters)
		{
			IsBusy = true;
			SelectedItem = null;
			NextStepCommand.RaiseCanExecuteChanged();
			parameters.TryGetValue("order", out _orderNewCleaningRequest);
			GetCleanersResponse response = null;
			try
			{
				response = await _api.GetCleanersAsync(new GetCleanersRequest(new GetCleanersRequest()));
			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
				IsBusy = false;
				return;
			}
			Cleaners.Clear();
			foreach (var cleaningService in response.Cleaners)
			{

				Cleaners.Add(cleaningService);
			}

			IsBusy = false;
		}
	}
}
