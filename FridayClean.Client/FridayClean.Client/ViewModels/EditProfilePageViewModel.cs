using FridayClean.Client.Api;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using FridayClean.Client.Api.Exceptions;
using Plugin.Toast;
using FridayClean.Common;

namespace FridayClean.Client.ViewModels
{
	public class EditProfilePageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;
		private IFridayCleanApi _api;
		private GetProfileInfoResponse _profileInfo;


		public DelegateCommand SaveCommand { get; }
		//public string ProfileName
		//{
		//	get => _profileInfo?.Name??"";
		//	set
		//	{
		//		if (_profileInfo?.Name == null)
		//			return;
		//		_profileInfo.Name = value;
		//		SaveCommand.RaiseCanExecuteChanged();

		//	}
		//}

		//public string ProfileAddress
		//{
		//	get => _profileInfo?.Address??"";
		//	set
		//	{
		//		if (_profileInfo?.Address == null)
		//			return;
		//		_profileInfo.Address = value;
		//		SaveCommand.RaiseCanExecuteChanged();

		//	}
		//}

		private string _profileName = "";
		public string ProfileName
		{
			get => _profileName;
			set
			{
				SaveCommand.RaiseCanExecuteChanged();
				SetProperty(ref _profileName, value);
			}
		}

		private string _profileAddress = "";
		public string ProfileAddress
		{
			get => _profileAddress;
			set
			{
				SaveCommand.RaiseCanExecuteChanged();
				SetProperty(ref _profileAddress, value);
			}
		}

		private bool _isBusy = false;
		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		public EditProfilePageViewModel(IFridayCleanApi api, INavigationService navigationService) : base(
			navigationService)
		{
			_api = api;
			_navigationService = navigationService;
			SaveCommand = new DelegateCommand(OnSaveCommand, () =>
			{
				return !string.IsNullOrEmpty(ProfileName)
				       && !string.IsNullOrEmpty(ProfileAddress);
			});
		}

		private async void OnSaveCommand()
		{
			IsBusy = true;
			try
			{
				var response = await _api.SetProfileInfoAsync(new SetProfileInfoRequest()
					{Address = ProfileAddress, Name = ProfileName});


				CrossToastPopUp.Current.ShowToastMessage(response.ResponseStatus == SetProfileInfoStatus.SetSuccessfully
					? "Информация профиля успешно обновлена"
					: "Не удалось обновить информацию о профиле");
			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
			}

			IsBusy = false;
			await _navigationService.GoBackAsync();
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			parameters.TryGetValue("_profileInfo", out _profileInfo);
			ProfileName = _profileInfo.Name;
			ProfileAddress = _profileInfo.Address;
			//RaisePropertyChanged(ProfileName);
			//RaisePropertyChanged(ProfileAddress);
		}
	}
}
