# Parser Experiments

## Miscellaneous

```fsharp
#r "FParsec.dll"; #r "FParsecCS.dll"; #r "ParserExperiment.dll";;
open System; open FParsec; open ParserExperiments.Parsers;;
run pFullExpr "${3h8s+ 2.0}";;
```