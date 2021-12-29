using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static YayysonParser.Features;

namespace YayysonParser.Tests.Features
{
    public class ParseAndCastToTimeSpanTest
    {
        [Theory]
        [MemberData(nameof(ValidYayysonExprs))]
        public void ParseAndCastToDateTime_GivenBinaryExpressionReducedToDateTime_ReturnsAppropriateDateTime(string expr, TimeSpan expected)
        {
            var actualResult = ParseAndCastToTimeSpan(expr);

            Assert.True(actualResult.IsOk);
            Assert.Equal(expected, actualResult.ResultValue);
        }

        [Theory]
        [MemberData(nameof(InvalidYayysonExprs))]
        public void ParseAndCastToDateTime_GivenBinaryExpressionNotReducedToDateTime_ReturnError(string expr, Exception expectedExn)
        {
            try
            {
                var actualResult = ParseAndCastToTimeSpan(expr);
                Assert.True(actualResult.IsError);
            }
            catch (Exception actualExn)
            {
                Assert.Equal(expectedExn.GetType(), actualExn.GetType());
                Assert.Equal(expectedExn.Message, actualExn.Message);
            }
        }

        public static IEnumerable<object[]> ValidYayysonExprs = new ValueTuple<string, TimeSpan>[]
        {
            ("11d2m9s + 89.9095d", TimeSpan.FromDays(11) 
                + TimeSpan.FromMinutes(2) + TimeSpan.FromSeconds(9) + TimeSpan.FromDays(89.9095)),
            ("12h + 30d_03h", TimeSpan.FromHours(12) + TimeSpan.FromDays(30) + TimeSpan.FromHours(3)),
            ("45d - 20.25d_62.125h_83m99.05s", 
                TimeSpan.FromDays(45)
                - TimeSpan.FromDays(20.25)
                - TimeSpan.FromHours(62.125)
                - TimeSpan.FromMinutes(83)
                - TimeSpan.FromSeconds(99.05)),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();

        public static IEnumerable<object[]> InvalidYayysonExprs = new ValueTuple<string, Exception>[]
        {
            ("DateTime.MinValue + 3h33s", null),
            ("30h_03m + DateTime.Now",
                new InvalidOperationException("Invalid operation: Addition of TimeSpan to DateTime. Try swapping the operands?")),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
