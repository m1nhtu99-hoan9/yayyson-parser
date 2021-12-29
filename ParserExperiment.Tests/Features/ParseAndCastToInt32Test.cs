using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static ParserExperiments.Features;

namespace ParserExperiments.Tests.Features
{
    public class ParseAndCastToInt32Test
    {
        [Theory]
        [MemberData(nameof(ValidJsonExprs))]
        public void ParseAndCastToInt32_GivenBinaryExpressionReducedToInt32_ReturnsAppropriateInt32(string expr, int expected)
        {
            var actualResult = ParseAndCastToInt32(expr);

            Assert.True(actualResult.IsOk);
            Assert.Equal(expected, actualResult.ResultValue);
        }

        [Theory]
        [MemberData(nameof(InvalidJsonExprs))]
        public void ParseAndCastToInt32_GivenBinaryExpressionNotReducedToInt32_ReturnError(string expr, Exception expectedExn)
        {
            try
            {
                var actualResult = ParseAndCastToInt32(expr);

                Assert.True(actualResult.IsError);
            }
            catch (Exception actualExn)
            {
                Assert.Equal(expectedExn.GetType(), actualExn.GetType());
                Assert.Equal(expectedExn.Message, actualExn.Message);
            }
        }

        public static IEnumerable<object[]> ValidJsonExprs = new []
        {
            ("1999", 1999),
            ("2030 + 2099", 2030 + 2099),
            ("2299 - 2088", 2299 - 2088),
            ("3489 - 3298 + 1986", 3489 - 3298 + 1986)
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();

        public static IEnumerable<object[]> InvalidJsonExprs = new ValueTuple<string, Exception>[]
        {
            ("221.75 + 38", new InvalidCastException()),
            ("18h58s + 7m", new InvalidCastException()),
            ("3h32m15s + 2", new NotImplementedException("Unsupported operation: Addition of TimeSpanLiteral to IntLiteral.")),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
