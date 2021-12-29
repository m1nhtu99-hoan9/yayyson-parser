module YayysonParser.Evaluation

open System

open Models


val eval: expr: Expr -> Expr
val evalBinaryExpr: binaryOp: BinaryOperator * expr1: Expr * expr2: Expr -> Expr

val unpack: expr: Expr -> obj
