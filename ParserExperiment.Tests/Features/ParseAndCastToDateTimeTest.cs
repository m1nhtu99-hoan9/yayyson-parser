using System;
using FluentAssertions;
using Xunit;
using static ParserExperiments.Features;

namespace ParserExperiments.Tests.Features
{
    public class ParseAndCastToDateTimeTest
    {
        [Fact] 
        public void ParseAndCastToDateTime_GivenMinValueDateTimeExpression_ReturnsMinDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.MinValue}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(DateTime.MinValue, actualResult.ResultValue);
        }

        [Fact]
        public void ParseAndCastToDateTime_GivenMaxValueDateTimeExpression_ReturnsMaxDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.MaxValue}");

            Assert.True(actualResult.IsOk);
            Assert.Equal(DateTime.MaxValue, actualResult.ResultValue);
        }
        
        [Fact]
        public void ParseAndCastToDateTime_GivenNowDateTimeExpression_ReturnsCurrentDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.Now}");

            Assert.True(actualResult.IsOk);
            actualResult.ResultValue.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
        
        [Fact]
        public void ParseAndCastToDateTime_GivenUtcNowDateTimeExpression_ReturnsCurrentUtcDateTime()
        {
            var actualResult = ParseAndCastToDateTime("${DateTime.UtcNow}");

            Assert.True(actualResult.IsOk);
            actualResult.ResultValue.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }
}
