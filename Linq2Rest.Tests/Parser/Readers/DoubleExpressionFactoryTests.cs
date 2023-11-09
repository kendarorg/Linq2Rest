// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleExpressionFactoryTests.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DoubleExpressionFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Tests.Parser.Readers
{
    using LinqCovertTools.Parser.Readers;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class DoubleExpressionFactoryTests
    {
        private DoubleExpressionFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new DoubleExpressionFactory();
        }

        [Test]
        public void WhenFilterIncludesDoubleParameterThenReturnedExpressionContainsDouble()
        {
            var expression = _factory.Convert("1.23");

            Assert.IsAssignableFrom<double>(expression.Value);
        }

        [Test]
        public void WhenFilterIncludesDoubleParameterWithTrailingLowerCaseMThenReturnedExpressionContainsDouble()
        {
            var expression = _factory.Convert("1.23d");

            Assert.IsAssignableFrom<double>(expression.Value);
        }

        [Test]
        public void WhenFilterIncludesDoubleParameterWithTrailingUpperCaseMThenReturnedExpressionContainsDouble()
        {
            var expression = _factory.Convert("1.23D");

            Assert.IsAssignableFrom<double>(expression.Value);
        }

        [Test]
        public void WhenFilterIsIncorrectFormatThenThrows()
        {
            const string Parameter = "blah";

            Assert.Throws<FormatException>(() => _factory.Convert(Parameter));
        }
    }
}