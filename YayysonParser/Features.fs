[<AutoOpen>]
module YayysonParser.Features

open System
open FParsec 

open Parsers
open Evaluation


let private _parseAndCast<'TResult> (str: string) = 
    match runParserOnString pFullExpr () "Yayyson expression" str with
    | ParserResult.Success (v, _, _) -> 
        let mutable v0 : obj = null
            
        try
            v0 <- unpack v

            Result.Ok (v0 :?> 'TResult)
        with
            | :? InvalidCastException -> 
                Result.Error <| String.Format (
                    "Given expression not evaluated to {0}, but {1}", 
                    typeof<'TResult>.FullName, if v0 = null then "null" else v0.GetType().FullName)
            | :? InvalidOperationException as exn -> Result.Error exn.Message 
            | :? NotImplementedException as exn -> Result.Error exn.Message
            | _ -> reraise ()
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