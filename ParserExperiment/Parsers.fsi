module ParserExperiments.Parsers

open System
open FParsec

open Models


val runPExpr: pexpr: Parser<'TResult, unit> -> streamName: string -> string -> ParserResult<'TResult, unit>
val pTimeSpan : Parser<TimeSpan, unit>
val pGuidExpr : Parser<GuidExpr, unit>
