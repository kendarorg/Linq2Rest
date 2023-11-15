// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ODataExpressionWriterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ODataExpressionWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Tests
{
    using LinqCovertTools.Tests.Provider;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    [TestFixture]
    public class ODataExpressionWriterTests
    {
        [Test]
        public void CanFilterOnSubCollection()
        {
            var converter = new ODataExpressionConverter();
            Expression<Func<FakeItem, bool>> expression = x => x.Child.Attributes.Any(y => y == "blah");

            var serialized = converter.Convert(expression);

            Assert.AreEqual("Child/Attributes/any(y: y eq 'blah')", serialized);
        }

        [Test]
        public void CanSerializeEmptyGuid()
        {
            var converter = new ODataExpressionConverter();
            Expression<Func<ChildDto, bool>> expression = x => x.GlobalID != Guid.Empty;

            var serialized = converter.Convert(expression);

            Assert.AreEqual("GlobalID ne guid'00000000-0000-0000-0000-000000000000'", serialized);
        }

        [Theory]
        [TestCase("givenName eq 'john'")]
        [TestCase("startswith(givenName, 'jo')")]
        [TestCase("endswith(givenName, 'Hn')")]
        [TestCase("indexof(givenName, 'j') eq 0")]
        [TestCase("substring(givenName, 0) eq 'john'")]
        [TestCase("substringof('OH', givenName)")]
        public void CollectionShouldBeEmptyWhenCaseIsNotIgnored(string query)
        {
            // Arrange
            ODataExpressionConverter converter = new();
            List<User> users = new()
            {
                new User()
                {
                    GivenName = "John"
                }
            };


            // Act
            var predicate = converter.Convert<User>(query, false).Compile();
            var results = users.Where(predicate);

            // Assert
            Assert.IsEmpty(results);
        }

        [Theory]
        [TestCase("givenName eq 'john'")]
        [TestCase("startswith(givenName, 'jo')")]
        [TestCase("endswith(givenName, 'Hn')")]
        [TestCase("indexof(givenName, 'j') eq 0")]
        [TestCase("substring(givenName, 0) eq 'john'")]
        [TestCase("substringof('OH', givenName)")]
        public void CollectionShouldNotBeEmptyWhenCaseIsIgnored(string query)
        {
            // Arrange
            ODataExpressionConverter converter = new();
            List<User> users = new()
            {
                new User()
                {
                    GivenName = "John"
                }
            };


            // Act
            var predicate = converter.Convert<User>(query, true).Compile();
            var results = users.Where(predicate);

            // Assert
            Assert.IsNotEmpty(results);
        }

        [Theory]
        [TestCase("indexof(GivenName, 'test') eq 0")]
        [TestCase("GivenName eq 'test'")]
        [TestCase("GivenName ne null")]
        [TestCase("startswith(GivenName, 'test')")]
        [TestCase("endswith(GivenName, 'test')")]
        [TestCase("length(GivenName) gt 1")]
        [TestCase("tolower(GivenName) eq 'test'")]
        [TestCase("toupper(GivenName) eq 'test'")]
        [TestCase("trim(GivenName) eq 'test'")]
        public void QueryReturnsEmptySetWhenProviderValueIsNull(string query)
        {
            // Arrange
            ODataExpressionConverter converter = new();
            List<User> users = new()
            {
                new()
                {
                    GivenName = null,
                    FamilyName = "Parks"
                }
            };

            // Act
            Func<User, bool> converted = converter.Convert<User>(query).Compile();

            // Assert
            Assert.IsEmpty(users.Where(converted));
        }

        [Test]
        public void ConvertsExpressionToString()
        {
            var converter = new ODataExpressionConverter();
            Expression<Func<ChildDto, bool>> expression = x => x.Name == "blah";

            var serialized = converter.Convert(expression);

            Assert.AreEqual("Name eq 'blah'", serialized);
        }

        [Test]
        public void ConvertsExpressionToString2()
        {
            var converter = new ODataExpressionConverter();
            Expression<Func<ChildDto, bool>> expression = x => x.Name.Length + (1 + 1) == 7;

            var serialized = converter.Convert(expression);

            Assert.AreEqual("length(Name) add 2 eq 7", serialized);
        }

        [Test]
        public void ConvertsFilterToExpression()
        {
            const string Filter = "Name eq 'blah'";
            var converter = new ODataExpressionConverter();
            Expression<Func<ChildDto, bool>> expression = x => x.Name == "blah";

            var converted = converter.Convert<ChildDto>(Filter);

            Assert.AreEqual(converted.ToString(), expression.ToString());
        }

        [Test]
        public void ConvertsFilterToExpression2()
        {
            const string Filter = "(length(Name) add 2) eq 7";
            var converter = new ODataExpressionConverter();
            Expression<Func<ChildDto, bool>> expression = x => x.Name.Length + (1 + 1) == 7;

            var converted = converter.Convert<ChildDto>(Filter);

            Assert.AreEqual(expression.ToString(), converted.ToString());
        }

        [Test]
        public void ConvertsFilterToExpressionForInterface()
        {
            const string filter = "startswith(emailAddress, 'user@')";
            var converter = new ODataExpressionConverter();

            Expression<Func<IQueryableUser, bool>> converted = converter.Convert<IQueryableUser>(filter);

            List<User> users = new()
            {
                new User()
                { 
                    GivenName = "User",
                    FamilyName = "One",
                    EmailAddress = new EmailAddress("user@xyz.com")
                },
                new User()
                { 
                    GivenName = "User",
                    FamilyName = "Two",
                    EmailAddress = new EmailAddress("nonuser@xyz.com")
                }
            };

            IQueryable<User> query = users.AsQueryable();
            
            query = query.Where(converted).Cast<User>();

            Assert.AreEqual(1, query.Count());
        }

        [Test]
        public void ConvertsFilterToExpressionForExplicitInterfaceProps()
        {
            const string filter = "firstName eq 'Mr.'";
            var converter = new ODataExpressionConverter();

            Expression<Func<IQueryableUser, bool>> converted = converter.Convert<IQueryableUser>(filter);

            List<User> users = new()
            {
                new User()
                {
                    GivenName = "User",
                    FamilyName = "One",
                    EmailAddress = new EmailAddress("user@xyz.com")
                },
                new User()
                {
                    GivenName = "Mr.",
                    FamilyName = "Two",
                    EmailAddress = new EmailAddress("nonuser@xyz.com")
                }
            };

            IQueryable<User> query = users.AsQueryable();

            query = query.Where(converted).Cast<User>();

            Assert.AreEqual(1, query.Count());
        }

        [TestCase("start gt datetimeoffset'2023-07-01'")]
        [TestCase("start gt datetimeoffset'2023-07-01T08:00'")]
        public void CanReadShortDateTimeOffsetValues(string filter)
        {
            // Arrange
            ODataExpressionConverter converter = new ();
            Expression<Func<DateTimeObject, bool>> converted = converter.Convert<DateTimeObject>(filter);

            DateTimeObject[] objects = new DateTimeObject[]
            {
                new DateTimeObject{ Start = new DateTimeOffset(2023, 07, 04, 0, 0, 0, TimeSpan.FromHours(0)) }
            };

            // Act
            var filterted = objects.AsQueryable().Where(converted);

            // Assert
            Assert.AreEqual(1, filterted.Count());
        }

        private class EmailAddress
        {
            public EmailAddress(string value)
            {
                Value = value;
            }

            public string Value { get; set; }

            public string? Name { get; set; }
        }

        private interface IQueryableUser
        {
            string? EmailAddress { get; }

            string? FirstName { get; }
        }

        private class DateTimeObject
        {
            public DateTimeOffset? Start { get; set; }
        }

        private class User : IQueryableUser
        {
            public string GivenName { get; set; }

            public string FamilyName { get; set; }

            public ICollection<string> Roles { get; set; }

            public EmailAddress? EmailAddress { get; set; }

            string? IQueryableUser.EmailAddress => EmailAddress?.Value;

            string? IQueryableUser.FirstName => GivenName;
        }
    }
}
