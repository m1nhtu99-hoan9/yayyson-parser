[<AutoOpen>]
module ParserExperiments.Features

open System
open FParsec 

open Parsers
open Evaluation


let private _parseAndCast<'TResult> (str: string) = 
    match runParserOnString pFullExpr () "Yayyson expression" str with
    | ParserResult.Success (v, _, _) -> 
        let v0 = unpack v
        try
            Result.Ok (v0 :?> 'TResult)
        with
            | :? InvalidCastException -> 
                Result.Error $"Given expression not evaluated to {typeof<'TResult>.FullName}, but {v0.GetType().FullName}"
    | ParserResult.Failure (msg, _, _) -> Result.Error msg


let private _parseSingularExpr<'TResult> (p: Parser<'TResult, unit>) (str: string) = 
    match runExprParser p $"Yayyson {typeof<'TResult>.Name} expression" str with 
    | ParserResult.Success (v, _, _) -> Result.Ok v   
    | ParserResult.Failure (msg, _, _) -> Result.Error msg 


let ParseToGuid (str: string) = _parseSingularExpr pGuid str

let ParseToTimeSpan (str: string) = _parseSingularExpr pTimeSpan str


let ParseAndCastToGuid (str: string) = _parseAndCast<Guid> str

let ParseAndCastToTimeSpan (str: string) = _parseAndCast<TimeSpan> str

let ParseAndCastToDateTime (str: string) = _parseAndCast<DateTime> str

let ParseAndCastToFloat (str: string) = _parseAndCast<float32> str

let ParseAndCastToInt32 (str: string) = _parseAndCast<int> str