// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringValueWriterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringValueWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Tests.Provider.Writers
{
    using LinqCovertTools.Provider.Writers;
    using NUnit.Framework;

    [TestFixture]
    public class StringValueWriterTests
    {
        private StringValueWriter _writer;

        [SetUp]
        public void Setup()
        {
            _writer = new StringValueWriter();
        }

        [Test]
        public void WhenWritingStringThenEnclosesInSingleQuote()
        {
            var result = _writer.Write("hello world");

            Assert.AreEqual("'hello world'", result);
        }
    }
}
