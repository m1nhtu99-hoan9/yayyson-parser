using System;
using FParsec.CSharp;
using Xunit;

namespace ParserExperiments.Tests
{
    public class GuidExprTest
    {
        [Fact]
        public void ParseNewGuidJsonExpr_ReturnNewGuid()
        {
            var guidExprString = "${Guid.NewGuid}";
            var (guidExpr, msg) = Parsers.runPExpr(Parsers.pGuidExpr, "GUID expression", guidExprString)
                .UnwrapResult();
            var actualGuid = Evaluation.evalGuidExpr(guidExpr);

            Assert.Null(msg);
            Assert.NotEqual(Guid.Empty, actualGuid);
        }

        [Fact]
        public void ParseEmptyGuidExpr_ReturnEmptyGuid()
        {
            var guidExprString = "${Guid.Empty}";
            var (guidExpr, msg) = Parsers.runPExpr(Parsers.pGuidExpr, "GUID expression", guidExprString)
                .UnwrapResult();
            var actualGuid = Evaluation.evalGuidExpr(guidExpr);

            Assert.Null(msg);
            Assert.Equal(Guid.Empty, actualGuid);
        }

        [Fact]
        public void ParseGuidParseExpr_ReturnExpectedGuid()
        {
            const string literal = "D0C9CC05-CB74-4732-BEDD-66AD4E03F897";
            var guidExprString = string.Format("${{Guid.Parse {0}}}", literal);
            var (guidExpr, msg) = Parsers.runPExpr(Parsers.pGuidExpr, "GUID expression", guidExprString)
                .UnwrapResult();
            var actualGuid = Evaluation.evalGuidExpr(guidExpr);

            Assert.Null(msg);
            Assert.Equal(Guid.Parse(literal), actualGuid);
        }
    }
}
