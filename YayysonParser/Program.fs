module YayysonParser.Program

open Parsers
open Features


let demoParsingGuid (guidExpr: string) = 
    printfn $"\"{guidExpr}\" -> "
    match ParseToGuid guidExpr with
    | Result.Ok value -> printfn $"\t{value}\n"
    | Result.Error msg -> printfn $"\tFailed: {msg}"

[<EntryPoint>]
let main argv =
    printfn $"/* Parser Experiments Demo */\n"

    demoParsingGuid "${Guid.NewGuid}"
    demoParsingGuid "${Guid.Parse D0C9CC05-CB74-4732-BEDD-66AD4E03F897}"
    demoParsingGuid "${Guid.Empty}"

    0 // return an integer exit code
