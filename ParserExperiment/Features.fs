module ParserExperiments.Features

open System
open FParsec 

open ParserExperiments.Parsers


let ParseToGuid (str: string) = 
    match runExprParser pGuid "GUID expression" str with 
    | ParserResult.Success (guid, _, _) -> Result.Ok guid   
    | ParserResult.Failure (msg, _, _) -> Result.Error msg 


let ParseToTimeSpan (str: string) = 
    match runExprParser pTimeSpan "TimeSpan expression" str with
    | ParserResult.Success (timeSpan, _, _) -> Result.Ok timeSpan
    | ParserResult.Failure (msg, _, _) -> Result.Error msg

