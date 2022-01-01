using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;
using static YayysonParser.Features;

namespace YayysonParser.Tests.Features
{
    public class ParseAndCastToFloatTest
    {
        [Theory]
        [MemberData(nameof(ValidYayysonExprs))]
        public void GivenBinaryExpressionReducedToFloat_ReturnsAppropriateFloat(string expr, float expected)
        {
            var actualResult = ParseAndCastToFloat(expr);

            Assert.True(actualResult.IsOk, $"Result is error with message: '{actualResult.ErrorValue}'.");
            actualResult.ResultValue.Should().Equals(expected);
        }

        [Theory]
        [MemberData(nameof(InvalidYayysonExprs))]
        public void GivenBinaryExpressionNotReducedToFloat_ReturnError(string expr, string expectedMsg)
        {
            try
            {
                var actualResult = ParseAndCastToFloat(expr);

                Assert.True(actualResult.IsError);
                Assert.Equal(expectedMsg, actualResult.ErrorValue);
            }
            catch (Exception exn)
            {
                throw new XunitException($"Expected no exception, but found: {exn}");
            }
        }


        public static IEnumerable<object[]> ValidYayysonExprs = new[]
        {
            ("1999.91", 1999.91),
            ("2030.75 + 2099", 2030.75 + 2099.0),
            ("2299 - 2088.88", 2299.0 - 2088.88),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();

        public static IEnumerable<object[]> InvalidYayysonExprs = new []
        {
            ("3489 - 3298 + 1986", "Given expression not evaluated to System.Single, but System.Int32"),
            ("DateTime.Now + 9h30m", "Given expression not evaluated to System.Single, but System.DateTime"),
            ("3h32m15s + 2.99", "Unsupported operation: Addition of TimeSpanLiteral to FloatLiteral."),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
