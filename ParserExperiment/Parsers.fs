module ParserExperiments.Parsers

open System
open FParsec

open Models

let internal createExprParser<'TResult> (pExprContent: Parser<'TResult, unit>) = 
    between (skipString "${") (skipChar '}') pExprContent

let internal runExprParser<'TResult> (pExprContent: Parser<'TResult, unit>) (streamName: string) (str: string) = 
    runParserOnString (createExprParser pExprContent) () streamName str

let internal endOfExprContent = followedBy <| pchar '}' <|> eof    


let pGuid : Parser<Guid, unit> = 
    let _pEmpty = pstring "Empty" >>% Guid.Empty
    let _pNew = pstring "NewGuid" >>% Guid.NewGuid ()
    let _pValue = ( pstring "Parse" 
        >>. skipChar ' '
        >>. manyCharsTill anyChar endOfExprContent
        |>> Guid.Parse )

    skipString "Guid." >>. choice [ _pEmpty; _pNew; _pValue ] 


let private _pTimeSpanPart (partAliases: string) : Parser<(TimeSpan * char option), unit> = 
    fun stream -> 
        if String.IsNullOrWhiteSpace partAliases then
            let r0 = stream |> endOfExprContent           
            match r0.Status with
            | ReplyStatus.Ok -> Reply <| (TimeSpan.Zero, None)
            | _ -> Reply (r0.Status, (TimeSpan.Zero, None), r0.Error)
        else
            if (stream |> endOfExprContent).Status = ReplyStatus.Ok then
                Reply <| (TimeSpan.Zero, None)
            else
                let r0 = pfloat stream
                match r0.Status with
                | ReplyStatus.Ok ->
                    let r1 = stream |> (skipped (skipAnyOf partAliases) .>>? optional (skipChar '_'))

                    match r1.Result with
                    | "d" -> Reply <| (TimeSpan.FromDays r0.Result, Some 'd')
                    | "h" -> Reply <| (TimeSpan.FromHours r0.Result, Some 'h')
                    | "m" -> Reply <| (TimeSpan.FromMinutes r0.Result, Some 'm')
                    | "s" -> Reply <| (TimeSpan.FromSeconds r0.Result, Some 's')
                    | _ -> Reply (r1.Status, (TimeSpan.Zero, None), r1.Error)
                | _ -> Reply (r0.Status, (TimeSpan.Zero, None), r0.Error)

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
                let (value, partAlias) = r.Result
                currentPartState <- partAlias

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



let pDateTime : Parser<DateTime, unit> =
    let _pMinValue = pstring "MinValue" >>% DateTime.MinValue
    let _pMaxValue = pstring "MaxValue" >>% DateTime.MaxValue
    let _pNow = pstring "Now" >>% DateTime.Now
    let _pUtcNow = pstring "UtcNow" >>% DateTime.UtcNow
    let _pValue = ( pstring "Parse" 
        >>. skipChar ' '
        >>. manyCharsTill anyChar eof 
        |>> Guid.Parse )

    skipString "DateTime." >>. choice [ _pMinValue; _pMaxValue; _pNow; _pUtcNow; ] 


let pIdentifierExpr : Parser<Expr, unit> = many1Chars (letter <|> digit) |>> Expr.Identifer
