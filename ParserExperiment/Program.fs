module ParserExperiments.Program

open System
open FParsec

open Models
open Parsers
open Evaluation


let demoGuid (guidExpr: string) = 
    printfn $"\"{guidExpr}\" -> "
    match runPExpr pGuidExpr "GUID expression" guidExpr with
    | ParserResult.Success (r, _, _) -> printfn $"\t{evalGuidExpr r}\n"
    | ParserResult.Failure (msg, _, _) -> printfn $"\tFailed: {msg}"

[<EntryPoint>]
let main argv =
    printfn $"/* Parser Experiments Demo */\n"

    demoGuid "${Guid.NewGuid}"
    demoGuid "${Guid.Parse D0C9CC05-CB74-4732-BEDD-66AD4E03F897}"
    demoGuid "${Guid.Empty}"

    0 // return an integer exit code
