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
using FridayClean.Client.Api.Exceptions;
using Xamarin.Forms.Internals;
using FridayClean.Common;
using Plugin.Toast;

namespace FridayClean.Client.ViewModels
{



	public class OrgerCleaningPageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;
		private IFridayCleanApi _api;
		public DelegateCommand ItemTappedCommand { get; set; }

		private bool _isBusy = false;

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		private void OnItemTappedCommand()
		{

		}

		public CleaningService SelectedItem { get; set; }

		public OrgerCleaningPageViewModel(IFridayCleanApi api, INavigationService navigationService) : base(
			navigationService)
		{
			ItemTappedCommand = new DelegateCommand(OnItemTappedCommand);
			CleaningServices = new ObservableCollection<CleaningService>();
			_api = api;
			_navigationService = navigationService;
		}

		public ObservableCollection<CleaningService> CleaningServices { get; }

		public async override void OnNavigatedTo(INavigationParameters parameters)
		{
			IsBusy = true;
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
