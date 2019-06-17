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

		private bool _isBusy = false;

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		private void OnItemTappedCommand()
		{

		}

		private void OnOrderNewCleaningCommand()
		{
			_navigationService.NavigateAsync("OrgerCleaningPage");
		}
		public OrderedCleaning SelectedItem { get; set; }
		public ObservableCollection<OrderedCleaning> OrderedCleanings { get; }
		public CleaningsPageViewModel(IFridayCleanApi api, INavigationService navigationService) : base(
			navigationService)
		{
			OrderedCleanings = new ObservableCollection<OrderedCleaning>();
			ItemTappedCommand = new DelegateCommand(OnItemTappedCommand);
			OrderNewCleaningCommand = new DelegateCommand(OnOrderNewCleaningCommand);
			_api = api;
			_navigationService = navigationService;
		}

		public async override void OnNavigatedTo(INavigationParameters parameters)
		{
			IsBusy = true;
			GetOrderedCleaningsResponse response = null;
			try
			{
				response = await _api.GetOrderedCleaningsAsync(new GetOrderedCleaningsRequest(new GetOrderedCleaningsRequest()));
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
