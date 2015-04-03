// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReflectionHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive
{
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Reflection;
	using Linq2Rest.Provider;

	internal static class ReflectionHelper
	{
#if !NETFX_CORE
		private readonly static MethodInfo CreateMethodInfo = typeof(ISerializerFactory).GetMethods().First(x => x.Name == "Create" && x.GetGenericArguments().Length == 1).GetGenericMethodDefinition();
		private readonly static MethodInfo AliasCreateMethodInfo = typeof(ISerializerFactory).GetMethods().First(x => x.Name == "Create" && x.GetGenericArguments().Length == 2).GetGenericMethodDefinition();
		private static readonly MethodInfo InnerCreateMethod = typeof(ISerializerFactory).GetMethods().First(x => x.Name == "Create" && x.GetGenericArguments().Length == 1);
#else
		private readonly static MethodInfo CreateMethodInfo = typeof(ISerializerFactory).GetTypeInfo().DeclaredMethods.First(x => x.Name == "Create" && x.GetGenericArguments().Length == 1).GetGenericMethodDefinition();
		private readonly static MethodInfo AliasCreateMethodInfo = typeof(ISerializerFactory).GetTypeInfo().DeclaredMethods.First(x => x.Name == "Create" && x.GetGenericArguments().Length == 2).GetGenericMethodDefinition();

		private static readonly MethodInfo InnerCreateMethod =
	        typeof(ISerializerFactory)
            .GetTypeInfo()
            .GetDeclaredMethod("Create");
#endif

		public static MethodInfo GenericCreateMethod
		{
			get
			{
#if !NETFX_CORE
				Contract.Ensures(Contract.Result<MethodInfo>() != null);
#endif

				return CreateMethodInfo;
			}
		}

		public static MethodInfo AliasGenericCreateMethod
		{
			get
			{
#if !NETFX_CORE
				Contract.Ensures(Contract.Result<MethodInfo>() != null);
#endif

				return AliasCreateMethodInfo;
			}
		}

		public static MethodInfo CreateMethod
		{
			get
			{
#if !NETFX_CORE
				Contract.Ensures(Contract.Result<MethodInfo>() != null);
#endif

				return InnerCreateMethod;
			}
		}
	}
}