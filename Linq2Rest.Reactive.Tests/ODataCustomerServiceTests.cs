// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ODataCustomerServiceTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ODataCustomerServiceTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.Tests
{
	using System;
	using System.Reactive.Linq;
	using System.Threading;
	using Linq2Rest.Reactive.Implementations;
	using Linq2Rest.Reactive.Tests.Fakes;
	using NUnit.Framework;

	[TestFixture]
	public class ODataCustomerServiceTests
	{
		private RestObservable<NorthwindCustomer> _customerContext;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			// Tests against the sample OData service.
			_customerContext = new RestObservable<NorthwindCustomer>(
				new AsyncJsonRestClientFactory(new Uri("http://services.odata.org/Northwind/Northwind.svc/Customers")), 
				new TestODataSerializerFactory());
		}

		[Test]
		public void WhenRequestingCustomerByNameEndsWithThenLoadsCustomer()
		{
			var waitHandle = new ManualResetEvent(false);

			_customerContext
				.Create()
				.Where(x => x.CompanyName.EndsWith("Futterkiste"))
				.Subscribe(
					x => waitHandle.Set(), 
					e => Console.WriteLine(e.Message), 
					() => waitHandle.Set());

			var result = waitHandle.WaitOne(5000);

			Assert.True(result);
		}

		[Test]
		public void WhenRequestingCustomerByNameLengthThenLoadsCustomer()
		{
			var waitHandle = new ManualResetEvent(false);

			_customerContext
				.Create()
				.Where(x => x.CompanyName.Length > 10)
				.Subscribe(x => waitHandle.Set(), () => waitHandle.Set());

			var result = waitHandle.WaitOne(2000);

			Assert.True(result);
		}

		[Test]
		public void WhenRequestingCustomerByNameStartsWithThenLoadsCustomer()
		{
			var waitHandle = new ManualResetEvent(false);

			_customerContext
				.Create()
				.Where(x => x.CompanyName.StartsWith("Alfr"))
				.Subscribe(x => waitHandle.Set(), () => waitHandle.Set());

			var result = waitHandle.WaitOne(2000);

			Assert.True(result);
		}

		[Test]
		public void WhenRequestingCustomerByNameThenLoadsCustomer()
		{
			var waitHandle = new ManualResetEvent(false);

			_customerContext
				.Create()
				.Where(x => x.CompanyName.IndexOf("Alfreds") > -1)
				.Subscribe(x => waitHandle.Set(), () => waitHandle.Set());

			var result = waitHandle.WaitOne(2000);

			Assert.True(result);
		}
	}
}