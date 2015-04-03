// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncExpressionProcessor.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the inteface for an expression processor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;
	using Linq2Rest.Provider;

	/// <summary>
	/// Defines the inteface for an expression processor.
	/// </summary>
	[ContractClass(typeof(AsyncExpressionProcessorContracts))]
	internal interface IAsyncExpressionProcessor
	{
		/// <summary>
		/// Processes the passed <see cref="MethodCallExpression"/> and returns the result as an observable.
		/// </summary>
		/// <typeparam name="T">The generic <see cref="Type"/> of object returned in the <see cref="IObservable{T}"/>.</typeparam>
		/// <param name="methodCall">The <see cref="MethodCallExpression"/> to process.</param>
		/// <param name="builder">The <see cref="ParameterBuilder"/> used to store values.</param>
		/// <param name="resultLoader">The result loader function.</param>
		/// <param name="intermediateResultLoader">The intermediate result loader function.</param>
		/// <returns>An <see cref="IObservable{T}"/> sequence with the result of the method call.</returns>
		IObservable<T> ProcessMethodCall<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IObservable<IEnumerable<T>>> resultLoader, Func<Type, ParameterBuilder, IObservable<IEnumerable>> intermediateResultLoader);
	}
	
	[ContractClassFor(typeof(IAsyncExpressionProcessor))]
	internal abstract class AsyncExpressionProcessorContracts : IAsyncExpressionProcessor
	{
		public IObservable<T> ProcessMethodCall<T>(MethodCallExpression methodCall, ParameterBuilder builder, Func<ParameterBuilder, IObservable<IEnumerable<T>>> resultLoader, Func<Type, ParameterBuilder, IObservable<IEnumerable>> intermediateResultLoader)
		{
			Contract.Requires(builder != null);
			Contract.Requires(resultLoader != null);
			Contract.Requires(intermediateResultLoader != null);

			throw new NotImplementedException();
		}
	}
}