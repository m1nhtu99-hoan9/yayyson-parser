module YayysonParser.Parsers

open System
open FParsec

open Models


let internal createExprParser<'TResult> (pExprContent: Parser<'TResult, unit>) = 
    between (skipString "${") (skipChar '}') pExprContent

let internal runExprParser<'TResult> (pExprContent: Parser<'TResult, unit>) (streamName: string) (str: string) = 
    runParserOnString (createExprParser pExprContent) () streamName str

let internal endOfExprContent = nextCharSatisfies (fun c -> Array.contains c [| ' '; '}'; '\n'; '\t' |]) 

let internal pNumericExpr = 
    let options = NumberLiteralOptions.DefaultFloat 
                  ||| NumberLiteralOptions.DefaultInteger 

    numberLiteral options "numeric value" 
    |>> fun n -> 
            if n.IsInteger
            then NumericLiteral.IntLiteral <| int32 n.String
            else NumericLiteral.FloatLiteral <| float32 n.String
    .>> skipMany (skipChar ' ')


let pGuid : Parser<Guid, unit> = 
    let _pEmpty = pstring "Empty" >>% Guid.Empty
    let _pNew = pstring "NewGuid" >>% Guid.NewGuid ()
    let _pValue = ( 
        pstring "Parse" 
        >>. skipChar ' '
        >>. many (hex <|> pchar '-')
        |>> fun xs -> String.Join (String.Empty, xs)
        |>> Guid.Parse 
    )

    skipString "Guid." >>. choice [ _pEmpty; _pNew; _pValue ] 


let private _createTimeSpanPartParser (allowedParts: string) : Parser<(TimeSpan * char option), unit> = 
    let inline interceptPChar (character : char) pChar = 
        if allowedParts.Contains character
        then pChar 
        else fail <| String.Format ("'{0}' is not in '{1}'.", char, allowedParts)

    let pEmpty = endOfExprContent >>% (TimeSpan.Zero, None)

    if String.IsNullOrWhiteSpace allowedParts then
        pEmpty
    else 
        pEmpty <|> (
            pfloat >>= fun fl -> 
                choice [
                    pchar 'd' >>% (TimeSpan.FromDays fl, Some 'd') |> interceptPChar 'd'
                    pchar 'h' >>% (TimeSpan.FromHours fl, Some 'h') |> interceptPChar 'h'
                    pchar 'm' >>% (TimeSpan.FromMinutes fl, Some 'm') |> interceptPChar 'm'
                    pchar 's' >>% (TimeSpan.FromSeconds fl, Some 's') |> interceptPChar 's'
                ] 
                .>> optional (skipChar '_')
        )

let pTimeSpan : Parser<TimeSpan, unit> =        
    fun stream ->
        let r0 = endOfExprContent stream

        if r0.Status = ReplyStatus.Ok then
            Reply (r0.Status, r0.Error)
        else
            let mutable reply = Reply TimeSpan.Zero
            let mutable remainingParts = "dhms"
            let mutable currentPartState = Some 'd'

            while currentPartState.IsSome do
                let r = stream |> _createTimeSpanPartParser remainingParts
                
                match r.Status with 
                | ReplyStatus.Ok -> 
                    match r.Result with
                    | (d, Some 'd') ->
                        currentPartState <- Some 'd'
                        remainingParts <- "hms"
                        reply.Result <- reply.Result.Add d
                    | (h, Some 'h') ->
                        currentPartState <- Some 'h'
                        remainingParts <- "ms"
                        reply.Result <- reply.Result.Add h
                    | (m, Some 'm') ->
                        currentPartState <- Some 'm'
                        remainingParts <- "s"
                        reply.Result <- reply.Result.Add m
                    | (s, Some 's') ->
                        currentPartState <- Some 's'
                        remainingParts <- String.Empty
                        reply.Result <- reply.Result.Add s
                    | (v, None) -> 
                        currentPartState <- None
                        reply.Error <- r.Error
                    | _ -> raise <| new InvalidOperationException "Unreachable parser result state"
                | _ -> 
                    currentPartState <- None
                    reply <- Reply (r.Status, r.Error)

            reply


let pDateTime : Parser<DateTime, unit> =
    let _pMinValue = skipString "MinValue" >>% DateTime.MinValue
    let _pMaxValue = skipString "MaxValue" >>% DateTime.MaxValue
    let _pNow = skipString "Now" >>% DateTime.Now
    let _pUtcNow = skipString "UtcNow" >>% DateTime.UtcNow
    
    skipString "DateTime." >>. choice [ _pMinValue; _pMaxValue; _pNow; _pUtcNow; ]


let pFullExpr : Parser<Expr, unit> = 
    let pDateTimeLiteral = 
        pDateTime .>> optional spaces 
        |>> fun d -> StructLiteral.DateTimeLiteral d :> ILiteral |> Expr.Literal

    let pTimeSpanLiteral = 
        pTimeSpan .>> optional spaces 
        |>> fun t -> StructLiteral.TimeSpanLiteral t :> ILiteral |> Expr.Literal

    let pGuidLiteral = 
        pGuid .>> optional spaces 
        |>> fun id -> StructLiteral.GuidLiteral id :> ILiteral |> Expr.Literal
    
    let pNumber = pNumericExpr |>> fun n -> n :> ILiteral |> Expr.Literal
    
    let inline createBinaryInfixOp (operatorString: string, 
                                    precedence: int, 
                                    operator: BinaryOperator) : InfixOperator<Expr, unit, unit> =
        InfixOperator (operatorString, spaces, precedence, Associativity.Left, fun x y -> Expr.Binary (operator, x, y))

    let ops = OperatorPrecedenceParser<Expr, _, _>()

    ops.AddOperator <| createBinaryInfixOp ("+", 1, BinaryOperator.Add)
    ops.AddOperator <| createBinaryInfixOp ("-", 2, BinaryOperator.Subtract)

    ops.TermParser <- (choice [        
        attempt pTimeSpanLiteral
        attempt pDateTimeLiteral 
        attempt pGuidLiteral 
        pNumber
    ])

    createExprParser ops.ExpressionParser
