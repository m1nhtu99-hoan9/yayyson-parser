module YayysonParser.DemoProgram

open System

open Features


type Random with
    member inline self.NextSingle () = self.NextDouble () |> float32
    member inline self.NextSingle (maxValue: float32) = self.NextSingle() * maxValue


let inline private _printInput content = printfn "\"%s\" -> " content
let inline private _printOkValue content = printfn "\t%A\n" content 
let inline private _printErrorMsg message = printfn "\tFailed: %s\n" message
let inline private _printExn (exn: Exception) = printfn "\t Exception: [%s] %s" <| exn.GetType().Name <| exn.Message 

let private _demoParsing<'TResult> (fn: string -> Result<'TResult, string>) (expr: string) =
    _printInput expr
    try
        match fn expr with
        | Result.Ok value -> _printOkValue value
        | Result.Error msg -> _printErrorMsg msg
    with
        | exn -> _printExn exn


let demoParsingGuid = _demoParsing ParseToGuid
let demoParsingGuid' = _demoParsing ParseAndCastToGuid
let demoParsingTimeSpan = _demoParsing ParseToTimeSpan
let demoParsingTimeSpan' = _demoParsing ParseAndCastToTimeSpan
let demoParsingInt32' = _demoParsing ParseAndCastToInt32
let demoParsingFloat32' = _demoParsing ParseAndCastToFloat
let demoParsingDateTime' = _demoParsing ParseAndCastToDateTime


[<EntryPoint>]
let main argv =
    let r = new Random ()

    printfn "YAYYSON PARSER DEMO\n"

    printfn "＼(^_^ ) GUID ＼(^_^ )\n"
    demoParsingGuid "${Guid.NewGuid}"
    demoParsingGuid <| String.Format ("${{Guid.Parse {0}}}", Guid.NewGuid().ToString().ToUpperInvariant())
    demoParsingGuid "${Guid.Empty}"
    printfn "\n"
    
    printfn "＼(^_^ ) TimeSpan ＼(^_^ )\n"
    demoParsingTimeSpan <| String.Format (
        "${{{0}d_{1}h_{2}m{3}s}}", 
        r.NextSingle 100.0f, r.NextSingle 24.0f, r.NextSingle 80.0f, r.NextSingle 100.0f)
    demoParsingTimeSpan' <| String.Format (
        "${{{0}h + {1}d_{2}h}}", 
        r.Next 100, r.Next 100, r.Next 100)
    demoParsingTimeSpan' <| String.Format (
        "${{{0}d - {1}d_{2}h_{3}m{4}s}}",
        r.NextSingle 100.0f, r.NextSingle 80.0f, r.NextSingle 24.0f, r.NextSingle 60.0f, r.NextSingle 60.0f)
    demoParsingTimeSpan' <| String.Format (
        "${{{0} + {1}d_{2}h_{3}m_{4}s}}",
        r.NextSingle 1000.0f, r.NextSingle 80.0f, r.NextSingle 24.0f, r.NextSingle 60.0f, r.NextSingle 60.0f)
    printfn "\n"
    
    printfn "＼(^_^ ) DateTime ＼(^_^ )\n"
    demoParsingDateTime' "${DateTime.MinValue}"
    demoParsingDateTime' "${DateTime.MaxValue}"
    demoParsingDateTime' "${DateTime.Now}"
    demoParsingDateTime' "${DateTime.UtcNow}"
    demoParsingDateTime' <| String.Format ("${{DateTime.MinValue + {0}d}}", r.NextSingle 100.0f)
    demoParsingDateTime' <| String.Format ("${{DateTime.Now - {0}d_{1}h}}", r.NextSingle 100.0f, r.NextSingle 24.0f)
    demoParsingDateTime' <| String.Format (
        "${{DateTime.UtcNow + {0}d_{1}h_{2}m_{3}s}}",
        r.NextSingle 100.0f, r.NextSingle 24.0f, r.NextSingle 60.0f, r.NextSingle 60.0f)
    demoParsingDateTime' <| String.Format ("${{DateTime.Now + {0}}}", r.NextSingle 20.0f)
    demoParsingDateTime' <| String.Format ("${{{0}d{1}h + DateTime.UtcNow}}", r.NextSingle 30.0f, r.NextSingle 24.0f)
    printfn "\n"
    
    printfn "＼(^_^ ) Float32 ＼(^_^ )\n"
    demoParsingFloat32' <| String.Format ("${{{0}}}", r.NextSingle 1000.0f)
    demoParsingFloat32' <| String.Format ("${{{0} + {1}}}", r.NextSingle 1000.0f, r.Next 1000)
    demoParsingFloat32' <| String.Format ("${{{0} - {1}}}", r.Next 1000, r.NextSingle 1000.0f)
    demoParsingFloat32' <| String.Format ("${{{0} + {1}}}", r.NextSingle 1000.0f, r.NextSingle 1000.0f)
    demoParsingFloat32' <| String.Format ("${{{0} - {1}}}", r.NextSingle 1000.0f, r.NextSingle 1000.0f)
    printfn "\n"
    
    printfn "＼(^_^ ) Int32 ＼(^_^ )\n"
    demoParsingInt32' <| String.Format ("${{{0}}}", r.Next 1000)
    demoParsingInt32' <| String.Format ("${{{0} + {1}}}", r.Next 1000, r.Next 1000)
    demoParsingInt32' <| String.Format ("${{{0} - {1}}}", r.Next 1000, r.Next 1000)
    printfn "\n"

    0 // return an integer exit code
