# Parser Experiments

## Miscellaneous

```fsharp
Expr.DateTimeLiteral <| new DateTime (2021, 12, 25)
// is identical to
Expr.DateTimeLiteral (new DateTime (2021, 12, 25))
```