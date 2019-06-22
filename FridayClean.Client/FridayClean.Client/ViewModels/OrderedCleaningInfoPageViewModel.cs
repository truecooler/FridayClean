using FridayClean.Client.Api;
using FridayClean.Client.Api.Exceptions;
using FridayClean.Common;
using Plugin.Toast;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridayClean.Client.ViewModels
{
	public class OrderedCleaningInfoPageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;
		private IFridayCleanApi _api;

		public DelegateCommand CleanerConfirmCleaningCommand { get; set; }
		public DelegateCommand CancelCleaningCommand { get; set; }
		public DelegateCommand ReturnBackCommand { get; set; }
		public DelegateCommand CleanerArrivedAndStartedToWorkCommand { get; set; }
		public DelegateCommand CleanerCompletedTheWorkCommand { get; set; }

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

		private async Task<bool> ChangeOrderedCleaningState(int orderId, OrderedCleaningState NewState)
		{
			bool isSuccess = true;
			try
			{
				var response = await _api.ChangeOrderedCleaningStateAsync(new ChangeOrderedCleaningStateRequest()
					{OrderId = orderId, RequiredState = NewState });

				if (response.ResponseStatus == ChangeOrderedCleaningStateStatus.OrderStatusChangeError)
				{
					CrossToastPopUp.Current.ShowToastError(
						$"Невозможно обновить статус заказа: {response.ErrorMessage}");
					isSuccess = false;
				}
				if (response.ResponseStatus == ChangeOrderedCleaningStateStatus.OrderStatusChangedSuccessfully)
				{
					CrossToastPopUp.Current.ShowToastMessage(
						$"Статус заказа успешно обновлен");
				}

			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
				isSuccess = false;
			}

			return isSuccess;
		}

		private async void OnCleanerConfirmCleaningCommand()
		{
			IsBusy = true;
			if (await ChangeOrderedCleaningState(_orderedCleaning.Id, OrderedCleaningState.WaitingForCleanerArrival))
			{
				await _navigationService.GoBackAsync();
				return;
			}
			IsBusy = false;
		}

		private async void OnCancelCleaningCommand()
		{
			IsBusy = true;
			if (await ChangeOrderedCleaningState(_orderedCleaning.Id, OrderedCleaningState.Canceled))
			{
				await _navigationService.GoBackAsync();
				return;
			}
			IsBusy = false;
		}

		private async void OnCleanerArrivedAndStartedToWorkCommand()
		{
			IsBusy = true;
			if (await ChangeOrderedCleaningState(_orderedCleaning.Id, OrderedCleaningState.CleanerWorkInProgress))
			{
				await _navigationService.GoBackAsync();
				return;
			}
			IsBusy = false;
		}

		private async void OnCleanerCompletedTheWorkCommand()
		{
			IsBusy = true;
			if (await ChangeOrderedCleaningState(_orderedCleaning.Id, OrderedCleaningState.Completed))
			{
				await _navigationService.GoBackAsync();
				return;
			}
			IsBusy = false;
		}

		private async void OnReturnBackCommand()
		{
			await _navigationService.GoBackAsync();
		}

		private OrderedCleaning _orderedCleaning = null;

		private int _id = 0;
		public int Id
		{
			get => _id;
			set => SetProperty(ref _id, value);
		}

		private string _cleanerName = "";
		public string CleanerName
		{
			get => _cleanerName;
			set => SetProperty(ref _cleanerName, value);
		}

		private string _cleanerPhone = "";
		public string CleanerPhone
		{
			get => _cleanerPhone;
			set => SetProperty(ref _cleanerPhone, value);
		}

		private string _customerName = "";
		public string CustomerName
		{
			get => _customerName;
			set => SetProperty(ref _customerName, value);
		}

		private string _customerPhone = "";
		public string CustomerPhone
		{
			get => _customerPhone;
			set => SetProperty(ref _customerPhone, value);
		}

		private CleaningType _cleaningType = 0;
		public CleaningType CleaningType
		{
			get => _cleaningType;
			set => SetProperty(ref _cleaningType, value);
		}

		private int _apartmentArea = 0;
		public int ApartmentArea
		{
			get => _apartmentArea;
			set => SetProperty(ref _apartmentArea, value);
		}

		private int _price = 0;
		public int Price
		{
			get => _price;
			set => SetProperty(ref _price, value);
		}

		private OrderedCleaningState _state = 0;
		public OrderedCleaningState State
		{
			get => _state;
			set
			{
				SetProperty(ref _state, value);
				CleanerConfirmCleaningCommand.RaiseCanExecuteChanged();
				CleanerArrivedAndStartedToWorkCommand.RaiseCanExecuteChanged();
				CleanerCompletedTheWorkCommand.RaiseCanExecuteChanged();
			}
		}


		private string _address = "";
		public string Address
		{
			get => _address;
			set => SetProperty(ref _address, value);
		}

		public OrderedCleaningInfoPageViewModel(IFridayCleanApi api, INavigationService navigationService) : base(
			navigationService)
		{
			CleanerConfirmCleaningCommand = new DelegateCommand(OnCleanerConfirmCleaningCommand,() => State == OrderedCleaningState.WaitingForCleanerConfirmation);
			CancelCleaningCommand = new DelegateCommand(OnCancelCleaningCommand);
			ReturnBackCommand = new DelegateCommand(OnReturnBackCommand);
			CleanerArrivedAndStartedToWorkCommand = new DelegateCommand(OnCleanerArrivedAndStartedToWorkCommand,() => State == OrderedCleaningState.WaitingForCleanerArrival);
			CleanerCompletedTheWorkCommand = new DelegateCommand(OnCleanerCompletedTheWorkCommand,() => State == OrderedCleaningState.CleanerWorkInProgress);

			_api = api;
			_navigationService = navigationService;
		}

		public async override void OnNavigatedTo(INavigationParameters parameters)
		{
			IsBusy = true;
			parameters.TryGetValue("order", out _orderedCleaning);
			GetProfileInfoResponse userInfo = null;
			try
			{
				userInfo = await _api.GetProfileInfoAsync(new GetProfileInfoRequest());
				//change to enum or constant
				IsCleaner = userInfo.UserRole == "Cleaner";
				IsCustomer = userInfo.UserRole == "Customer";
			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
			}

			Id = _orderedCleaning.Id;
			CleanerName = _orderedCleaning.CleanerName;
			CleanerPhone = _orderedCleaning.CleanerPhone;
			CustomerName = _orderedCleaning.CustomerName;
			CustomerPhone = _orderedCleaning.CustomerPhone;
			CleaningType = _orderedCleaning.CleaningType;
			ApartmentArea = _orderedCleaning.ApartmentArea;
			Price = _orderedCleaning.Price;
			State = _orderedCleaning.State;
			Address = _orderedCleaning.Address;

			IsBusy = false;

		}
	}
}
