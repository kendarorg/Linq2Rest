// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Provides application-specific behavior to supplement the default Application class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.WinRT.Sample
{
	using Windows.ApplicationModel;
	using Windows.ApplicationModel.Activation;
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Controls;

	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="App"/> class. 
		/// This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			InitializeComponent();
			Suspending += OnSuspending;
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used when the application is launched to open a specific file, to display
		/// search results, and so forth.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
			{
				// TODO: Load state from previously suspended application
			}

			// Create a Frame to act navigation context and navigate to the first page
			var rootFrame = new Frame();
			rootFrame.Navigate(typeof(BlankPage));

			// Place the frame in the current Window and ensure that it is active
			Window.Current.Content = rootFrame;
			Window.Current.Activate();
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			// TODO: Save application state and stop any background activity
		}
	}
}
