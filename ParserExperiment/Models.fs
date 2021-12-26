module ParserExperiments.Models

open System


type BinaryOperator = 
    | Add 
    | Subtract

type Expr = 
    | IntLiteral of int
    | FloatLiteral of float
    | GuidLiteral of Guid
    | DateTimeLiteral of DateTime
    | TimeSpanLiteral of TimeSpan
    | Identifer of string
    | Binary of (BinaryOperator * Expr * Expr)
