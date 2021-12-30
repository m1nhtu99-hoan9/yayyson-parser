using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;
using static YayysonParser.Features;

namespace YayysonParser.Tests.Features
{
    public class ParseAndCastToGuidTest
    {
        [Fact]
        public void ParseAndCastToGuid_GivenNewGuidYayysonExpr_ReturnNewGuid()
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
        [MemberData(nameof(InvalidYayysonExprs))]
        public void ParseAndCastToGuid_GivenBinaryExpressionNotReducedToDateTime_ReturnError(string expr, string expectedMsg)
        {
            try
            {
                var actualResult = ParseAndCastToGuid(expr);
                Assert.True(actualResult.IsError);
                Assert.Equal(expectedMsg, actualResult.ErrorValue);
            }
            catch (Exception exn)
            {
                throw new XunitException($"Expected no exceptionn, but found: {exn}");
            }
        }

        public static IEnumerable<object[]> InvalidYayysonExprs = new []
        {
            ("Guid.NewGuid + DateTime.Now","Unsupported operation: Addition of GuidLiteral to DateTimeLiteral."),
        }.Select(x => new object[] { string.Format("${{{0}}}", x.Item1), x.Item2 }).AsEnumerable();
    }
}
