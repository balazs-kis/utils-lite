using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestLite;
using UtilsLite.Mapping;

namespace UtilsLite.Tests.Mapping
{
    [TestClass]
    public class GenericMapperTests
    {
        public class MyFirstClass
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public double SuccessRate { get; set; }
            public DateTime Expiration { get; set; }
        }

        public class MySecondClass
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public double SuccessRate { get; set; }
            public DateTime Expiration { get; set; }
        }


        [TestMethod]
        public void MapsCorrectlyByPropertyNames() => Test
            .Arrange(() =>
            {
                var from = new MyFirstClass
                {
                    Name = "Item",
                    Age = 2,
                    SuccessRate = 0.85,
                    Expiration = new DateTime(2018, 12, 31)
                };

                var mapper = new GenericMapper<MyFirstClass, MySecondClass>();
                return (from, mapper);
            })
            .Act((from, mapper) => (mapped: mapper.MapByPropertyName(from), from))
            .Assert()
                .Validate(result => result.mapped.Name.Should().Be(result.from.Name))
                .Validate(result => result.mapped.Age.Should().Be(result.from.Age))
                .Validate(result => result.mapped.SuccessRate.Should().Be(result.from.SuccessRate))
                .Validate(result => result.mapped.Expiration.Should().Be(result.from.Expiration));
    }
}