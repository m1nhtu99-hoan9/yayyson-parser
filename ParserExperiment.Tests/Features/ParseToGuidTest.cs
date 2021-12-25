using System;
using Xunit;
using static ParserExperiments.Features;

namespace ParserExperiments.Tests.Features
{
    public class ParseToGuidTest
    {
        [Fact]
        public void ParseToGuid_GivenNewGuidJsonExpr_ReturnNewGuid()
        {
            var actualResult = ParseToGuid("${Guid.NewGuid}");

            Assert.True(actualResult.IsOk);
            Assert.NotEqual(Guid.Empty, actualResult.ResultValue);
        }

        [Fact]
        public void ParseToGuid_GivenEmptyGuidExpr_ReturnEmptyGuid()
        {
            var actualResult = ParseToGuid("${Guid.Empty}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(Guid.Empty, actualResult.ResultValue);
        }

        [Fact]
        public void ParseToGuid_GivenGuidParseExpr_ReturnExpectedGuid()
        {
            var expected = Guid.NewGuid();
            var guidExprString = string.Format("${{Guid.Parse {0}}}", expected.ToString("D"));
            var actualResult = ParseToGuid(guidExprString);

            Assert.True(actualResult.IsOk);
            Assert.Equal(expected, actualResult.ResultValue);
        }
    }
}
