// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteralObservableExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the InteralObservableExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive
{
	using System;
	using System.Collections;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Reactive.Linq;
	using System.Reflection;

	internal static class InternalObservableExtensions
	{
		private static readonly MethodInfo InnerToObservableMethod;
		private static readonly MethodInfo InnerToQbservableMethod;

		static InternalObservableExtensions()
		{
#if !NETFX_CORE
			var qbservableMethods = typeof(Qbservable).GetMethods(BindingFlags.Static | BindingFlags.Public);
			var observableMethods = typeof(Observable).GetMethods(BindingFlags.Static | BindingFlags.Public);
#else
			var qbservableMethods = typeof(Qbservable).GetTypeInfo().GetDeclaredMethods("AsQbservable").ToArray();
			var observableMethods = typeof(Observable).GetTypeInfo().GetDeclaredMethods("ToObservable").ToArray();
#endif
			
			Contract.Assume(qbservableMethods.Length > 0);
			Contract.Assume(observableMethods.Length > 0);

			InnerToObservableMethod = observableMethods
					.First(x => x.Name == "ToObservable" && x.GetParameters().Length == 1);
			InnerToQbservableMethod = qbservableMethods
					.First(x => x.Name == "AsQbservable" && x.GetParameters().Length == 1);
		}

		public static object ToQbservable(this IEnumerable enumerable, Type type)
		{
			Contract.Requires(enumerable != null);
			Contract.Requires(type != null);

			var genericObservableMethod = InnerToObservableMethod.MakeGenericMethod(type);
			var genericQbservableMethod = InnerToQbservableMethod.MakeGenericMethod(type);

			var observable = genericObservableMethod.Invoke(null, new object[] { enumerable });
			var qbservable = genericQbservableMethod.Invoke(null, new[] { observable });

			return qbservable;
		}
	}
}