// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InnerRestObservable.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines an observable REST query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;

	/// <summary>
	/// Defines an observable REST query.
	/// </summary>
	/// <typeparam name="TItem">The <see cref="Type"/> of object returned by the REST service.</typeparam>
	/// <typeparam name="TSource">The <see cref="Type"/> of source item to derive aliasing from.</typeparam>
	internal class InnerRestObservable<TItem, TSource> : InnerRestObservableBase<TItem, TSource>
	{
		private readonly RestQueryableProvider<TSource> _provider;

		internal InnerRestObservable(
			IAsyncRestClientFactory restClient,
			ISerializerFactory serializerFactory,
			IMemberNameResolver memberNameResolver,
			IEnumerable<IValueWriter> valueWriters,
			Expression expression,
			IScheduler subscriberScheduler,
			IScheduler observerScheduler)
			: base(restClient, serializerFactory, memberNameResolver, valueWriters, expression, subscriberScheduler, observerScheduler)
		{
			Contract.Requires(restClient != null);
			Contract.Requires(serializerFactory != null);
			Contract.Requires(memberNameResolver != null);
			Contract.Requires(valueWriters != null);
			Contract.Requires(subscriberScheduler != null);
			Contract.Requires(observerScheduler != null);

			_provider = new RestQueryableProvider<TSource>(restClient, serializerFactory, memberNameResolver, ValueWriters, subscriberScheduler, observerScheduler);
		}

		/// <summary>
		/// Gets the query provider that is associated with this data source.
		/// </summary>
		public override IQbservableProvider Provider
		{
			get { return _provider; }
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_provider != null);
		}
	}
}