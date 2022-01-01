using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;
using static YayysonParser.Features;

namespace YayysonParser.Tests.Features
{
    public class ParseToTimeSpanTest
    {
        [Theory]
        [MemberData(nameof(ValidYayysonExprs))]
        public void GivenValidTimeSpanYayysonExpr_ReturnValue(string expr, TimeSpan expected)
        {
            var actualResult = ParseToTimeSpan(expr);

            Assert.True(actualResult.IsOk);
            Assert.Equal(expected, actualResult.ResultValue);
        }

        [Theory]
        [MemberData(nameof(InvalidYayysonExprs))]
        public void GivenInvalidTimeSpanYayysonExpr_DoesNotReturnValue(string expr, string expectedMsg)
        {
            try
            {
                var actualResult = ParseToTimeSpan(expr);

                Assert.True(actualResult.IsError);
                Assert.Contains(expectedMsg, actualResult.ErrorValue);
            }
            catch (Exception exn)
            {
                throw new XunitException($"Expecting no exception, but found: {exn}");
            }
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
            ("0.5h0d7m9s", "Expecting: 'm' or 's'"),  // Hours-part precedes Days-part
            ("30d 03h", "Expecting: '}'"),
            ("89,9095d", "Expecting: 'd', 'h', 'm' or 's'"),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
