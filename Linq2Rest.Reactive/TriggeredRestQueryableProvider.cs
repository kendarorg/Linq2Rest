// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggeredRestQueryableProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TriggeredRestQueryableProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;
	using System.Reactive;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;

	internal class TriggeredRestQueryableProvider<TSource> : RestQueryableProviderBase<TSource>
	{
		private readonly IObservable<Unit> _trigger;

		public TriggeredRestQueryableProvider(
			IObservable<Unit> trigger,
			IAsyncRestClientFactory asyncRestClient,
			ISerializerFactory serializerFactory,
			IScheduler subscriberScheduler,
			IScheduler observerScheduler)
			: this(trigger, asyncRestClient, serializerFactory, new MemberNameResolver(), new IValueWriter[0], subscriberScheduler, observerScheduler)
		{
			Contract.Requires(trigger != null);
			Contract.Requires(asyncRestClient != null);
			Contract.Requires(serializerFactory != null);
			Contract.Requires(subscriberScheduler != null);
			Contract.Requires(observerScheduler != null);

			_trigger = trigger;
		}

		public TriggeredRestQueryableProvider(
			IObservable<Unit> trigger,
			IAsyncRestClientFactory asyncRestClient,
			ISerializerFactory serializerFactory,
			IMemberNameResolver memberNameResolver,
			IEnumerable<IValueWriter> valueWriters,
			IScheduler subscriberScheduler,
			IScheduler observerScheduler)
			: base(asyncRestClient, serializerFactory, memberNameResolver, valueWriters, subscriberScheduler, observerScheduler)
		{
			Contract.Requires(trigger != null);
			Contract.Requires(asyncRestClient != null);
			Contract.Requires(serializerFactory != null);
			Contract.Requires(subscriberScheduler != null);
			Contract.Requires(memberNameResolver != null);
			Contract.Requires(valueWriters != null);
			Contract.Requires(observerScheduler != null);

			_trigger = trigger;
		}

		protected override IQbservable<TResult> CreateQbservable<TResult>(Expression expression, IScheduler subscriberScheduler, IScheduler observerScheduler)
		{
			return new TriggeredRestObservable<TResult, TSource>(_trigger, AsyncRestClient, SerializerFactory, MemberNameResolver, ValueWriters, expression, subscriberScheduler, observerScheduler);
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_trigger != null);
		}
	}
}