// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestPostQueryable.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestPostQueryable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq.Expressions;
	using Linq2Rest.Provider.Writers;

	internal class RestPostQueryable<T> : RestQueryableBase<T>
	{
		private readonly RestPostQueryProvider<T> _restPostQueryProvider;

		public RestPostQueryable(IRestClient client, ISerializerFactory serializerFactory, Expression expression, Stream inputData, Type sourceType)
			: this(client, serializerFactory, new MemberNameResolver(), new IValueWriter[0], expression, inputData, sourceType)
		{
			Contract.Requires(client != null);
			Contract.Requires(serializerFactory != null);
			Contract.Requires(expression != null);
			Contract.Requires(inputData != null);
		}

		public RestPostQueryable(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Expression expression, Stream inputData, Type sourceType)
			: base(client, serializerFactory, memberNameResolver, valueWriters)
		{
			Contract.Requires(client != null);
			Contract.Requires(serializerFactory != null);
			Contract.Requires(expression != null);
			Contract.Requires(memberNameResolver != null);
			Contract.Requires(valueWriters != null);
			Contract.Requires(inputData != null);

			_restPostQueryProvider = new RestPostQueryProvider<T>(
				client,
				serializerFactory,
				new ExpressionProcessor(new ExpressionWriter(memberNameResolver, ValueWriters), memberNameResolver),
				memberNameResolver,
				ValueWriters,
				inputData,
				sourceType);
			Provider = _restPostQueryProvider;
			Expression = expression;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					_restPostQueryProvider.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_restPostQueryProvider != null);
		}
	}
}