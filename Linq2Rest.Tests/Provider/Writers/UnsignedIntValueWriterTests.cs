// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsignedIntValueWriterTests.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnsignedIntValueWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Tests.Provider.Writers
{
    using LinqCovertTools.Provider.Writers;
    using NUnit.Framework;

    [TestFixture]
    public class UnsignedIntValueWriterTests
    {
        private UnsignedIntValueWriter _writer;

        [SetUp]
        public void Setup()
        {
            _writer = new UnsignedIntValueWriter();
        }

        [Test]
        public void WhenWritingUnsignedIntValueThenWritesString()
        {
            var result = _writer.Write((uint)123);

            Assert.AreEqual("123", result);
        }
    }
}