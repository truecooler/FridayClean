using System;
using FridayClean.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FridayClean.Client.Views;
using FridayClean.Client.Api;
using Prism;
using Prism.Ioc;

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
			await NavigationService.NavigateAsync(nameof(LoginPage));// + "/" + nameof(NavigationPage) + "/" + nameof(ViewA)
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterForNavigation<NavigationPage>();
			containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
			containerRegistry.RegisterForNavigation<DashboardPage, DashboardPageViewModel>();
			containerRegistry.RegisterForNavigation<ContactsPage>();
			containerRegistry.RegisterForNavigation<ReminderPage>();
			containerRegistry.RegisterForNavigation<TodoListPage>();
		}
	}
}
