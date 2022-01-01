using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;
using static YayysonParser.Features;

namespace YayysonParser.Tests.Features
{
    public class ParseToGuidTest
    {
        [Fact]
        public void GivenNewGuidYayysonExpr_ReturnNewGuid()
        {
            var actualResult = ParseToGuid("${Guid.NewGuid}");

            Assert.True(actualResult.IsOk);
            Assert.NotEqual(Guid.Empty, actualResult.ResultValue);
        }

        [Fact]
        public void GivenEmptyGuidExpr_ReturnEmptyGuid()
        {
            var actualResult = ParseToGuid("${Guid.Empty}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(Guid.Empty, actualResult.ResultValue);
        }

        [Fact]
        public void GivenValidGuidParseExpr_ReturnExpectedGuid()
        {
            var expected = Guid.NewGuid();
            var guidExprString = string.Format("${{Guid.Parse {0}}}", expected.ToString("D"));
            var actualResult = ParseToGuid(guidExprString);

            Assert.True(actualResult.IsOk);
            Assert.Equal(expected, actualResult.ResultValue);
        }
        
        [Theory]
        [MemberData(nameof(GetInvalidYayysonExprContents))]
        public void GivenInvalidGuidParseExpr_ReturnError(string invalidExprContent,
            params string[] expectedMsgs)
        {
            try
            {
                var actualResult = ParseToGuid(string.Format("${{Guid.Parse {0}}}", invalidExprContent));

                Assert.True(actualResult.IsError);
                
                foreach (var msgSnippet in expectedMsgs)
                {
                    Assert.Contains(msgSnippet, actualResult.ErrorValue);
                }
            } 
            catch (Exception exn)
            {
                throw new XunitException($"Expected no exception, but found: {exn}");
            }

        }

        public static IEnumerable<object[]> GetInvalidYayysonExprContents()
        {
            var literal0 = Guid.NewGuid().ToString();
            yield return new[] 
            { 
                literal0.Substring(1, literal0.Length - 1),
                "Guid should contain 32 digits with 4 dashes",
                "(xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."
            };

            yield return new[] 
            { 
                "not-even-a-valid-guid",
                "Unrecognized Guid format."
            };
        }

    }
}
