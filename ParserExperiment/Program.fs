module ParserExperiments.Program

open Parsers
open Features


let demoGuid (guidExpr: string) = 
    printfn $"\"{guidExpr}\" -> "
    match ParseToGuid guidExpr with
    | Result.Ok value -> printfn $"\t{value}\n"
    | Result.Error msg -> printfn $"\tFailed: {msg}"

[<EntryPoint>]
let main argv =
    printfn $"/* Parser Experiments Demo */\n"

    demoGuid "${Guid.NewGuid}"
    demoGuid "${Guid.Parse D0C9CC05-CB74-4732-BEDD-66AD4E03F897}"
    demoGuid "${Guid.Empty}"

    0 // return an integer exit code
