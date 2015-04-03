// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestQueryableProviderBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestQueryableProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;

	[ContractClass(typeof(RestQueryableProviderBaseContracts<>))]
	internal abstract class RestQueryableProviderBase<TSource> : IQbservableProvider
	{
		private readonly IAsyncRestClientFactory _asyncRestClient;
		private readonly IScheduler _observerScheduler;
		private readonly ISerializerFactory _serializerFactory;
		private readonly IScheduler _subscriberScheduler;

		public RestQueryableProviderBase(
			IAsyncRestClientFactory asyncRestClient,
			ISerializerFactory serializerFactory,
			IMemberNameResolver memberNameResolver,
			IEnumerable<IValueWriter> valueWriters,
			IScheduler subscriberScheduler,
			IScheduler observerScheduler)
		{
			Contract.Requires(asyncRestClient != null);
			Contract.Requires(serializerFactory != null);
			Contract.Requires(memberNameResolver != null);
			Contract.Requires(valueWriters != null);
			Contract.Requires(subscriberScheduler != null);
			Contract.Requires(observerScheduler != null);

			MemberNameResolver = memberNameResolver;
			ValueWriters = valueWriters;
			_asyncRestClient = asyncRestClient;
			_serializerFactory = serializerFactory;
			_subscriberScheduler = subscriberScheduler;
			_observerScheduler = observerScheduler;
		}

		protected IMemberNameResolver MemberNameResolver { get; private set; }

		protected IEnumerable<IValueWriter> ValueWriters { get; private set; }

		protected IAsyncRestClientFactory AsyncRestClient
		{
			get { return _asyncRestClient; }
		}

		protected ISerializerFactory SerializerFactory
		{
			get { return _serializerFactory; }
		}

		public IQbservable<TResult> CreateQuery<TResult>(Expression expression)
		{
			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null)
			{
				switch (methodCallExpression.Method.Name)
				{
					case "SubscribeOn":
						{
							var constantExpression = methodCallExpression.Arguments[1] as ConstantExpression;

							Contract.Assume(constantExpression != null);

							var subscribeScheduler = constantExpression.Value as IScheduler;

							Contract.Assume(subscribeScheduler != null);

							return CreateQbservable<TResult>(
															 methodCallExpression.Arguments[0],
															 subscribeScheduler,
															 _observerScheduler);
						}

					case "ObserveOn":
						{
							var constantExpression = methodCallExpression.Arguments[1] as ConstantExpression;

							Contract.Assume(constantExpression != null);

							var observeScheduler = constantExpression.Value as IScheduler;

							Contract.Assume(observeScheduler != null);

							return CreateQbservable<TResult>(
															 methodCallExpression.Arguments[0],
															 _subscriberScheduler,
															 observeScheduler);
						}
				}
			}

			return CreateQbservable<TResult>(expression, _subscriberScheduler, _observerScheduler);
		}

		protected abstract IQbservable<TResult> CreateQbservable<TResult>(Expression expression, IScheduler subscriberScheduler, IScheduler observerScheduler);

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_asyncRestClient != null);
			Contract.Invariant(_serializerFactory != null);
			Contract.Invariant(_subscriberScheduler != null);
			Contract.Invariant(_observerScheduler != null);
			Contract.Invariant(MemberNameResolver != null);
			Contract.Invariant(ValueWriters != null);
		}
	}

	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Contract class.")]
	[ContractClassFor(typeof(RestQueryableProviderBase<>))]
	internal abstract class RestQueryableProviderBaseContracts<TSource> : RestQueryableProviderBase<TSource>
	{
		protected RestQueryableProviderBaseContracts(
			IAsyncRestClientFactory asyncRestClient,
			ISerializerFactory serializerFactory,
			IMemberNameResolver memberNameResolver,
			IEnumerable<IValueWriter> valueWriters,
			IScheduler subscriberScheduler,
			IScheduler observerScheduler)
			: base(asyncRestClient, serializerFactory, memberNameResolver, valueWriters, subscriberScheduler, observerScheduler)
		{
			Contract.Requires(asyncRestClient != null);
			Contract.Requires(serializerFactory != null);
			Contract.Requires(memberNameResolver != null);
			Contract.Requires(valueWriters != null);
			Contract.Requires(subscriberScheduler != null);
			Contract.Requires(observerScheduler != null);
		}

		protected override IQbservable<TResult> CreateQbservable<TResult>(Expression expression, IScheduler subscriberScheduler, IScheduler observerScheduler)
		{
			Contract.Requires(subscriberScheduler != null);
			Contract.Requires(observerScheduler != null);
			Contract.Ensures(Contract.Result<IQbservable<TResult>>() != null);
			throw new NotImplementedException();
		}
	}
}