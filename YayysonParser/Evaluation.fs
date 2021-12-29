module YayysonParser.Evaluation

open System

open Models
open Internals


let _evalAdd (expr1: Expr, expr2: Expr) : Expr =
    let inline addIntToFloat (i: int) (fl: float32) = (float32 i) + fl |> FloatLiteral

    let addStructToStruct (lit1: StructLiteral) (lit2: StructLiteral) = 
        match (lit1, lit2) with
        | (TimeSpanLiteral x1, TimeSpanLiteral x2) -> x1 + x2   |> StructLiteral.TimeSpanLiteral :> ILiteral |> Expr.Literal
        | (DateTimeLiteral x1, TimeSpanLiteral x2) -> x1.Add x2 |> StructLiteral.DateTimeLiteral :> ILiteral |> Expr.Literal
        | (TimeSpanLiteral _, DateTimeLiteral _) -> 
            raise <| new InvalidOperationException "Invalid operation: Addition of TimeSpan to DateTime. Try swapping the operands?"
        | (v1, v2) -> 
            raise <| new NotImplementedException $"Unsupported operation: Addition of {getUnionCaseName v1} to {getUnionCaseName v2}."

    let addNumToNum (numExpr1: NumericLiteral) (numExpr2: NumericLiteral) =
        match (numExpr1, numExpr2) with
        | (IntLiteral x1, IntLiteral x2) -> x1 + x2 |> IntLiteral :> ILiteral |> Expr.Literal
        | (FloatLiteral x1, FloatLiteral x2) -> x1 + x2 |> FloatLiteral :> ILiteral |> Expr.Literal
        | (IntLiteral x1, FloatLiteral x2) -> addIntToFloat x1 x2 :> ILiteral |> Expr.Literal
        | (FloatLiteral x1, IntLiteral x2) -> addIntToFloat x2 x1:> ILiteral |> Expr.Literal

    match (expr1, expr2) with
    | (Literal l1, Literal l2) ->
        match (l1, l2) with
        | ((:? StructLiteral as litExpr1), (:? StructLiteral as litExpr2)) -> addStructToStruct litExpr1 litExpr2
        | ((:? NumericLiteral as numExpr1), (:? NumericLiteral as numExpr2)) -> addNumToNum numExpr1 numExpr2
        | (v1, v2) -> raise <| new NotImplementedException 
                        $"Unsupported operation: Addition of {getUnionCaseName v1} to {getUnionCaseName v2}."
    | _ -> raise <| new InvalidOperationException $"Argument(s) not reduced to normal form and not ready for addition."


let _evalSubtract (expr1: Expr, expr2: Expr) : Expr =
    let subtractStructToStruct (litExpr1: StructLiteral) (litExpr2: StructLiteral) = 
        match (litExpr1, litExpr2) with
        | (TimeSpanLiteral x1, TimeSpanLiteral x2) -> x1 - x2        |> TimeSpanLiteral :> ILiteral |> Expr.Literal
        | (DateTimeLiteral x1, TimeSpanLiteral x2) -> x1.Subtract x2 |> DateTimeLiteral :> ILiteral |> Expr.Literal
        | (TimeSpanLiteral _, DateTimeLiteral _) -> 
            raise <| new InvalidOperationException "Invalid operation: Subtraction of a TimeSpan to a DateTime. Try swapping the operands?"
        | (v1, v2) -> 
            raise <| new NotImplementedException $"Unsupported operation: Addition of {getUnionCaseName v1} to {getUnionCaseName v2}."

    let subtractNumToNum (numExpr1: NumericLiteral) (numExpr2: NumericLiteral) = 
        match (numExpr1, numExpr2) with
        | (IntLiteral x1, IntLiteral x2) -> x1 - x2 |> IntLiteral :> ILiteral |> Literal
        | (FloatLiteral x1, FloatLiteral x2) -> x1 - x2 |> FloatLiteral :> ILiteral |> Literal
        | (IntLiteral x1, FloatLiteral x2) -> (float32 x1) - x2 |> FloatLiteral :> ILiteral |> Literal
        | (FloatLiteral x1, IntLiteral x2) -> x1 - (float32 x2) |> FloatLiteral :> ILiteral |> Literal
        
    match (expr1, expr2) with
    | (Literal l1, Literal l2) ->
        match (l1, l2) with
        | ((:? StructLiteral as litExpr1), (:? StructLiteral as litExpr2)) -> subtractStructToStruct litExpr1 litExpr2
        | ((:? NumericLiteral as numExpr1), (:? NumericLiteral as numExpr2)) -> subtractNumToNum numExpr1 numExpr2
        | (v1, v2) -> raise <| new NotImplementedException 
                        $"Unsupported operation: Subtraction of {getUnionCaseName v1} to {getUnionCaseName v2}."    
    | (v1, v2) -> 
        raise <| new NotImplementedException $"Unsupported operation: Subtraction of {getUnionCaseName v1} to {getUnionCaseName v2}."


let rec eval (expr: Expr) =
    match expr with
    | Binary tpl -> evalBinaryExpr tpl
    | _ -> expr

and evalBinaryExpr ((binaryOp, expr1, expr2): (BinaryOperator * Expr * Expr)) =
    let expr1' = eval expr1
    let expr2' = eval expr2
    match binaryOp with
    | Add -> _evalAdd (expr1', expr2')
    | Subtract -> _evalSubtract (expr1', expr2')


let unpack (expr: Expr) =
    match eval expr with
    | Literal l -> 
        match l with 
        | :? NumericLiteral as num -> 
            match num with 
            | IntLiteral i -> i :> obj
            | FloatLiteral fl -> fl :> obj
        | :? StructLiteral as strct -> 
            match strct with 
            | GuidLiteral g -> g :> obj
            | DateTimeLiteral d -> d :> obj
            | TimeSpanLiteral t -> t :> obj
        | _ -> raise <| new InvalidOperationException "Unreachable object state"
    | Binary _ -> raise <| new InvalidOperationException "Cannot unpack expression not reduced to normal form."
