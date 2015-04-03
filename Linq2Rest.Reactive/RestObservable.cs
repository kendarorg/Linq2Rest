// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestObservable.cs" company="Reimers.dk">
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
	using System.Linq;
	using System.Reactive;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;

	/// <summary>
	/// Defines an observable REST query.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> of object returned by the REST service.</typeparam>
	public class RestObservable<T>
	{
		private readonly IAsyncRestClientFactory _restClientFactory;
		private readonly ISerializerFactory _serializerFactory;
		private readonly IMemberNameResolver _memberNameResolver;
		private readonly IEnumerable<IValueWriter> _valueWriters;

		/// <summary>
		/// Initializes a new instance of the <see cref="RestObservable{T}"/> class.
		/// </summary>
		/// <param name="restClientFactory">An <see cref="IAsyncRestClientFactory"/> to perform requests.</param>
		/// <param name="serializerFactory">An <see cref="ISerializerFactory"/> to perform deserialization.</param>
		public RestObservable(IAsyncRestClientFactory restClientFactory, ISerializerFactory serializerFactory)
			: this(restClientFactory, serializerFactory, new MemberNameResolver(), Enumerable.Empty<IValueWriter>())
		{
			Contract.Requires<ArgumentNullException>(restClientFactory != null);
			Contract.Requires<ArgumentNullException>(serializerFactory != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RestObservable{T}"/> class.
		/// </summary>
		/// <param name="restClientFactory">An <see cref="IAsyncRestClientFactory"/> to perform requests.</param>
		/// <param name="serializerFactory">An <see cref="ISerializerFactory"/> to perform deserialization.</param>
		/// <param name="memberNameResolver">An <see cref="IMemberNameResolver"/> for member name resolution.</param>
		/// <param name="valueWriters">Custom value writers.</param>
		public RestObservable(IAsyncRestClientFactory restClientFactory, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters)
		{
			Contract.Requires<ArgumentNullException>(restClientFactory != null);
			Contract.Requires<ArgumentNullException>(serializerFactory != null);
			Contract.Requires<ArgumentNullException>(memberNameResolver != null);
			Contract.Requires<ArgumentNullException>(valueWriters != null);

			_restClientFactory = restClientFactory;
			_serializerFactory = serializerFactory;
			_memberNameResolver = memberNameResolver;
			_valueWriters = valueWriters.ToArray();
		}

		/// <summary>
		/// Creates an observable performing a single call to the defined service.
		/// </summary>
		/// <returns>An instance of an <see cref="IQbservable{T}"/>.</returns>
		public IQbservable<T> Create()
		{
			Contract.Assume(ImmediateScheduler.Instance != null);

			return new InnerRestObservable<T, T>(
				_restClientFactory,
				_serializerFactory,
				_memberNameResolver, 
				_valueWriters,
				null,
				ImmediateScheduler.Instance,
				ImmediateScheduler.Instance);
		}

		/// <summary>
		/// Creates an observable performing calls when triggered.
		/// </summary>
		/// <param name="trigger">The trigger.</param>
		/// <returns>An instance of an <see cref="IQbservable{T}"/>.</returns>
		public IQbservable<T> Poll(IObservable<Unit> trigger)
		{
			Contract.Requires<ArgumentNullException>(trigger != null);
			Contract.Assume(ImmediateScheduler.Instance != null);

			return new TriggeredRestObservable<T, T>(
				trigger,
				_restClientFactory,
				_serializerFactory,
				_memberNameResolver,
				_valueWriters,
				null,
				ImmediateScheduler.Instance,
				ImmediateScheduler.Instance);
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_restClientFactory != null);
			Contract.Invariant(_serializerFactory != null);
			Contract.Invariant(_memberNameResolver != null);
			Contract.Invariant(_valueWriters != null);
		}
	}
}
