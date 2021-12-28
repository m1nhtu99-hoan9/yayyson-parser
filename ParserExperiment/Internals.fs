module internal ParserExperiments.Internals

open Microsoft.FSharp.Reflection


let getUnionCaseName (x: 'UnionType) = 
    match FSharpValue.GetUnionFields (x, typeof<'UnionType>) with 
    | case, _ -> case.Name

