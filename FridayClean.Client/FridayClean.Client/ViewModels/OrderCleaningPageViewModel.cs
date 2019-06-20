using FridayClean.Client.Api;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FridayClean.Client.Api.Exceptions;
using Xamarin.Forms.Internals;
using FridayClean.Common;
using FridayClean.Common.Helpers;
using Plugin.Toast;
using Prism.Services;

namespace FridayClean.Client.ViewModels
{



	public class OrderCleaningPageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;
		private IFridayCleanApi _api;
		private IPageDialogService _dialogService;
		public DelegateCommand ItemSelectedCommand { get; set; }
		public DelegateCommand ShowCleaningServiceDescriptionCommand { get; set; }
		public DelegateCommand NextStepCommand { get; set; }

		private bool _isBusy = true;

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		private void OnItemSelectedCommand()
		{
			NextStepCommand.RaiseCanExecuteChanged();
		}

		private async void OnShowCleaningServiceDescriptionCommand()
		{
			if (SelectedItem == null)
				return;
			await Task.Yield();
			await _dialogService.DisplayAlertAsync(SelectedItem.Name, SelectedItem.Description, "OK");

		}

		private void OnNextStepCommand()
		{
			var order = new OrderNewCleaningRequest()
			{
				CleaningType = SelectedItem.CleaningType,
				ApartmentArea = SliderValue
			};

			NavigationParameters parameters = new NavigationParameters();
			parameters.Add("order",order);
			IsBusy = true;
			_navigationService.NavigateAsync("OrderCleaningPage2", parameters);
		}

		private int _sliderMin = 0;
		public int SliderMin
		{
			get => _sliderMin;
			set => SetProperty(ref _sliderMin, value);
		}

		private int _sliderMax = 100;
		public int SliderMax
		{
			get => _sliderMax;
			set => SetProperty(ref _sliderMax, value);
		}

		private CleaningService _selectedItem = null;
		public CleaningService SelectedItem
		{
			get => _selectedItem;
			set
			{
				if (value != null)
				{
					SliderMax = value.ApartmentAreaMax;
					SliderMin = value.ApartmentAreaMin;
					SliderValue = value.ApartmentAreaMin;

				}

				SetProperty(ref _selectedItem, value);
			}
		}

		private int _sliderValue = 0;
		public int SliderValue
		{
			get => _sliderValue;
			set
			{
				SetProperty(ref _sliderValue, value);
				RaisePropertyChanged(nameof(CalculatedPrice));
			}
		}

		private int _calculatedPrice = 0;

		public int CalculatedPrice
		{
			get
			{
				if (SelectedItem == null)
					return 0;

				return Utils.PriceCalculator.Calculate(SelectedItem.ApartmentAreaMin, SliderValue,
					SelectedItem.StartingPrice);
			}
		}

		public OrderCleaningPageViewModel(IFridayCleanApi api, INavigationService navigationService, IPageDialogService dialogService) : base(
			navigationService)
		{
			ItemSelectedCommand = new DelegateCommand(OnItemSelectedCommand);
			ShowCleaningServiceDescriptionCommand = new DelegateCommand(OnShowCleaningServiceDescriptionCommand);
			NextStepCommand = new DelegateCommand(OnNextStepCommand,()=>
			{
				return SelectedItem != null;
			});
			CleaningServices = new ObservableCollection<CleaningService>();
			_api = api;
			_navigationService = navigationService;
			_dialogService = dialogService;
		}

		public ObservableCollection<CleaningService> CleaningServices { get; }

		public async override void OnNavigatedTo(INavigationParameters parameters)
		{
			IsBusy = true;
			SelectedItem = null;
			NextStepCommand.RaiseCanExecuteChanged();
			GetCleaningServicesResponse response = null;
			try
			{
				response = await _api.GetCleaningServicesAsync(new GetCleaningServicesRequest(new GetCleaningServicesRequest()));
			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
				IsBusy = false;
				return;
			}
			CleaningServices.Clear();
			foreach (var cleaningService in response.CleaningServices)
			{
				
				CleaningServices.Add(cleaningService);
			}

			IsBusy = false;
		}
	}
}
