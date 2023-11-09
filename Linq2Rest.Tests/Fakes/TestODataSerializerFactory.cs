// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestODataSerializerFactory.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestODataSerializerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Tests.Fakes
{
    using LinqCovertTools.Provider;

    public class TestODataSerializerFactory : ISerializerFactory
    {
        public ISerializer<T> Create<T>()
        {
            return new TestODataSerializer<T>();
        }

        public ISerializer<T> Create<T, TSource>()
        {
            return Create<T>();
        }
    }
}