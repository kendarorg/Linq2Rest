// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggeredRestObservableTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TriggeredRestObservableTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.Tests
{
	using System;
	using System.IO;
	using System.Reactive;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Linq2Rest.Reactive.Tests.Fakes;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class TriggeredRestObservableTests
	{
		[Test]
		public void WhenDisposingPollSubscriptionThenCompletes()
		{
			var waitHandle = new ManualResetEvent(false);
			var factory = new FakeAsyncRestClientFactory("[{\"Text\":\"blah\", \"Number\":1}]");
			var observable = new RestObservable<FakeItem>(factory, new TestSerializerFactory());
			var subscription = observable
				.Poll(Observable.Interval(TimeSpan.FromSeconds(0.5)).Select(x => Unit.Default))
				.Where(x => x.StringValue == "blah")
				.Subscribe(x => { }, () => waitHandle.Set());

			Task.Factory.StartNew(
				() =>
				{
					Thread.Sleep(1000);
					subscription.Dispose();
				});

			var result = waitHandle.WaitOne(2000);

			Assert.True(result);

			subscription.Dispose();
		}

		[Test]
		public void WhenInvokingThenCallsRestClient()
		{
			var waitHandle = new ManualResetEvent(false);
			var mockRestClient = new Mock<IAsyncRestClient>();
			mockRestClient.Setup(x => x.Download())
				.Returns(() => Task<Stream>.Factory.StartNew(() => "[]".ToStream()));

			var mockClientFactory = new Mock<IAsyncRestClientFactory>();
			mockClientFactory.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			mockClientFactory.Setup(x => x.Create(It.IsAny<Uri>()))
				.Callback<Uri>(Console.WriteLine)
				.Returns(mockRestClient.Object);

			var subscription = new RestObservable<FakeItem>(mockClientFactory.Object, new TestSerializerFactory())
				.Poll(Observable.Repeat(Unit.Default, 2))
				.Where(x => x.IntValue == 2)
				.Subscribe(x => { }, () => waitHandle.Set());

			waitHandle.WaitOne(2000);

			mockClientFactory.Verify(x => x.Create(It.IsAny<Uri>()), Times.Exactly(2));

			subscription.Dispose();
		}

		[Test]
		public void WhenObservablePollsThenDoesNotComplete()
		{
			var waitHandle = new ManualResetEvent(false);
			var factory = new FakeAsyncRestClientFactory("[{\"Text\":\"blah\", \"Number\":1}]");
			var observable = new RestObservable<FakeItem>(factory, new TestSerializerFactory());
			var subscription = observable
				.Poll(Observable.Interval(TimeSpan.FromSeconds(0.5)).Select(x => Unit.Default))
				.ObserveOn(TaskPoolScheduler.Default)
				.Where(x => x.StringValue == "blah")
				.Subscribe(x => { }, () => waitHandle.Set());

			var result = waitHandle.WaitOne(2000);

			Assert.False(result);

			subscription.Dispose();
		}
	}
}