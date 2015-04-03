// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestGetQueryProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestGetQueryProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using Linq2Rest.Provider.Writers;

	internal class RestGetQueryProvider<T> : RestQueryProvider<T>
	{
		public RestGetQueryProvider(IRestClient client, ISerializerFactory serializerFactory, IExpressionProcessor expressionProcessor, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Type sourceType)
			: base(client, serializerFactory, expressionProcessor, memberNameResolver, valueWriters, sourceType)
		{
			Contract.Requires(client != null);
			Contract.Requires(serializerFactory != null);
			Contract.Requires(expressionProcessor != null);
			Contract.Requires(valueWriters != null);
			Contract.Requires(sourceType != null);
		}

		protected override Func<IRestClient, ISerializerFactory, IMemberNameResolver, IEnumerable<IValueWriter>, Expression, Type, IQueryable<TResult>> CreateQueryable<TResult>()
		{
			return InnerCreateQueryable<TResult>;
		}

		protected override IEnumerable<T> GetResults(ParameterBuilder builder)
		{
			var fullUri = builder.GetFullUri();
			var response = Client.Get(fullUri);
			var serializer = GetSerializer(builder.SourceType);
			var resultSet = serializer.DeserializeList(response);

			Contract.Assume(resultSet != null);

			return resultSet;
		}

		protected override IEnumerable GetIntermediateResults(Type type, ParameterBuilder builder)
		{
			var fullUri = builder.GetFullUri();
			var response = Client.Get(fullUri);

			var serializer = GetSerializer(type, builder.SourceType);
			var deserializeListMethod = serializer.GetType().GetMethod("DeserializeList");
			var resultSet = (IEnumerable)deserializeListMethod.Invoke(serializer, new object[] { response });

			return resultSet;
		}

		private IQueryable<TResult> InnerCreateQueryable<TResult>(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Expression expression, Type sourceType)
		{
			Contract.Requires(client != null);
			Contract.Requires(serializerFactory != null);
			Contract.Requires(memberNameResolver != null);
			Contract.Requires(valueWriters != null);
			Contract.Requires(expression != null);
			Contract.Requires(sourceType != null);

			return new RestGetQueryable<TResult>(
				client,
				serializerFactory,
				memberNameResolver,
				valueWriters,
				sourceType,
				expression);
		}
	}
}