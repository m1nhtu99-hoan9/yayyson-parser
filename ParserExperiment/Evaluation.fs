module ParserExperiments.Evaluation

open System

open Models


let _evalAdd (expr1: Expr, expr2: Expr) : Expr =
    let inline addIntToFloat (i: int) (fl: float) = (float i) + fl |> FloatLiteral

    match (expr1, expr2) with
    | (DateTimeLiteral x1, TimeSpanLiteral x2) -> x1.Add x2 |> DateTimeLiteral
    | (TimeSpanLiteral _, DateTimeLiteral _) -> 
        raise <| new InvalidOperationException "Invalid operation: Addition of a TimeSpan to a DateTime. Try swapping the operands?"
    | (IntLiteral x1, IntLiteral x2) -> x1 + x2 |> IntLiteral
    | (FloatLiteral x1, FloatLiteral x2) -> x1 + x2 |> FloatLiteral
    | (IntLiteral x1, FloatLiteral x2) -> addIntToFloat x1 x2
    | (FloatLiteral x1, IntLiteral x2) -> addIntToFloat x2 x1
    | _ -> raise <| new NotImplementedException "Unsupported type"

let _evalSubtract (expr1: Expr, expr2: Expr) : Expr =
    match (expr1, expr2) with
    | (DateTimeLiteral x1, TimeSpanLiteral x2) -> x1.Subtract x2 |> DateTimeLiteral
    | (TimeSpanLiteral _, DateTimeLiteral _) -> 
        raise <| new InvalidOperationException "Invalid operation: Subtraction of a TimeSpan to a DateTime. Try swapping the operands?"
    | (IntLiteral x1, IntLiteral x2) -> x1 - x2 |> IntLiteral
    | (FloatLiteral x1, FloatLiteral x2) -> x1 - x2 |> FloatLiteral
    | (IntLiteral x1, FloatLiteral x2) -> (float x1) - x2 |> FloatLiteral
    | (FloatLiteral x1, IntLiteral x2) -> x1 - (float x2) |> FloatLiteral
    | _ -> raise <| new NotImplementedException "Unsupported type"


let rec eval (expr: Expr) =
    match expr with
    | Expr.Binary tpl ->  evalBinaryExpr tpl 
    | _ -> expr

and evalBinaryExpr ((binaryOp, expr1, expr2): (BinaryOperator * Expr * Expr)) =
    let expr1' = eval expr1
    let expr2' = eval expr2
    match binaryOp with
    | Add -> _evalAdd (expr1', expr2')
    | Subtract -> _evalSubtract (expr1', expr2')


let unpack (expr: Expr) =
    match eval expr with
    | IntLiteral i -> i :> obj
    | FloatLiteral fl -> fl :> obj
    | GuidLiteral g -> g :> obj
    | DateTimeLiteral d -> d :> obj
    | TimeSpanLiteral t -> t :> obj
    | Identifer id -> id :> obj
    | Binary _ -> raise <| new InvalidOperationException "Cannot unpack expression not reduced to normal form."
