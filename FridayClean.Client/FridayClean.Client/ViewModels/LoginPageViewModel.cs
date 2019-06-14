using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using FridayClean.Client.Views;
using FridayClean.Client.Api;
using FridayClean.Client.Helpers;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using Grpc.Core;
using FridayClean.Client.Api.Exceptions;

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
		public ICommand LoginCommand { protected set; get; }
		public LoginPageViewModel(INavigationService navigationService,
			IPageDialogService dialogService, IFridayCleanApi api) : base(navigationService)
		{
			

			_dialogService = dialogService;
			_api = api;
			_navigationService = navigationService;
			LoginCommand = new Command(OnLoginAsync,()=>false);
		}

		public override async void OnNavigatedTo(INavigationParameters parameters)
		{
			await Task.Yield();
			if (Utils.AppCrashHelper.IsCrashLogExists)
			{
				await _dialogService.DisplayAlertAsync("Last crash info", Utils.AppCrashHelper.ReadCrashLogFile(), "OK");
			}
		}

		public async void OnLoginAsync()
		{
			try
			{
				if (!IsWaitingForCode)
				{
					IsBusy = true;
					var sendCodeResponse = await _api.AuthSendCodeAsync(new AuthSendCodeRequest()
						{Phone = Utils.PhoneTrimer(Phone)});
					IsBusy = false;
					if (sendCodeResponse.ResponseStatus == AuthSendCodeResponseStatus.InvalidPhone)
					{
						CrossToastPopUp.Current.ShowToastError("Ошибка: Вы ввели неправильный номер!");
						return;
					}

					if (sendCodeResponse.ResponseStatus == AuthSendCodeResponseStatus.GateWayError)
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
					{AuthCode = code, Phone = Utils.PhoneTrimer(Phone)});

				if (vadidateCodeResponse.ResponseStatus == AuthValidateCodeResponseStatus.InvalidCode)
				{
					CrossToastPopUp.Current.ShowToastError("Ошибка: Вы ввели неверный код!", ToastLength.Short);
					return;
				}

				await _navigationService.NavigateAsync("/DashboardPage");
			}
			catch (GrpcUnavailableException ex)
			{
				CrossToastPopUp.Current.ShowToastError($"Ошибка: Невозможно выполнить запрос. ({ex.Message})");
				IsBusy = false;
			}
		}
	}
}
