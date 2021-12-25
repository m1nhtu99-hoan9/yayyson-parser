module ParserExperiments.Parsers

open System
open FParsec

open Models


let runPExpr (pexpr: Parser<'TResult, unit>) (streamName: string) (str: string) = 
    let pExprContent = skipString "${" >>. manyCharsTill anyChar (skipChar '}')
    let r0 = run pExprContent str 
    match r0 with 
    | ParserResult.Success (exprContent, _, _) -> runParserOnString pexpr () streamName exprContent
    | ParserResult.Failure (msg, err, _) -> ParserResult.Failure (msg, err, ())


let internal pNumberLiteral : Parser<NumberLiteral, obj> =  
    let supportedOptions = 
        NumberLiteralOptions.DefaultFloat 
        ||| NumberLiteralOptions.DefaultInteger
    numberLiteral supportedOptions "number"



let private _pTimeSpanPart (partAliases: string) : Parser<(TimeSpan * char option), unit> = 
    fun stream -> 
        if String.IsNullOrWhiteSpace partAliases then
            let r0 = eof stream            
            match r0.Status with
            | ReplyStatus.Ok -> Reply <| (TimeSpan.Zero, None)
            | _ -> Reply (r0.Status, (TimeSpan.Zero, None), r0.Error)
        else
            if stream.IsEndOfStream then
                Reply <| (TimeSpan.Zero, None)
            else
                let r0 = pfloat stream
                match r0.Status with
                | ReplyStatus.Ok ->
                    let r1 = stream |> skipped (skipAnyOf partAliases)
                    match r1.Result with
                    | "d" -> Reply <| (TimeSpan.FromDays r0.Result, Some 'd')
                    | "h" -> Reply <| (TimeSpan.FromHours r0.Result, Some 'h')
                    | "m" -> Reply <| (TimeSpan.FromMinutes r0.Result, Some 'm')
                    | "s" -> Reply <| (TimeSpan.FromSeconds r0.Result, Some 's')
                    | _ -> Reply (r1.Status, (TimeSpan.Zero, None), r1.Error)
                | _ -> Reply (r0.Status, (TimeSpan.Zero, None), r0.Error)


let pTimeSpan : Parser<TimeSpan, unit> =        
    fun stream ->
        let r0 = eof stream

        if r0.Status = ReplyStatus.Ok then
            Reply (r0.Status, r0.Error)
        else
            let mutable reply = Reply TimeSpan.Zero
            let mutable remainingParts = "dhms"
            let mutable currentPartState = Some 'd'

            while currentPartState.IsSome do
                let r = stream |> _pTimeSpanPart remainingParts
                let (value, partAlias) = r.Result
                match (partAlias, r.Status, value) with
                | (None, ReplyStatus.Ok, _) -> ()
                | (None, _, _) -> 
                    reply <- Reply (r.Status, r.Error)
                | (Some 'd', _, d) ->
                    remainingParts <- "hms"
                    reply.Result <- reply.Result.Add d
                | (Some 'h', _, h) ->
                    remainingParts <- "ms"
                    reply.Result <- reply.Result.Add h
                | (Some 'm', _, m) ->
                    remainingParts <- "s"
                    reply.Result <- reply.Result.Add m
                | (Some 's', _, s) ->
                    remainingParts <- String.Empty
                    reply.Result <- reply.Result.Add s
                | _ -> ()

            reply


let pGuid : Parser<Guid, unit> = 
    let _pEmpty = pstring "Empty" >>% Guid.Empty
    let _pNew = pstring "NewGuid" >>% Guid.NewGuid ()
    let _pValue = ( pstring "Parse" 
        >>. skipChar ' '
        >>. manyCharsTill anyChar eof 
        |>> Guid.Parse )

    skipString "Guid." >>. choice [ _pEmpty; _pNew; _pValue ] 



let pIdentifier : Parser<Expr, unit> = many1Chars (letter <|> digit) |>> Expr.Identifer
