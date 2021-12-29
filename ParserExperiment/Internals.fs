module internal ParserExperiments.Internals

open Microsoft.FSharp.Reflection


let getUnionCaseName (x: 'UnionType) = 
    match FSharpValue.GetUnionFields (x, x.GetType()) with 
    | case, _ -> case.Name

