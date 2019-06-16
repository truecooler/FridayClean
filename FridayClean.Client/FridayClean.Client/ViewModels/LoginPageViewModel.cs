using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using FridayClean.Client.Views;
using FridayClean.Client.Api;
using FridayClean.Common.Helpers;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using FridayClean.Client.Api.Exceptions;

using Xamarin.Essentials;
using FridayClean.Common;
using Prism.Commands;

namespace FridayClean.Client.ViewModels
{
	public class LoginPageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;

		private IPageDialogService _dialogService;

		private IFridayCleanApi _api;

		private bool _isWaitingForCode = false;
		public bool IsWaitingForCode
		{
			get =>_isWaitingForCode; 

			set => SetProperty(ref _isWaitingForCode, value);
		}

		private bool _isBusy = false;
		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		private string _phone;
		public string Phone
		{
			get => _phone; 
			set => SetProperty(ref _phone, value);
		}
		private string _code;
		public string Code
		{
			get => _code;
			set => SetProperty(ref _code, value);
		}
		public DelegateCommand LoginCommand { protected set; get; }
		public LoginPageViewModel(INavigationService navigationService,
			IPageDialogService dialogService, IFridayCleanApi api) : base(navigationService)
		{
			

			_dialogService = dialogService;
			_api = api;
			_navigationService = navigationService;
			LoginCommand = new DelegateCommand(OnLoginAsync,()=>false);
		}

		public override async void OnNavigatedTo(INavigationParameters parameters)
		{
			await Task.Yield();
			
			if (Utils.AppCrashHelper.IsCrashLogExists)
			{
				await _dialogService.DisplayAlertAsync("Last crash info", Utils.AppCrashHelper.ReadCrashLogFile(), "OK");
			}
			IsBusy = true;

			string accessToken = "";
			try
			{
				accessToken = await SecureStorage.GetAsync(Constants.AccessTokenSecureStorageKey);
			}
			catch (Exception ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.SecureStorageNotSupportedMessage} ({ex})", ToastLength.Long);
			}

			
			if (string.IsNullOrEmpty(accessToken))
			{
				IsBusy = false;
				return;
			}

			try
			{
				if ((await _api.AuthValidateTokenAsync(new AuthValidateTokenRequest() {AccessToken = accessToken}))
				    .ResponseStatus == AuthValidateTokenStatus.InvalidToken)
				{
					IsBusy = false;
					CrossToastPopUp.Current.ShowToastError("Токен авторизации истек, авторизуйтесь заного.");
					return;
				}
			}
			catch (GrpcExceptionBase ex)
			{
				IsBusy = false;
				CrossToastPopUp.Current.ShowToastError($"Невозможно подтвердить вашу сессию, сервер временно недоступен, попробуйте позже. {ex.Message}");
				return;
			}

			IsBusy = false;
			_api.Settings.AccessToken = accessToken;
			await _navigationService.NavigateAsync($"/DashboardPage/NavigationPage/ProfilePage");

		}

		private async void OnLoginAsync()
		{
			IsBusy = true;
			try
			{
				
				if (!IsWaitingForCode)
				{
					var sendCodeResponse = await _api.AuthSendCodeAsync(new AuthSendCodeRequest()
						{Phone = Utils.PhoneTrimer(Phone)});
					IsBusy = false;
					if (sendCodeResponse.ResponseStatus == AuthSendCodeStatus.InvalidPhone)
					{
						CrossToastPopUp.Current.ShowToastError("Ошибка: Вы ввели неправильный номер!");
						return;
					}

					if (sendCodeResponse.ResponseStatus == AuthSendCodeStatus.GateWayError)
					{
						CrossToastPopUp.Current.ShowToastError(
							"Ошибка: Смс шлюз времнно недоступен, попробуйте позже.");
						return;
					}

					IsWaitingForCode = true;
					return;
				}

				int.TryParse(Code, out int code);
				
				var vadidateCodeResponse = await _api.AuthValidateCodeAsync(new AuthValidateCodeRequest()
					{Code = code, Phone = Utils.PhoneTrimer(Phone)});

				if (vadidateCodeResponse.ResponseStatus == AuthValidateCodeStatus.InvalidCode)
				{
					CrossToastPopUp.Current.ShowToastError("Ошибка: Вы ввели неверный код!", ToastLength.Short);
					IsBusy = false;
					return;
				}

				_api.Settings.AccessToken = vadidateCodeResponse.AccessToken;

				try
				{
					await SecureStorage.SetAsync(Constants.AccessTokenSecureStorageKey, vadidateCodeResponse.AccessToken);
				}
				catch (Exception ex)
				{
					CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.SecureStorageNotSupportedMessage} ({ex.Message})", ToastLength.Long);
				}

				await _navigationService.NavigateAsync("/DashboardPage/NavigationPage/ProfilePage");
			}
			catch (GrpcExceptionBase ex)
			{
				CrossToastPopUp.Current.ShowToastError($"{Constants.Messages.UnableToCallRpcMessage} ({ex.Message})");
			}
			IsBusy = false;
		}
	}
}
