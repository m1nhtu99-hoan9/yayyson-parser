module ParserExperiments.Operators

open FParsec

open Models
open Parsers


let private _createBinaryInfixOp (operatorString: string, 
                                  precedence: int, 
                                  operator: BinaryOperator) : InfixOperator<Expr, unit, unit> =
    InfixOperator (operatorString, spaces, precedence, Associativity.Left, fun x y -> Expr.Binary (operator, x, y))

let ops = OperatorPrecedenceParser<Expr, _, _>()

ops.AddOperator <| _createBinaryInfixOp ("-", 1, BinaryOperator.Add)
ops.AddOperator <| _createBinaryInfixOp ("+", 2, BinaryOperator.Subtract)

let expr = ops.ExpressionParser

ops.TermParser <- createExprParser (choice [
    pGuid |>> Expr.GuidLiteral
    pTimeSpan |>> Expr.TimeSpanLiteral
    pfloat |>> Expr.FloatLiteral
    pint32 |>> Expr.IntLiteral
])
