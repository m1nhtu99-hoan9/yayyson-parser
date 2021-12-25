module ParserExperiments.Parsers

open System
open FParsec

open Models


val pTimeSpan: Parser<TimeSpan, unit>
val pDateTime: Parser<DateTime, unit>
val pGuid: Parser<Guid, unit>
val pIdentifierExpr: Parser<Expr, unit>

val internal runExprParser: pExprContent: Parser<'TResult, unit> -> streamName: string -> string 
    -> ParserResult<'TResult, unit> 
val internal createExprParser: pExprContent: Parser<'TResult, unit> 
    -> Parser<'TResult, unit>
