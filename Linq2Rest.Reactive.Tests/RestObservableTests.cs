// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestObservableTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestObservableTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.Tests
{
	using System;
	using System.IO;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Linq2Rest.Reactive.Tests.Fakes;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RestObservableTests
	{
		[Test]
		public void CanCreateQbservable()
		{
			Assert.DoesNotThrow(
								() => new RestObservable<FakeItem>(
									new FakeAsyncRestClientFactory(),
									new TestSerializerFactory()));
		}

		[Test]
		public void CanCreateSubscription()
		{
			var waitHandle = new ManualResetEvent(false);
			var observable = new RestObservable<FakeItem>(new FakeAsyncRestClientFactory(), new TestSerializerFactory());

			observable
				.Create()
				.SubscribeOn(NewThreadScheduler.Default)
				.Where(x => x.StringValue == "blah")
				.ObserveOn(Scheduler.Default)
				.Subscribe(
						   x =>
						   {
							   Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
							   waitHandle.Set();
						   },
						   () =>
						   {
							   Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
							   waitHandle.Set();
						   });

			var result = waitHandle.WaitOne(5000);

			Assert.True(result);
		}

		[Test]
		public void WhenDisposingSubscriptionThenDoesNotExecute()
		{
			var completedWaitHandle = new ManualResetEvent(false);
			var onnextWaitHandle = new ManualResetEvent(false);

			var observable = new RestObservable<FakeItem>(new FakeAsyncRestClientFactory(2000), new TestSerializerFactory());
			var subscription = observable
				.Create()
				.Where(x => x.StringValue == "blah")
				.ObserveOn(Scheduler.CurrentThread)
				.Subscribe(x => onnextWaitHandle.Set(), () => completedWaitHandle.Set());

			subscription.Dispose();

			var next = onnextWaitHandle.WaitOne(2000);
			var completed = completedWaitHandle.WaitOne(2000);

			Assert.False(next);
			Assert.True(completed);
		}

		[Test]
		public void WhenGettingSingleThenReturnsResults()
		{
			var observable = new RestObservable<FakeItem>(new FakeAsyncRestClientFactory(), new TestSerializerFactory());
			var result = observable
				.Create()
				.Where(x => x.StringValue == "blah")
				.SingleOrDefault();

			Assert.Null(result);
		}

		[Test]
		public void WhenGroupingSourceThenReturnsResults()
		{
			var waitHandle = new ManualResetEvent(false);
			var observable = new RestObservable<FakeItem>(new FakeAsyncRestClientFactory(), new TestSerializerFactory());
			observable
				.Create()
				.Where(x => x.StringValue == "blah")
				.GroupBy(x => x.StringValue)
				.Subscribe(
					x => { },
					e =>
						{
							Console.WriteLine(e.Message);
							Console.WriteLine(e.StackTrace);
							waitHandle.Set();
						},
					() => waitHandle.Set());

			var result = waitHandle.WaitOne();

			Assert.True(result);
		}

		[Test]
		public void WhenInvokingDeleteThenHttpMethodIsSetOnClientFactory()
		{
			var waitHandle = new ManualResetEvent(false);

			var mockRestClient = new Mock<IAsyncRestClient>();
			mockRestClient.Setup(x => x.Download())
				.Returns(() => Task<Stream>.Factory.StartNew(() => "[]".ToStream()));

			var mockClientFactory = new Mock<IAsyncRestClientFactory>();
			mockClientFactory.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			mockClientFactory.Setup(x => x.Create(It.IsAny<Uri>())).Returns(mockRestClient.Object);

			new RestObservable<FakeItem>(mockClientFactory.Object, new TestSerializerFactory())
				.Create()
				.Delete()
				.Where(x => x.StringValue == "blah")
				.Subscribe(x => waitHandle.Set(), () => waitHandle.Set());

			waitHandle.WaitOne(5000);

			mockClientFactory.Verify(x => x.SetMethod(HttpMethod.Delete));
		}

		[Test]
		public void WhenInvokingPostThenHttpMethodIsSetOnClientFactory()
		{
			var waitHandle = new ManualResetEvent(false);

			var mockRestClient = new Mock<IAsyncRestClient>();
			mockRestClient.Setup(x => x.Download())
				.Returns(() => Task<Stream>.Factory.StartNew(() => "[]".ToStream()));

			var mockClientFactory = new Mock<IAsyncRestClientFactory>();
			mockClientFactory.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			mockClientFactory.Setup(x => x.Create(It.IsAny<Uri>())).Returns(mockRestClient.Object);

			new RestObservable<FakeItem>(mockClientFactory.Object, new TestSerializerFactory())
				.Create()
				.Post(() => 1)
				.Where(x => x.StringValue == "blah")
				.Subscribe(x => waitHandle.Set(), () => waitHandle.Set());

			waitHandle.WaitOne(5000);

			mockClientFactory.Verify(x => x.SetMethod(HttpMethod.Post));
		}

		[Test]
		public void WhenInvokingPutThenHttpMethodIsSetOnClientFactory()
		{
			var waitHandle = new ManualResetEvent(false);

			var mockRestClient = new Mock<IAsyncRestClient>();
			mockRestClient.Setup(x => x.Download())
				.Returns(() => Task<Stream>.Factory.StartNew(() => "[]".ToStream()));

			var mockClientFactory = new Mock<IAsyncRestClientFactory>();
			mockClientFactory.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			mockClientFactory.Setup(x => x.Create(It.IsAny<Uri>())).Returns(mockRestClient.Object);

			new RestObservable<FakeItem>(mockClientFactory.Object, new TestSerializerFactory())
				.Create()
				.Put(() => 1)
				.Where(x => x.StringValue == "blah")
				.Subscribe(x => waitHandle.Set(), () => waitHandle.Set());

			waitHandle.WaitOne(5000);

			mockClientFactory.Verify(x => x.SetMethod(HttpMethod.Put));
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
			mockClientFactory.Setup(x => x.Create(It.IsAny<Uri>())).Returns(mockRestClient.Object);

			new RestObservable<FakeItem>(mockClientFactory.Object, new TestSerializerFactory())
				.Create()
				.Where(x => x.StringValue == "blah")
				.Subscribe(x => waitHandle.Set(), () => waitHandle.Set());

			waitHandle.WaitOne(5000);

			mockRestClient.Verify(x => x.Download());
		}

		[Test]
		public void WhenInvokingWithExpandThenCallsRestClient()
		{
			var waitHandle = new ManualResetEvent(false);

			var mockRestClient = new Mock<IAsyncRestClient>();
			mockRestClient.Setup(x => x.Download())
				.Returns(() => Task<Stream>.Factory.StartNew(() => "[]".ToStream()));

			var mockClientFactory = new Mock<IAsyncRestClientFactory>();
			mockClientFactory.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			mockClientFactory.Setup(x => x.Create(It.IsAny<Uri>())).Returns(mockRestClient.Object);

			new RestObservable<FakeItem>(mockClientFactory.Object, new TestSerializerFactory())
				.Create()
				.Expand<FakeItem, FakeItem>(i => i.Children, i => i.MoreChildren)
				.Subscribe(x => waitHandle.Set(), () => waitHandle.Set());

			waitHandle.WaitOne(5000);

			mockRestClient.Verify(x => x.Download());
		}

		[Test]
		public void WhenObservingOnDifferentSchedulerThenInvocationHappensOnDifferentThread()
		{
			var testThreadId = Thread.CurrentThread.ManagedThreadId;

			var waitHandle = new ManualResetEvent(false);
			var observable = new RestObservable<FakeItem>(new FakeAsyncRestClientFactory(), new TestSerializerFactory());
			observable
				.Create()
				.Where(x => x.StringValue == "blah")
				.ObserveOn(Scheduler.Default)
				.Subscribe(
					x =>
					{
					},
					() =>
					{
						var observerThreadId = Thread.CurrentThread.ManagedThreadId;
						if (observerThreadId != testThreadId)
						{
							waitHandle.Set();
						}
					});

			var result = waitHandle.WaitOne(2000);

			Assert.True(result);
		}

		[Test]
		public void WhenResultReturnedThenCompletesSubscription()
		{
			var waitHandle = new ManualResetEvent(false);
			var observable = new RestObservable<FakeItem>(new FakeAsyncRestClientFactory(), new TestSerializerFactory());
			var subscription = observable
				.Create()
				.Where(x => x.StringValue == "blah")
				.Subscribe(x => { }, () => waitHandle.Set());

			var result = waitHandle.WaitOne();

			Assert.True(result);
		}
	}
}