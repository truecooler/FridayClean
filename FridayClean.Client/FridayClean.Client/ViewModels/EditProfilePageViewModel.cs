using FridayClean.Client.Api;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FridayClean.Client.ViewModels
{
	public class EditProfilePageViewModel : ViewModelBase
	{
		private INavigationService _navigationService;
		private IFridayCleanApi _api;
		public EditProfilePageViewModel(IFridayCleanApi api, INavigationService navigationService) : base(navigationService)
		{
			_api = api;
			_navigationService = navigationService;
		}
	}
}
