// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MainPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.WP8.Sample
{
	using System;
	using System.Collections.ObjectModel;
	using System.Reactive;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using System.Threading;
	using Linq2Rest.Reactive.WP8.Sample.Models;
	using Linq2Rest.Reactive.WP8.Sample.Support;
	using Microsoft.Phone.Controls;

	public partial class MainPage : PhoneApplicationPage
	{
		private readonly IDisposable _nugetObservable;
		private readonly ObservableCollection<NugetPackage> _packageCollection;

		public MainPage()
		{
			_packageCollection = new ObservableCollection<NugetPackage>();
			Resources.Add("Packages", _packageCollection);
			InitializeComponent();
			var trigger = Observable.FromEventPattern(txtSearch, "TextChanged")
								 .Throttle(TimeSpan.FromMilliseconds(300))
								 .Do(_ => Dispatcher.BeginInvoke(() => _packageCollection.Clear()))
								 .Select(_ => Unit.Default);
			_nugetObservable = new RestObservable<NugetPackage>(
				new AsyncJsonRestClientFactory(new Uri("http://nuget.org/api/v2/Packages")), 
				new ODataSerializerFactory())
				.Poll(trigger)
				.Where(p => p.Id.ToLower()
							 .Contains(txtSearch.Text))
				.ObserveOn(new SynchronizationContextScheduler(SynchronizationContext.Current))
				.SubscribeOn(new SynchronizationContextScheduler(SynchronizationContext.Current))
				.SubscribeSafe(Observer.Create<NugetPackage>(_packageCollection.Add));
		}
	}
}