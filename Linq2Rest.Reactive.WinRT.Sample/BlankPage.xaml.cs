// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlankPage.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   An empty page that can be used on its own or navigated to within a Frame.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.WinRT.Sample
{
	using System;
	using System.Reactive.Linq;
	using Linq2Rest.Reactive.Implementations;
	using Linq2Rest.Reactive.WinRT.Sample.Models;
	using Linq2Rest.Reactive.WinRT.Sample.Support;
	using Windows.UI.Core;
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Controls;

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class BlankPage : Page
	{
		public BlankPage()
		{
			InitializeComponent();
		}

		private void AddFilm(NetflixFilm film)
		{
			films.Items.Add(film);
		}

		private void OnSearch(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			button.IsEnabled = false;
			if (films.Items.Count > 0)
			{
				films.Items.Clear();
			}

			var query = search.Text;
			new RestObservable<NetflixFilm>(
				new AsyncJsonRestClientFactory(
					new Uri("http://odata.netflix.com/v2/Catalog/Titles")), 
				new ODataSerializerFactory())
				.Create()
				.Where(x => x.Name.Contains(query))
				.Subscribe(
					x => Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => AddFilm(x)), 
					ex => Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => button.IsEnabled = true), 
					() => Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => button.IsEnabled = true));
		}
	}
}
