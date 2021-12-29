module YayysonParser.Models

open System


type IOperator = interface end
type ILiteral = interface end 


type BinaryOperator = 
    | Add 
    | Subtract
    interface IOperator

type NumericLiteral = 
    | IntLiteral of int
    | FloatLiteral of float32
    interface ILiteral

type StructLiteral = 
    | GuidLiteral of Guid
    | DateTimeLiteral of DateTime
    | TimeSpanLiteral of TimeSpan
    interface ILiteral

type Expr = 
    | Binary of (BinaryOperator * Expr * Expr)
    | Literal of ILiteral 
