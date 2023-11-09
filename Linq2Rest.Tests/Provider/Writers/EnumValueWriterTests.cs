// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumValueWriterTests.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EnumValueWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Tests.Provider.Writers
{
    using LinqCovertTools.Provider.Writers;
    using NUnit.Framework;

    [TestFixture]
    public class EnumValueWriterTests
    {
        private EnumValueWriter _writer;

        [SetUp]
        public void Setup()
        {
            _writer = new EnumValueWriter();
        }

        [Test]
        public void WhenWritingByteValueThenWritesString()
        {
            const Choice Value = Choice.That;
            var result = _writer.Write(Value);

            Assert.AreEqual("LinqCovertTools.Tests.Choice'That'", result);
        }
    }
}