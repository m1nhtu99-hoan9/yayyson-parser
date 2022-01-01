using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;
using static YayysonParser.Features;

namespace YayysonParser.Tests.Features
{
    public class ParseAndCastToDateTimeTest
    {
        [Fact]
        public void GivenMinValueDateTimeExpression_ReturnsMinDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.MinValue}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(DateTime.MinValue, actualResult.ResultValue);
        }

        [Fact]
        public void GivenMaxValueDateTimeExpression_ReturnsMaxDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.MaxValue}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(DateTime.MaxValue, actualResult.ResultValue);
        }

        [Fact]
        public void GivenNowDateTimeExpression_ReturnsCurrentDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.Now}");

            Assert.True(actualResult.IsOk);
            actualResult.ResultValue.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void GivenUtcNowDateTimeExpression_ReturnsCurrentUtcDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.UtcNow}");

            Assert.True(actualResult.IsOk);
            actualResult.ResultValue.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [MemberData(nameof(ValidYayysonExprs))]
        public void GivenBinaryExpressionReducedToDateTime_ReturnsAppropriateDateTime(string expr, DateTime expected)
        {
            var actualResult = ParseAndCastToDateTime(expr);

            Assert.True(actualResult.IsOk);
            actualResult.ResultValue.Should().BeCloseTo(expected, TimeSpan.FromSeconds(20));
        }

        [Theory]
        [MemberData(nameof(InvalidYayysonExprs))]
        public void GivenBinaryExpressionNotReducedToDateTime_ReturnError(string expr, string expectedMsg)
        {
            try
            {
                var actualResult = ParseAndCastToDateTime(expr);

                Assert.True(actualResult.IsError);
                actualResult.ErrorValue.Should().Equals(expectedMsg);
            }
            catch (Exception exn)
            {
                throw new XunitException($"Expected no exception, but found: {exn}");
            }
        }

        public static IEnumerable<object[]> ValidYayysonExprs = new[]
        {
            ("DateTime.MinValue + 89.9095d", DateTime.MinValue.Add(TimeSpan.FromDays(89.9095))),
            ("DateTime.Now + 30d_03h", DateTime.Now.Add(TimeSpan.FromDays(30) + TimeSpan.FromHours(3))),
            ("DateTime.UtcNow + 20.25d_62.125h_83m99.05s", DateTime.UtcNow.Add(
                TimeSpan.FromDays(20.25)
                + TimeSpan.FromHours(62.125)
                + TimeSpan.FromMinutes(83)
                + TimeSpan.FromSeconds(99.05))),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();

        public static IEnumerable<object[]> InvalidYayysonExprs = new[]
        {
            ("2.0 + 3", null),
            ("DateTime.Now + 2.022", "Unsupported operation: Addition of DateTimeLiteral to FloatLiteral."),
            ("30d_03h + DateTime.UtcNow", "Invalid operation: Addition of TimeSpan to DateTime. Try swapping the operands?"),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
