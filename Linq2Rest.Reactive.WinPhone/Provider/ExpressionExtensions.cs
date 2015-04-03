// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExpressionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.Provider
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Reflection;

	internal static class ExpressionExtensions
	{
		public static Tuple<Type, string> GetNameFromAlias(this IMemberNameResolver memberNameResolver, MemberInfo alias, Type sourceType)
		{
			Contract.Requires(sourceType != null);
			Contract.Requires(alias != null);
			Contract.Ensures(Contract.Result<Tuple<Type, string>>() != null);

#if !NETFX_CORE
			var source = sourceType.GetMembers()
#else
			var source = sourceType.GetTypeInfo().DeclaredMembers
#endif
				.Select(x => new { Original = x, Name = memberNameResolver.ResolveName(x) })
				.FirstOrDefault(x => x.Name == alias.Name);

			return source != null
					   ? new Tuple<Type, string>(GetMemberType(source.Original), source.Name)
					   : new Tuple<Type, string>(GetMemberType(alias), alias.Name);
		}

		private static Type GetMemberType(MemberInfo member)
		{
#if !NETFX_CORE
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
				case MemberTypes.Method:
					return ((MethodInfo)member).ReturnType;
				default:
					throw new InvalidOperationException(member.MemberType + " is not resolvable");
			}
#else
			var fieldInfo = member as FieldInfo;
			if (fieldInfo != null)
			{
				return fieldInfo.FieldType;
			}
			var methodInfo = member as MethodInfo;
			if (methodInfo != null)
			{
				return methodInfo.ReturnType;
			}
			var propertyInfo = member as PropertyInfo;
			if (propertyInfo != null)
			{
				return propertyInfo.PropertyType;
			}
			throw new InvalidOperationException(member + " is not resolvable");
#endif
		}
	}
}
