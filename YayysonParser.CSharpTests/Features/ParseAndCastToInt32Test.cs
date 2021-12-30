using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;
using static YayysonParser.Features;

namespace YayysonParser.Tests.Features
{
    public class ParseAndCastToInt32Test
    {
        [Theory]
        [MemberData(nameof(ValidYayysonExprs))]
        public void ParseAndCastToInt32_GivenBinaryExpressionReducedToInt32_ReturnsAppropriateInt32(string expr, int expected)
        {
            var actualResult = ParseAndCastToInt32(expr);

            Assert.True(actualResult.IsOk);
            Assert.Equal(expected, actualResult.ResultValue);
        }

        [Theory]
        [MemberData(nameof(InvalidYayysonExprs))]
        public void ParseAndCastToInt32_GivenBinaryExpressionNotReducedToInt32_ReturnError(string expr, string expectedMsg)
        {
            try
            {
                var actualResult = ParseAndCastToInt32(expr);

                Assert.True(actualResult.IsError);
                Assert.Equal(expectedMsg, actualResult.ErrorValue);
            }
            catch (Exception exn)
            {
                throw new XunitException($"Expected no exception, but found: {exn}");
            }
        }

        public static IEnumerable<object[]> ValidYayysonExprs = new []
        {
            ("1999", 1999),
            ("2030 + 2099", 2030 + 2099),
            ("2299 - 2088", 2299 - 2088),
            ("3489 - 3298 + 1986", 3489 - 3298 + 1986)
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();

        public static IEnumerable<object[]> InvalidYayysonExprs = new ValueTuple<string, string>[]
        {
            ("221.75 + 38", "Given expression not evaluated to System.Int32, but System.Single"),
            ("18h58s + 7m", "Given expression not evaluated to System.Int32, but System.TimeSpan"),
            ("3h32m15s + 2", "Unsupported operation: Addition of TimeSpanLiteral to IntLiteral."),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
