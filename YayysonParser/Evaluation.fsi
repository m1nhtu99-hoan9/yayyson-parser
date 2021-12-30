module YayysonParser.Evaluation

open System

open Models


/// <exception cref="InvalidOperationException"></exception>
/// <exception cref="NotImplementedException"></exception> 
val eval: expr: Expr -> Expr

/// <exception cref="InvalidOperationException"></exception>
/// <exception cref="NotImplementedException"></exception>
val evalBinaryExpr: binaryOp: BinaryOperator * expr1: Expr * expr2: Expr -> Expr

/// <exception cref="InvalidOperationException"></exception>
/// <exception cref="NotImplementedException"></exception>
val unpack: expr: Expr -> obj
