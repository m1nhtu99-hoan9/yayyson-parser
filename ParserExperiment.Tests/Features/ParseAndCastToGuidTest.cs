using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static ParserExperiments.Features;

namespace ParserExperiments.Tests.Features
{
    public class ParseAndCastToGuidTest
    {
        [Fact]
        public void ParseAndCastToGuid_GivenNewGuidJsonExpr_ReturnNewGuid()
        {
            var actualResult = ParseAndCastToGuid("${Guid.NewGuid}");

            Assert.True(actualResult.IsOk);
            Assert.NotEqual(Guid.Empty, actualResult.ResultValue);
        }

        [Fact]
        public void ParseAndCastToGuid_GivenEmptyGuidExpr_ReturnEmptyGuid()
        {
            var actualResult = ParseAndCastToGuid("${Guid.Empty}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(Guid.Empty, actualResult.ResultValue);
        }

        [Fact]
        public void ParseAndCastToGuid_GivenGuidParseExpr_ReturnExpectedGuid()
        {
            var expected = Guid.NewGuid();
            var guidExprString = string.Format("${{Guid.Parse {0}}}", expected.ToString("D"));
            var actualResult = ParseAndCastToGuid(guidExprString);

            Assert.True(actualResult.IsOk);
            Assert.Equal(expected, actualResult.ResultValue);
        }

        [Theory]
        [MemberData(nameof(InvalidJsonExprs))]
        public void ParseAndCastToGuid_GivenBinaryExpressionNotReducedToDateTime_ReturnError(string expr, Exception expectedExn)
        {
            try
            {
                var actualResult = ParseAndCastToGuid(expr);
                Assert.True(actualResult.IsError);
            }
            catch (Exception actualExn)
            {
                Assert.Equal(expectedExn.GetType(), actualExn.GetType());
                Assert.Equal(expectedExn.Message, actualExn.Message);
            }
        }

        public static IEnumerable<object[]> InvalidJsonExprs = new ValueTuple<string, Exception>[]
        {
            ("Guid.NewGuid + DateTime.Now",
                new NotImplementedException("Unsupported operation: Addition of GuidLiteral to DateTimeLiteral.")),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
