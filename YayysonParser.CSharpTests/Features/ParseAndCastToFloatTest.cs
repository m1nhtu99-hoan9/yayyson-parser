using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using static YayysonParser.Features;

namespace YayysonParser.Tests.Features
{
    public class ParseAndCastToFloatTest
    {
        [Theory]
        [MemberData(nameof(ValidYayysonExprs))]
        public void ParseAndCastToFloat_GivenBinaryExpressionReducedToFloat_ReturnsAppropriateFloat(string expr, float expected)
        {
            var actualResult = ParseAndCastToFloat(expr);

            Assert.True(actualResult.IsOk, $"Result is error with message: '{actualResult.ErrorValue}'.");
            actualResult.ResultValue.Should().Equals(expected);
        }

        [Theory]
        [MemberData(nameof(InvalidYayysonExprs))]
        public void ParseAndCastToFloat_GivenBinaryExpressionNotReducedToFloat_ReturnError(string expr, Exception expectedExn)
        {
            try
            {
                var actualResult = ParseAndCastToFloat(expr);

                Assert.True(actualResult.IsError);
            }
            catch (Exception actualExn)
            {
                Assert.Equal(expectedExn.GetType(), actualExn.GetType());
                Assert.Equal(expectedExn.Message, actualExn.Message);
            }
        }

        public static IEnumerable<object[]> ValidYayysonExprs = new[]
        {
            ("1999.91", 1999.91),
            ("2030.75 + 2099", 2030.75 + 2099.0),
            ("2299 - 2088.88", 2299.0 - 2088.88),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();

        public static IEnumerable<object[]> InvalidYayysonExprs = new ValueTuple<string, Exception>[]
        {
            ("3489 - 3298 + 1986", new InvalidCastException("Given expression not evaluated to System.Single, but System.Int32")),
            ("DateTime.Now + 9h30m", new InvalidCastException()),
            ("3h32m15s + 2.99", 
                new NotImplementedException("Unsupported operation: Addition of TimeSpanLiteral to FloatLiteral.")),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
