using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static YayysonParser.Features;

namespace YayysonParser.Tests.Features
{
    public class ParseToTimeSpanTest
    {
        [Theory]
        [MemberData(nameof(ValidYayysonExprs))]
        public void ParseToTimeSpan_GivenValidTimeSpanYayysonExpr_ReturnValue(string expr, TimeSpan expected)
        {
            var actualResult = ParseToTimeSpan(expr);

            Assert.True(actualResult.IsOk);
            Assert.Equal(expected, actualResult.ResultValue);
        }

        [Theory]
        [MemberData(nameof(InvalidYayysonExprs))]
        public void ParseToTimeSpan_GivenInvalidTimeSpanYayysonExpr_DoesNotReturnValue(string expr)
        {
            var actualResult = ParseToTimeSpan(expr);

            Assert.True(actualResult.IsError);
            Assert.NotNull(actualResult.ErrorValue);
        }

        public static IEnumerable<object[]> ValidYayysonExprs = new[]
        {
            ("0.0d0h0m0s", TimeSpan.Zero),
            ("89.9095d", TimeSpan.FromDays(89.9095)),
            ("30d_03h", TimeSpan.FromDays(30) + TimeSpan.FromHours(3)),
            ("9h_32.5s", TimeSpan.FromHours(9) + TimeSpan.FromSeconds(32.5)),
            ("28.82m", TimeSpan.FromMinutes(28.82)),
            ("20.25d_62.125h_83m99.05s",
                TimeSpan.FromDays(20.25) 
                + TimeSpan.FromHours(62.125) 
                + TimeSpan.FromMinutes(83) 
                + TimeSpan.FromSeconds(99.05)),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();

        public static IEnumerable<object[]> InvalidYayysonExprs = new[]
        {
            "0.0h0d0m0s",  // Hours-part precedes Days-part
            "89,9095d",
            "30d 03h",
        }.Select(x => new object[] { string.Format("${{{0}}}", x) }).AsEnumerable();
    }
}
