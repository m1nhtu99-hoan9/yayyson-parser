module ParserExperiments.Evaluation

open System

open Models


let _evalAdd (expr1: Expr, expr2: Expr) =
    match (expr1, expr2) with
    | (DateTimeLiteral x1, TimeSpanLiteral x2) -> Result.Ok <| x1.Add x2 
    | _ -> Result.Error "Unsupported type"


let evalBinaryExpr ((binaryOp, expr1, expr2): (BinaryOperator * Expr * Expr)) =
    match binaryOp with
    | Add -> _evalAdd (expr1, expr2) 
    | _ -> Result.Error "Unsupported operation"
