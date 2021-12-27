module ParserExperiments.Parsers

open System
open FParsec

open Models


let internal createExprParser<'TResult> (pExprContent: Parser<'TResult, unit>) = 
    between (skipString "${") (skipChar '}') pExprContent

let internal runExprParser<'TResult> (pExprContent: Parser<'TResult, unit>) (streamName: string) (str: string) = 
    runParserOnString (createExprParser pExprContent) () streamName str

let internal endOfExprContent = followedBy <| pchar '}'
     


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


let private _pTimeSpanPart (partAliases: string) : Parser<(TimeSpan * char option), unit> = 
    let inline createPChar (character : char) pChar = 
        if partAliases.Contains character
        then pChar 
        else fail $"'{char}' is not in '{partAliases}'."

    let pEmpty = endOfExprContent >>% (TimeSpan.Zero, None)

    if String.IsNullOrWhiteSpace partAliases then
        pEmpty
    else 
        pEmpty <|> (
            pfloat >>= fun fl -> 
                choice [
                    pchar 'd' >>% (TimeSpan.FromDays fl, Some 'd') |> createPChar 'd'
                    pchar 'h' >>% (TimeSpan.FromHours fl, Some 'h') |> createPChar 'h'
                    pchar 'm' >>% (TimeSpan.FromMinutes fl, Some 'm') |> createPChar 'm'
                    pchar 's' >>% (TimeSpan.FromSeconds fl, Some 's') |> createPChar 's'
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
                let r = stream |> _pTimeSpanPart remainingParts
                
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
    let _pMinValue = pstring "MinValue" >>% DateTime.MinValue
    let _pMaxValue = pstring "MaxValue" >>% DateTime.MaxValue
    let _pNow = pstring "Now" >>% DateTime.Now
    let _pUtcNow = pstring "UtcNow" >>% DateTime.UtcNow
    
    skipString "DateTime." >>. choice [ _pMinValue; _pMaxValue; _pNow; _pUtcNow; ] 


let pIdentifierExpr : Parser<Expr, unit> = many1Chars (letter <|> digit) |>> Expr.Identifer


let pFullExpr : Parser<Expr, unit> = 
    let pDateTimeLiteral = pDateTime .>> optional spaces |>> Expr.DateTimeLiteral
    let pTimeSpanLiteral = pTimeSpan .>> optional spaces |>> Expr.TimeSpanLiteral
    let pGuidLiteral = pGuid .>> optional spaces |>> Expr.GuidLiteral
    let pFloatLiteral = pfloat .>> optional spaces |>> Expr.FloatLiteral
    let pIntLiteral = pint32 .>> optional spaces |>> Expr.IntLiteral

    let inline createBinaryInfixOp (operatorString: string, 
                                    precedence: int, 
                                    operator: BinaryOperator) : InfixOperator<Expr, unit, unit> =
        InfixOperator (operatorString, spaces, precedence, Associativity.Left, fun x y -> Expr.Binary (operator, x, y))

    let ops = OperatorPrecedenceParser<Expr, _, _>()

    ops.AddOperator <| createBinaryInfixOp ("+", 1, BinaryOperator.Add)
    ops.AddOperator <| createBinaryInfixOp ("-", 2, BinaryOperator.Subtract)

    ops.TermParser <- choice [
        attempt pTimeSpanLiteral
        attempt pDateTimeLiteral 
        attempt pGuidLiteral 
        attempt pIntLiteral
        pFloatLiteral
    ]

    createExprParser ops.ExpressionParser
