﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelFilterExtensionsTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ModelFilterExtensionsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Tests
{
    using LinqCovertTools.Tests.Fakes;
    using NUnit.Framework;
    using System;
    using System.Collections.Specialized;
    using System.Linq;

    [TestFixture]
    public class ModelFilterExtensionsTests
    {
        private FakeItem[] _source;

        [SetUp]
        public void TestSetup()
        {
            _source = new[]
                          {
                              new FakeItem
                                  {
                                      DoubleValue = 123d
                                  }
                          };
        }

        [TestCase("DoubleValue gt blah")]
        [TestCase("DateValue eq 123")]
        [TestCase("DateValue eq datetime'123'")]
        public void WhenFilteringWithInvalidFilterParametersThenThrows(string filter)
        {
            var parameters = new NameValueCollection
                                 {
                                     { "$filter", filter }
                                 };

            Assert.Throws<FormatException>(() => _source.Filter(parameters));
        }

        [TestCase("DoubleValues")]
        [TestCase("Date")]
        [TestCase("'123'")]
        public void WhenFilteringWithInvalidOrderingParametersThenThrows(string sorting)
        {
            var parameters = new NameValueCollection
                                 {
                                     { "$orderby", sorting }
                                 };

            Assert.Throws<FormatException>(() => _source.Filter(parameters));
        }

        [TestCase("DoubleValues")]
        [TestCase("x")]
        [TestCase("'123'")]
        public void WhenFilteringWithInvalidTopParametersThenThrows(string top)
        {
            var parameters = new NameValueCollection
                                 {
                                     { "$top", top }
                                 };

            Assert.Throws<FormatException>(() => _source.Filter(parameters));
        }

        [TestCase("DoubleValues")]
        [TestCase("x")]
        [TestCase("'123'")]
        public void WhenFilteringWithInvalidSkipParametersThenThrows(string top)
        {
            var parameters = new NameValueCollection
                                 {
                                     { "$skip", top }
                                 };

            Assert.Throws<FormatException>(() => _source.Filter(parameters));
        }

        [Test]
        public void WhenFilteringOnInterfaceThenReturnsResults()
        {
            var points = new object[]
                           {
                               new DataPoint { Flags = new QualityFlags(100) }
                           };
            var nc = new NameValueCollection { { "$filter", "Flags/Value gt 64" } };

            CollectionAssert.IsNotEmpty(points.Cast<IDataPoint>().Filter(nc));
        }

        [Test]
        public void WhenFilteringOnConcreteTypeThenReturnsResults()
        {
            var points = new object[]
                           {
                               new DataPoint { Flags = new QualityFlags(100) }
                           };
            var nc = new NameValueCollection { { "$filter", "Flags/Value gt 64" } };

            CollectionAssert.IsNotEmpty(points.Cast<DataPoint>().Filter(nc));
        }
    }
}
