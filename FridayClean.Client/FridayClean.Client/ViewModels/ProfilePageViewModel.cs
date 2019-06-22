using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FridayClean.Client.Api;
using FridayClean.Client.Api.Exceptions;
using FridayClean.Common;
using Prism.Navigation;
using Plugin.Toast;
using Xamarin.Essentials;
using Plugin.Toast.Abstractions;

namespace FridayClean.Client.ViewModels
{
	public class ProfilePageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;

		private IFridayCleanApi _api;

		private GetProfileInfoResponse _profileInfo;

		private bool _isBusy = true;
		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		public string ProfileName
		{
			get { return string.IsNullOrEmpty(_profileInfo?.Name) ? "Незнакомец" : _profileInfo.Name; }
		}

		public string ProfileAddress
		{
			get { return string.IsNullOrEmpty(_profileInfo?.Address) ? "Адрес не установлен" : _profileInfo.Address; }
		}

		public string ProfileAvatarLink
		{
			get { return string.IsNullOrEmpty(_profileInfo?.AvatarLink) ? "avatar.jpg" : _profileInfo.AvatarLink; }
		}

		public int ProfileMoney
		{
			get { return _profileInfo?.Money == null ? 0 : _profileInfo.Money; }
		}

		public DelegateCommand EditProfileCommand { get; }

		public DelegateCommand LogoutCommand { get; }


		private async void OnEditProfileCommand()
		{
			var parameters = new NavigationParameters();
			parameters.Add("_profileInfo", _profileInfo);
			IsBusy = true;
			await _navigationService.NavigateAsync("EditProfilePage", parameters);
		}

		private async void OnLogoutCommand()
		{

			try
			{
				 await _api.UserLogoutAsync(new UserLogoutRequest(new UserLogoutRequest()));
			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
			}

			try
			{
				SecureStorage.Remove(Constants.AccessTokenSecureStorageKey);
			}
			catch (Exception ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.SecureStorageNotSupportedMessage} ({ex.Message})", ToastLength.Long);
			}

			await _navigationService.NavigateAsync("/LoginPage");

		}
		public ProfilePageViewModel(IFridayCleanApi api, INavigationService navigationService) : base(navigationService)
		{
			_api = api;
			_navigationService = navigationService;
			EditProfileCommand = new DelegateCommand(OnEditProfileCommand);
			LogoutCommand = new DelegateCommand(OnLogoutCommand);
		}

		public override async void OnNavigatedTo(INavigationParameters parameters)
		{
			IsBusy = true;

			try
			{
				_profileInfo = await _api.GetProfileInfoAsync(new GetProfileInfoRequest(new GetProfileInfoRequest()));
				RaisePropertyChanged(nameof(ProfileName));
				RaisePropertyChanged(nameof(ProfileAddress));
				RaisePropertyChanged(nameof(ProfileAvatarLink));
				RaisePropertyChanged(nameof(ProfileMoney));
			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
			}

			IsBusy = false;
		}
	}
}
