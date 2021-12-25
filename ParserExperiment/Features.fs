module ParserExperiments.Features

open System
open FParsec 

open ParserExperiments.Parsers
open ParserExperiments.Evaluation


let ParseToGuid (str: string) = 
    match runPExpr pGuidExpr "GUID expression" str with 
    | ParserResult.Success (guidExpr, _, _) -> Result.Ok <| evalGuidExpr guidExpr
    | ParserResult.Failure (msg, _, _) -> Result.Error msg 


let ParseToTimeSpan (str: string) = 
    match runPExpr pTimeSpan "TimeSpan expression" str with
    | ParserResult.Success (timeSpan, _, _) -> Result.Ok timeSpan
    | ParserResult.Failure (msg, _, _) -> Result.Error msg

