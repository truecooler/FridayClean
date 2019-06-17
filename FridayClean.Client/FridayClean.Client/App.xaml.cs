using System;
using FridayClean.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FridayClean.Client.Views;
using FridayClean.Client.Api;
using Prism;
using Prism.Ioc;
using System.Threading.Tasks;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using System.IO;
using FridayClean.Common.Helpers;
using Xamarin.Essentials;
using FridayClean.Common;
//using System.ServiceModel.Internals;

namespace FridayClean.Client
{
	public partial class App
	{
		/* 
		 * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
		 * This imposes a limitation in which the App class must have a default constructor. 
		 * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
		 */
		public App() : this(null) { }

		public App(IPlatformInitializer initializer) : base(initializer) { }

		

		protected override async void OnInitialized()
		{
			InitializeComponent();

			Utils.AppCrashHelper.SubscribeForUnhandledAndUnobservedExceptions();

			AppCenter.Start("android=7c7d0384-5cae-4425-b03c-17af65dddeea;" +
			                "uwp={Your UWP App secret here};" +
			                "ios={Your iOS App secret here}",
				typeof(Analytics), typeof(Crashes), typeof(Push));

			await NavigationService
					.NavigateAsync("/LoginPage"); // + "/" + nameof(NavigationPage) + "/" + nameof(ViewA)
			
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterForNavigation<NavigationPage>();
			containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
			containerRegistry.RegisterForNavigation<DashboardPage, DashboardPageViewModel>();
			containerRegistry.RegisterForNavigation<CleaningPage>();
			containerRegistry.RegisterForNavigation<ProfilePage>();
			containerRegistry.RegisterForNavigation<RatingPage>();
			containerRegistry.RegisterForNavigation<EditProfilePage>();
			//containerRegistry.RegisterInstance<IFridayCleanApi>(
			//new FridayCleanApi(FridayCleanApiSettings.ProductionDefault()));

			containerRegistry.RegisterInstance(FridayCleanApiSettings.ProductionDefault());
			containerRegistry.RegisterSingleton<IFridayCleanApi, FridayCleanApi>();


			containerRegistry.RegisterForNavigation<EditProfilePage, EditProfilePageViewModel>();
		}
	}
}
