// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncExpressionProcessor.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   The default async expression processor implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reactive.Linq;

#if NETFX_CORE
	using System.Reflection;
#endif

	using Linq2Rest.Provider;

	/// <summary>
	/// The default async expression processor implementation.
	/// </summary>
	internal class AsyncExpressionProcessor : IAsyncExpressionProcessor
	{
		private readonly IExpressionWriter _writer;
		private readonly IMemberNameResolver _memberNameResolver;

		public AsyncExpressionProcessor(IExpressionWriter writer, IMemberNameResolver memberNameResolver)
		{
			Contract.Requires(writer != null);
			Contract.Requires(memberNameResolver != null);

			_writer = writer;
			_memberNameResolver = memberNameResolver;
		}

		public IObservable<T> ProcessMethodCall<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IObservable<IEnumerable<T>>> resultLoader, Func<Type, ParameterBuilder, IObservable<IEnumerable>> intermediateResultLoader)
		{
			var task = ProcessMethodCallInternal(methodCall, builder, resultLoader, intermediateResultLoader);
			return task == null
					   ? resultLoader(builder)
							 .SelectMany(x => x)
					   : task.Select(o => (IQbservable<T>)o)
							 .SelectMany(x => x);
		}

		private static IObservable<object> InvokeEager<T>(MethodCallExpression methodCall, object source)
		{
			Contract.Requires(methodCall != null);

			var enumerableSource = source as IEnumerable;

			Contract.Assume(enumerableSource != null);

			var parameters = ResolveInvocationParameters(enumerableSource, typeof(T), methodCall);
			return Observable.Return(methodCall.Method.Invoke(null, parameters));
		}

		private static object[] ResolveInvocationParameters(IEnumerable results, Type type, MethodCallExpression methodCall)
		{
			Contract.Requires(results != null);
			Contract.Requires(type != null);
			Contract.Requires(methodCall != null);

			var parameters = new[] { results.ToQbservable(type) }
				.Concat(methodCall.Arguments.Where((x, i) => i > 0).Select(GetExpressionValue))
				.Where(x => !ReferenceEquals(x, null))
				.ToArray();
			return parameters;
		}

		private static object GetExpressionValue(Expression expression)
		{
			if (expression is UnaryExpression)
			{
				return (expression as UnaryExpression).Operand;
			}

			if (expression is ConstantExpression)
			{
				return (expression as ConstantExpression).Value;
			}

			return null;
		}

		private IObservable<object> ProcessMethodCallInternal<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IObservable<IEnumerable<T>>> resultLoader, Func<Type, ParameterBuilder, IObservable<IEnumerable>> intermediateResultLoader)
		{
			Contract.Requires(builder != null);
			Contract.Requires(resultLoader != null);

			if (methodCall == null)
			{
				return null;
			}

			var method = methodCall.Method.Name;

			switch (method)
			{
				case "First":
				case "FirstOrDefault":
					builder.TakeParameter = "1";
					return methodCall.Arguments.Count >= 2
							? GetMethodResult(methodCall, builder, resultLoader, intermediateResultLoader)
							: GetResult(methodCall, builder, resultLoader, intermediateResultLoader);
				case "Where":
					Contract.Assume(methodCall.Arguments.Count >= 2);
					{
						var result = ProcessMethodCallInternal(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
						if (result != null)
						{
							return InvokeEager<T>(methodCall, result);
						}

						var newFilter = _writer.Write(methodCall.Arguments[1], builder.SourceType);

						builder.FilterParameter = string.IsNullOrWhiteSpace(builder.FilterParameter)
													? newFilter
													: string.Format("({0}) and ({1})", builder.FilterParameter, newFilter);
					}

					break;
				case "Select":
					Contract.Assume(methodCall.Arguments.Count >= 2);
					{
						var result = ProcessMethodCallInternal(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
						if (result != null)
						{
							return InvokeEager<T>(methodCall, result);
						}

						var unaryExpression = methodCall.Arguments[1] as UnaryExpression;
						if (unaryExpression != null)
						{
							var lambdaExpression = unaryExpression.Operand as LambdaExpression;
							if (lambdaExpression != null)
							{
								ResolveProjection(builder, lambdaExpression, builder.SourceType);
							}
						}
					}

					break;
				case "Take":
					Contract.Assume(methodCall.Arguments.Count >= 2);
					{
						var result = ProcessMethodCallInternal(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
						if (result != null)
						{
							return InvokeEager<T>(methodCall, result);
						}

						builder.TakeParameter = _writer.Write(methodCall.Arguments[1], builder.SourceType);
					}

					break;
				case "Skip":
					Contract.Assume(methodCall.Arguments.Count >= 2);
					{
						var result = ProcessMethodCallInternal(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);
						if (result != null)
						{
							return InvokeEager<T>(methodCall, result);
						}

						builder.SkipParameter = _writer.Write(methodCall.Arguments[1], builder.SourceType);
					}

					break;
				case "Expand":
					Contract.Assume(methodCall.Arguments.Count >= 2);

					builder.ExpandParameter = string.IsNullOrWhiteSpace(builder.ExpandParameter)
						? methodCall.Arguments[1].ToString()
						: string.Join(",", builder.ExpandParameter, methodCall.Arguments[1].ToString());
					break;
				default:
					return ExecuteMethod(methodCall, builder, resultLoader, intermediateResultLoader);
			}

			return null;
		}

		private IObservable<object> GetMethodResult<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IObservable<IEnumerable<T>>> resultLoader, Func<Type, ParameterBuilder, IObservable<IEnumerable>> intermediateResultLoader)
		{
			Contract.Requires(methodCall != null);
			Contract.Requires(builder != null);
			Contract.Requires(resultLoader != null);
			Contract.Assume(methodCall.Arguments.Count >= 2);

			ProcessMethodCallInternal(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);

			var processResult = _writer.Write(methodCall.Arguments[1], builder.SourceType);
			var currentParameter = string.IsNullOrWhiteSpace(builder.FilterParameter)
									? processResult
									: string.Format("({0}) and ({1})", builder.FilterParameter, processResult);
			builder.FilterParameter = currentParameter;

			var genericArguments = methodCall.Method.GetGenericArguments();
#if !NETFX_CORE
			var method = typeof(Queryable)
				.GetMethods()
				.Single(x => string.Equals(x.Name, methodCall.Method.Name) && x.GetParameters().Length == 1)
				.MakeGenericMethod(genericArguments);
#else
			var method = typeof(Queryable).GetTypeInfo()
				.GetDeclaredMethods(methodCall.Method.Name)
				.Single(x => x.GetParameters().Length == 1)
				.MakeGenericMethod(genericArguments);
#endif

			return resultLoader(builder)
				.Select<IEnumerable<T>, object>(
							  list =>
							  {
								  Contract.Assume(list != null);

								  var qbservable = list.ToObservable().AsQbservable();
								  var parameters = new object[] { qbservable };

								  return method.Invoke(null, parameters);
							  });
		}

		private IObservable<object> GetResult<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IObservable<IEnumerable<T>>> resultLoader, Func<Type, ParameterBuilder, IObservable<IEnumerable>> intermediateResultLoader)
		{
			Contract.Requires(methodCall != null);
			Contract.Requires(builder != null);
			Contract.Requires(resultLoader != null);
			Contract.Assume(methodCall.Arguments.Count >= 1);

			ProcessMethodCallInternal(methodCall.Arguments[0] as MethodCallExpression, builder, resultLoader, intermediateResultLoader);

			return resultLoader(builder)
				.Select(
							  list =>
							  {
								  Contract.Assume(!ReferenceEquals(list, null));

								  var parameters = ResolveInvocationParameters(list, typeof(T), methodCall);
								  return methodCall.Method.Invoke(null, parameters);
							  });
		}

		private IObservable<object> ExecuteMethod<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IObservable<IEnumerable<T>>> resultLoader, Func<Type, ParameterBuilder, IObservable<IEnumerable>> intermediateResultLoader)
		{
			Contract.Requires(methodCall != null);
			Contract.Requires(resultLoader != null);
			Contract.Requires(intermediateResultLoader != null);
			Contract.Requires(builder != null);
			Contract.Assume(methodCall.Arguments.Count >= 2);

			var innerMethod = methodCall.Arguments[0] as MethodCallExpression;

			if (innerMethod == null)
			{
				return null;
			}

			var result = ProcessMethodCallInternal(innerMethod, builder, resultLoader, intermediateResultLoader);
			if (result != null)
			{
				return InvokeEager<T>(innerMethod, result);
			}

#if !NETFX_CORE
			var genericArgument = innerMethod.Method.ReturnType.GetGenericArguments()[0];
#else
			var genericArgument = innerMethod.Method.ReturnType.GenericTypeArguments[0];
#endif
			var type = typeof(T);

			var methodInfo = methodCall.Method;

			var observable = type != genericArgument
						? intermediateResultLoader(genericArgument, builder)
							.Select(
										  resultList =>
										  {
											  var arguments = ResolveInvocationParameters(resultList, genericArgument, methodCall);
											  var methodResult = methodInfo.Invoke(null, arguments);

											  return methodResult;
										  })
						: resultLoader(builder)
							.Select(
										  resultList =>
										  {
											  var arguments = ResolveInvocationParameters(resultList, genericArgument, methodCall);
											  return methodInfo.Invoke(null, arguments);
										  });

			return observable;
		}

		private void ResolveProjection(ParameterBuilder builder, LambdaExpression lambdaExpression, Type sourceType)
		{
			Contract.Requires(lambdaExpression != null);

			var selectFunction = lambdaExpression.Body as NewExpression;

			if (selectFunction != null)
			{
#if !NETFX_CORE
				var properties = sourceType.GetProperties();
#else
				var properties = sourceType.GetTypeInfo().DeclaredProperties;
#endif
				var members = selectFunction.Members
					.Select(x => properties.FirstOrDefault(y => y.Name == x.Name) ?? x)
										 .Select(x => _memberNameResolver.ResolveName(x))
										 .ToArray();
				var args = selectFunction.Arguments.OfType<MemberExpression>()
										 .Select(x => properties.FirstOrDefault(y => y.Name == x.Member.Name) ?? x.Member)
										 .Select(x => _memberNameResolver.ResolveName(x))
										 .ToArray();
				if (members.Intersect(args).Count() != members.Length)
				{
					throw new InvalidOperationException("Projection into new member names is not supported.");
				}

				builder.SelectParameter = string.Join(",", args);
			}

			var propertyExpression = lambdaExpression.Body as MemberExpression;
			if (propertyExpression != null)
			{
				builder.SelectParameter = string.IsNullOrWhiteSpace(builder.SelectParameter)
					? _memberNameResolver.ResolveName(propertyExpression.Member)
					: builder.SelectParameter + "," + _memberNameResolver.ResolveName(propertyExpression.Member);
			}
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_writer != null);
		}
	}
}