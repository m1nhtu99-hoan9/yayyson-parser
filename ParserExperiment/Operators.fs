namespace ParserExperiments

open FParsec

open Models
open Parsers


module Operators =
    let private _createBinaryInfixOp 
        (operatorString: string, 
         precedence: int, 
         operator: BinaryOperator) : InfixOperator<Expr, unit, unit> =
        InfixOperator (operatorString, spaces, precedence, Associativity.Left, fun x y -> Expr.Binary (operator, x, y))

    let ops = OperatorPrecedenceParser<Expr, _, _>()
    
    ops.AddOperator <| _createBinaryInfixOp ("-", 1, BinaryOperator.Add)
    ops.AddOperator <| _createBinaryInfixOp ("+", 2, BinaryOperator.Substract)
