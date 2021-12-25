module ParserExperiments.Models

open System



type BinaryOperator = 
    | Add 
    | Substract

type Expr = 
    | IntLiteral of int
    | FloatLiteral of float
    | StringLiteral of string
    | DateTimeLiteral of DateTime
    | TimeSpanLiteral of TimeSpan
    | Identifer of string
    | Binary of (BinaryOperator * Expr * Expr)
    | GuidLiteral of Guid
