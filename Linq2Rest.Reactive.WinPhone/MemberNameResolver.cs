// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberNameResolver.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberNameResolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	internal class MemberNameResolver : IMemberNameResolver
	{
		private static readonly object LockObject = new object();
		private static readonly Dictionary<MemberInfo, string> KnownMemberNames = new Dictionary<MemberInfo, string>();
		private static readonly Dictionary<string, MemberInfo> KnownAliasNames = new Dictionary<string, MemberInfo>();

		public string ResolveName(Type type, string alias)
		{
			var members = type.GetMembers();
			var name = members.Select(ResolveName).FirstOrDefault(x => x == alias);

			return name;
		}

		public MemberInfo ResolveAlias(Type type, string alias)
		{
			var key = type.AssemblyQualifiedName + alias;
			if (!KnownAliasNames.ContainsKey(key))
			{
				lock (LockObject)
				{
					return KnownAliasNames[key] = ResolveAliasInternal(type, alias);
				}
			}

			return KnownAliasNames[key];
		}

		public string ResolveName(MemberInfo member)
		{
			if (!KnownMemberNames.ContainsKey(member))
			{
				KnownMemberNames[member] = ResolveNameInternal(member);
			}

			var result = KnownMemberNames[member];

			Contract.Assume(result != null);

			return result;
		}

		private static MemberInfo ResolveAliasInternal(Type type, string alias)
		{
			var member = GetMembers(type)
				.Select(
					x =>
					{
						var attributes = x.GetCustomAttributes(true);
						var dataMember = attributes.OfType<DataMemberAttribute>()
							.FirstOrDefault();
						if (dataMember != null && dataMember.Name == alias)
						{
							return x;
						}

						var xmlElement = attributes.OfType<XmlElementAttribute>()
							.FirstOrDefault();
						if (xmlElement != null && xmlElement.ElementName == alias)
						{
							return x;
						}

						var xmlAttribute = attributes.OfType<XmlAttributeAttribute>()
							.FirstOrDefault();
						if (xmlAttribute != null && xmlAttribute.AttributeName == alias)
						{
							return x;
						}

						if (x.Name == alias)
						{
							return x;
						}

						return null;
					})
				.FirstOrDefault(x => x != null);

			return member;
		}

		private static IEnumerable<MemberInfo> GetMembers(Type type)
		{
#if NETFX_CORE
			var typeInfo = type.GetTypeInfo();
			if (typeInfo.IsInterface)
			{
				var propertyInfos = new List<MemberInfo>();

				var considered = new List<Type>();
				var queue = new Queue<TypeInfo>();
				considered.Add(type);
				queue.Enqueue(typeInfo);
				while (queue.Count > 0)
				{
					var subType = queue.Dequeue();
					foreach (var subInterface in subType.ImplementedInterfaces.Where(x => !considered.Contains(x)))
					{
						considered.Add(subInterface);
						queue.Enqueue(subInterface.GetTypeInfo());
					}

					var typeProperties = subType.DeclaredMembers;

					var newPropertyInfos = typeProperties
						.Where(x => !propertyInfos.Contains(x));

					propertyInfos.InsertRange(0, newPropertyInfos);
				}

				return propertyInfos.ToArray();
			}

			return type.GetTypeInfo().DeclaredMembers;
#else
			if (type.IsInterface)
			{
				var propertyInfos = new List<MemberInfo>();

				var considered = new List<Type>();
				var queue = new Queue<Type>();
				considered.Add(type);
				queue.Enqueue(type);
				while (queue.Count > 0)
				{
					var subType = queue.Dequeue();
					foreach (var subInterface in subType.GetInterfaces()
						.Where(x => !considered.Contains(x)))
					{
						considered.Add(subInterface);
						queue.Enqueue(subInterface);
					}

					var typeProperties = subType.GetMembers(
						BindingFlags.FlattenHierarchy
						| BindingFlags.Public
						| BindingFlags.Instance);

					var newPropertyInfos = typeProperties
						.Where(x => !propertyInfos.Contains(x));

					propertyInfos.InsertRange(0, newPropertyInfos);
				}

				return propertyInfos.ToArray();
			}

			return type.GetMembers(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
		}

		private static string ResolveNameInternal(MemberInfo member)
		{
			Contract.Requires(member != null);

			var dataMember = member.GetCustomAttributes(typeof(DataMemberAttribute), true)
				.OfType<DataMemberAttribute>()
				.FirstOrDefault();

			if (dataMember != null && dataMember.Name != null)
			{
				return dataMember.Name;
			}

			var xmlElement = member.GetCustomAttributes(typeof(XmlElementAttribute), true)
				.OfType<XmlElementAttribute>()
				.FirstOrDefault();

			if (xmlElement != null && xmlElement.ElementName != null)
			{
				return xmlElement.ElementName;
			}

			var xmlAttribute = member.GetCustomAttributes(typeof(XmlAttributeAttribute), true)
				.OfType<XmlAttributeAttribute>()
				.FirstOrDefault();

			if (xmlAttribute != null && xmlAttribute.AttributeName != null)
			{
				return xmlAttribute.AttributeName;
			}

			Contract.Assert(member.Name != null, "Member must have name");
			return member.Name;
		}
	}
}