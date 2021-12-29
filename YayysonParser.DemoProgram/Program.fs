module YayysonParser.DemoProgram

open System

open Features


let inline private _printInput content = printfn "\"{%s}\" -> " content
let inline private _printOkValue content = printfn "\t{%A}\n" content 
let inline private _printErrorMsg message = printfn "\tFailed: {%s}\n" message
let inline private _printExn (exn: Exception) = printfn "\t Exception: [{%s}] %s" <| exn.GetType().Name <| exn.Message 

let private _demoParsing<'TResult> (fn: string -> Result<'TResult, string>) (expr: string) =
    _printInput expr
    try
        match fn expr with
        | Result.Ok value -> _printOkValue value
        | Result.Error msg -> _printErrorMsg msg
    with
        | exn -> _printExn exn


let demoParsingGuid = _demoParsing ParseToGuid
    

[<EntryPoint>]
let main argv =
    printfn $"/* Parser Experiments Demo */\n"

    demoParsingGuid "${Guid.NewGuid}"
    demoParsingGuid "${Guid.Parse D0C9CC05-CB74-4732-BEDD-66AD4E03F897}"
    demoParsingGuid "${Guid.Empty}"

    0 // return an integer exit code
