using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using static ParserExperiments.Features;

namespace ParserExperiments.Tests.Features
{
    public class ParseAndCastToDateTimeTest
    {
        [Fact]
        public void ParseAndCastToDateTime_GivenMinValueDateTimeExpression_ReturnsMinDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.MinValue}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(DateTime.MinValue, actualResult.ResultValue);
        }

        [Fact]
        public void ParseAndCastToDateTime_GivenMaxValueDateTimeExpression_ReturnsMaxDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.MaxValue}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(DateTime.MaxValue, actualResult.ResultValue);
        }

        [Fact]
        public void ParseAndCastToDateTime_GivenNowDateTimeExpression_ReturnsCurrentDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.Now}");

            Assert.True(actualResult.IsOk);
            actualResult.ResultValue.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void ParseAndCastToDateTime_GivenUtcNowDateTimeExpression_ReturnsCurrentUtcDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.UtcNow}");

            Assert.True(actualResult.IsOk);
            actualResult.ResultValue.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [MemberData(nameof(ValidJsonExprs))]
        public void ParseAndCastToDateTime_GivenBinaryExpressionReducedToDateTime_ReturnsAppropriateDateTime(string expr, DateTime expected)
        {
            var actualResult = ParseAndCastToDateTime(expr);

            Assert.True(actualResult.IsOk);
            actualResult.ResultValue.Should().BeCloseTo(expected, TimeSpan.FromSeconds(20));
        }

        [Theory]
        [MemberData(nameof(InvalidJsonExprs))]
        public void ParseAndCastToDateTime_GivenBinaryExpressionNotReducedToDateTime_ReturnErrorMessage(string expr, Exception expectedExn)
        {
            try
            {
                var actualResult = ParseAndCastToDateTime(expr);
                Assert.True(actualResult.IsError);
            }
            catch (Exception actualExn)
            {
                Assert.Equal(expectedExn.GetType(), actualExn.GetType());
                actualExn.Message.Should().BeEquivalentTo(expectedExn.Message);
            }

        }

        public static IEnumerable<object[]> ValidJsonExprs = new[]
        {
            ("DateTime.MinValue + 89.9095d", DateTime.MinValue.Add(TimeSpan.FromDays(89.9095))),
            ("DateTime.Now + 30d_03h", DateTime.Now.Add(TimeSpan.FromDays(30) + TimeSpan.FromHours(3))),
            ("DateTime.UtcNow + 20.25d_62.125h_83m99.05s", DateTime.UtcNow.Add(
                TimeSpan.FromDays(20.25)
                + TimeSpan.FromHours(62.125)
                + TimeSpan.FromMinutes(83)
                + TimeSpan.FromSeconds(99.05))),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();

        public static IEnumerable<object[]> InvalidJsonExprs = new ValueTuple<string, Exception>[]
        {
            ("2.0 + 3", null),  
            ("DateTime.Now + 2.022", 
                new NotImplementedException("Unsupported operation: Addition of DateTimeLiteral to FloatLiteral.")),
            ("30d_03h + DateTime.UtcNow", 
                new InvalidOperationException("Invalid operation: Addition of TimeSpan to DateTime. Try swapping the operands?")),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
