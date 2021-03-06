// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ODataSerializerFactory.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ODataSerializerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.WP8.Sample.Support
{
	using Linq2Rest.Provider;

	public class ODataSerializerFactory : ISerializerFactory
	{
		public ISerializer<T> Create<T>()
		{
			return new ODataSerializer<T>();
		}

		public ISerializer<T> Create<T, TSource>()
		{
			return Create<T>();
		}
	}
}