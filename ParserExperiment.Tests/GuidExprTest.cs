using System;
using Xunit;
using static ParserExperiments.Features;

namespace ParserExperiments.Tests
{
    public class GuidExprTest
    {
        [Fact]
        public void ParseNewGuidJsonExpr_ReturnNewGuid()
        {
            var actualResult = ParseToGuid("${Guid.NewGuid}");

            Assert.True(actualResult.IsOk);
            Assert.NotEqual(Guid.Empty, actualResult.ResultValue);
        }

        [Fact]
        public void ParseEmptyGuidExpr_ReturnEmptyGuid()
        {
            var actualResult = ParseToGuid("${Guid.Empty}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(Guid.Empty, actualResult.ResultValue);
        }

        [Fact]
        public void ParseGuidParseExpr_ReturnExpectedGuid()
        {
            const string literal = "D0C9CC05-CB74-4732-BEDD-66AD4E03F897";
            var guidExprString = string.Format("${{Guid.Parse {0}}}", literal);
            var actualResult = ParseToGuid(guidExprString);

            Assert.True(actualResult.IsOk);
            Assert.Equal(Guid.Parse(literal), actualResult.ResultValue);
        }
    }
}
