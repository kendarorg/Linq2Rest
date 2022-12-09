// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsignedShortValueWriterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnsignedShortValueWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Tests.Provider.Writers
{
    using LinqCovertTools.Provider.Writers;
    using NUnit.Framework;

    [TestFixture]
    public class UnsignedShortValueWriterTests
    {
        private UnsignedShortValueWriter _writer;

        [SetUp]
        public void Setup()
        {
            _writer = new UnsignedShortValueWriter();
        }

        [Test]
        public void WhenWritingUnsignedShortValueThenWritesString()
        {
            var result = _writer.Write((ushort)123);

            Assert.AreEqual("123", result);
        }
    }
}