// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines public extension methods on <see cref="IObservable{T}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

#if NETFX_CORE
	using System.Runtime.CompilerServices;
#endif

	/// <summary>
	/// Defines public extension methods on <see cref="IObservable{T}"/>
	/// </summary>
	public static class ObservableExtensions
	{
		/// <summary>
		/// Creates an <see cref="IObservable{T}"/> over a POST request.
		/// </summary>
		/// <param name="source">The source <see cref="IObservable{T}"/>.</param>
		/// <param name="input">A <see cref="Func{TInput}"/> to generate POST data.</param>
		/// <typeparam name="T">The <see cref="Type"/> of item in the <see cref="IObservable{T}"/>.</typeparam>
		/// <typeparam name="TInput">The <see cref="Type"/> of item to POST to the server.</typeparam>
		/// <returns>An <see cref="IObservable{T}"/> instance.</returns>
		public static IObservable<T> Post<T, TInput>(this IObservable<T> source, Func<TInput> input)
		{
			var restObservable = source as InnerRestObservableBase<T, T>;
			if (restObservable != null)
			{
				restObservable.ChangeMethod(HttpMethod.Post);
				var serializer = restObservable.SerializerFactory.Create<TInput>();
				var inputData = input();
				if (ReferenceEquals(inputData, null))
				{
					throw new NullReferenceException();
				}

				var serialized = serializer.Serialize(inputData);
				restObservable.SetInput(serialized);
			}

			return source;
		}

		/// <summary>
		/// Creates an <see cref="IObservable{T}"/> over a PUT request.
		/// </summary>
		/// <param name="source">The source <see cref="IObservable{T}"/>.</param>
		/// <param name="input">A <see cref="Func{TInput}"/> to generate PUT data.</param>
		/// <typeparam name="T">The <see cref="Type"/> of item in the <see cref="IObservable{T}"/>.</typeparam>
		/// <typeparam name="TInput">The <see cref="Type"/> of item to PUT on the server.</typeparam>
		/// <returns>An <see cref="IObservable{T}"/> instance.</returns>
		public static IObservable<T> Put<T, TInput>(this IObservable<T> source, Func<TInput> input)
		{
			var restObservable = source as InnerRestObservableBase<T, T>;
			if (restObservable != null)
			{
				restObservable.ChangeMethod(HttpMethod.Put);
				var serializer = restObservable.SerializerFactory.Create<TInput>();
				var inputData = input();
				if (ReferenceEquals(inputData, null))
				{
					throw new NullReferenceException();
				}

				var serialized = serializer.Serialize(inputData);
				restObservable.SetInput(serialized);
			}

			return source;
		}

		/// <summary>
		/// Creates an <see cref="IObservable{T}"/> over a DELETE request.
		/// </summary>
		/// <param name="source">The source <see cref="IObservable{T}"/>.</param>
		/// <typeparam name="T">The <see cref="Type"/> of item in the <see cref="IObservable{T}"/>.</typeparam>
		/// <returns>An <see cref="IObservable{T}"/> instance.</returns>
		public static IObservable<T> Delete<T>(this IObservable<T> source)
		{
			var restObservable = source as InnerRestObservableBase<T, T>;
			if (restObservable != null)
			{
				restObservable.ChangeMethod(HttpMethod.Delete);
			}

			return source;
		}

		/// <summary>
		/// Expands the specified source.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TAlias">The <see cref="Type"/> to derive aliases from.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="paths">The paths to expand in the format "Child1, Child2/GrandChild2".</param>
		/// <returns>An <see cref="IObservable{T}"/> for continued querying.</returns>
		public static IObservable<TSource> Expand<TSource, TAlias>(this IObservable<TSource> source, string paths)
		{
			Contract.Requires<ArgumentNullException>(source != null);

			var restObservable = source as InnerRestObservable<TSource, TAlias>;
			if (restObservable == null)
			{
				return source;
			}

#if !NETFX_CORE
			var genericMethod = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new[] { typeof(TSource), typeof(TAlias) });
#else
			var genericMethod = CreateGenericMethod<TSource>();
#endif
			return restObservable.Provider.CreateQuery<TSource>(
					Expression.Call(
						null, 
						genericMethod, 
						new[] { restObservable.Expression, Expression.Constant(paths) }));
		}

		/// <summary>
		/// Expands the specified source.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TAlias">The <see cref="Type"/> to derive aliases from.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="properties">The paths to expand.</param>
		/// <returns>An <see cref="IObservable{T}"/> for continued querying.</returns>
		public static IObservable<TSource> Expand<TSource, TAlias>(this IObservable<TSource> source, params Expression<Func<TAlias, object>>[] properties)
		{
			Contract.Requires<ArgumentNullException>(source != null);
			Contract.Assume(properties != null);

			var propertyNames = string.Join(",", properties.Where(x => x != null).Select(ResolvePropertyName));

			return Expand<TSource, TAlias>(source, propertyNames);
		}

		private static string ResolvePropertyName<TSource>(Expression<Func<TSource, object>> property)
		{
			Contract.Requires(property != null);

			var pathPrefixes = new List<string>();

			var body = property.Body;
			if (body.NodeType == ExpressionType.Convert)
			{
				body = ((UnaryExpression)body).Operand;
			}

			var currentMemberExpression = body as MemberExpression;
			while (currentMemberExpression != null)
			{
				pathPrefixes.Add(currentMemberExpression.Member.Name);
				currentMemberExpression = currentMemberExpression.Expression as MemberExpression;
			}

			pathPrefixes.Reverse();
			return string.Join("/", pathPrefixes);
		}

#if NETFX_CORE
		private static MethodInfo CreateGenericMethod<TSource>([CallerMemberName] string callerMemberName = "")
		{
			return typeof(ObservableExtensions)
				.GetTypeInfo()
				.GetDeclaredMethod(callerMemberName)
				.MakeGenericMethod(typeof(TSource));
		}
#endif
	}
}