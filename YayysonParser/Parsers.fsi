module YayysonParser.Parsers

open System
open FParsec

open Models


val pFullExpr: Parser<Expr, unit>
val pTimeSpan: Parser<TimeSpan, unit>
val pDateTime: Parser<DateTime, unit>
val pGuid: Parser<Guid, unit>

val internal pNumericExpr: Parser<NumericLiteral, unit>

val internal runExprParser: pExprContent: Parser<'TResult, unit> -> streamName: string -> string 
    -> ParserResult<'TResult, unit> 
val internal createExprParser: pExprContent: Parser<'TResult, unit> 
    -> Parser<'TResult, unit>
