module Parser

open Tokenizer
open System
open Utils

exception InvalidSyntax of string 

type ParserBase(tokenizer:Tokenizer) = 
    let mutable index = 0

    let tokens = List.toArray tokenizer.tokens

    member this.source = tokenizer.source

    member internal this.consumeToken() = 
        let (token, value) = this.consume()
        token

    member internal this.consume() = 
        index <- index + 1
        this.current()

    member internal this.current() = 
        tokens.[index]

    member internal this.currentToken() = 
        let (token, value) = this.current()
        token

    member internal this.lookForward n = 
        tokens.[index + n]

    member internal this.consumeAndAdvance token = 
        let invalidEx token curr = 
            InvalidSyntax (String.Format("Expecting token type {0} but found {1}", (Tokenizer.getTokenTypeName token), (Tokenizer.getTokenName (this.current()))))

        let (currentToken, value) = this.current()
        if currentToken = token then
            this.consumeToken()
        else
            match token with 
                | Name -> 
                    match currentToken with 
                        | Name -> this.consumeToken()
                        | _ -> raise (invalidEx token (this.current()))
                | _ -> raise (invalidEx token (this.current()))


(* Validates syntax *)

type Parser(tokenizer:Tokenizer) = 
    inherit ParserBase(tokenizer)

    member this.validate() = 
        try
            this.list()
            true
        with
            | Tokenizer.UnexpectedToken -> Console.WriteLine("There was an unexpected token found")
                                           false
            | InvalidSyntax(error) -> Console.WriteLine(error)
                                      false

    member private this.list() = 
        this.consumeAndAdvance TokenType.LeftBracket |> ignore
        this.elements()
        this.consumeAndAdvance TokenType.RightBracket |> ignore
        
    member private this.elements() = 
        this.element() |> ignore

        let rec getAllElements() = 
            match this.currentToken() with 
                | TokenType.Comma -> 
                                        this.consumeAndAdvance TokenType.Comma |> ignore
                                        this.element()
                                        getAllElements()
                | _ -> ()

        getAllElements()

    member private this.element() = 
        match this.currentToken() with 
            | TokenType.Name -> 
                let (nextToken, _) = this.lookForward 1
                match nextToken with
                    | TokenType.Assignment -> 
                        this.consumeAndAdvance TokenType.Name |> ignore
                        this.consumeAndAdvance TokenType.Assignment |> ignore
                        this.consumeAndAdvance TokenType.Name |> ignore
                    | _ -> 
                        this.consumeAndAdvance TokenType.Name |> ignore    
            | TokenType.LeftBracket -> this.list() 
            | _ -> raise (InvalidSyntax (String.Format("Invalid element syntax. Found {0}", Tokenizer.getTokenName (this.current()))))

    