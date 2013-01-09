module Parser

open Tokenizer
open System
open Utils

exception InvalidSyntax of string 

type TokenizerConsumer(tokenizer:Tokenizer) = 
    let mutable index = 0

    let mutable snapshotIndex = 0

    let tokens = tokenizer.tokens

    member this.source = tokenizer.source

    member internal this.consumeToken() = 
        let (token, value) = this.consume()
        token
        
    member internal this.snapshot() = 
        snapshotIndex <- index

    member internal this.relase() = 
        index <- snapshotIndex

    member internal this.consume() = 
        if index < ((List.length tokens) - 1) then
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
    let tokenizerConsumer = new TokenizerConsumer(tokenizer)

    let take item = tokenizerConsumer.consumeAndAdvance item

    let peek num = tokenizerConsumer.lookForward num

    let (|Alt|_|) func item = 
        try
            try
                tokenizerConsumer.snapshot()

                func()
                
                Some()
            with
                | _ -> None
        finally
            tokenizerConsumer.relase()
        
        
    member this.source = tokenizerConsumer.source

    member this.validate() = 
        let rList = fun () -> this.root this.list
        let rAssign = fun () -> this.root this.assign

        try
            match () with
                | Alt rList -> this.list()
                | Alt rAssign -> this.assign()
                | _ -> raise (InvalidSyntax ("Could not find any suitable form"))
            true
        with
            | Tokenizer.UnexpectedToken -> Console.WriteLine("There was an unexpected token found")
                                           false
            | InvalidSyntax(error) -> Console.WriteLine(error)
                                      false

    member private this.root func = 
        func()
        take TokenType.EOF |> ignore

    member private this.assign() = 
        this.list()

        take TokenType.Assignment |> ignore

        this.list()

    member private this.list() = 
        take TokenType.LeftBracket |> ignore

        this.elements()
        
        take TokenType.RightBracket |> ignore
        
    member private this.elements() = 
        this.element() |> ignore

        let rec getAllElements() = 
            match tokenizerConsumer.currentToken() with 
                | TokenType.Comma -> 
                                        take TokenType.Comma |> ignore
                                        this.element()
                                        getAllElements()
                | _ -> ()

        getAllElements()

    member private this.element() = 
        match tokenizerConsumer.currentToken() with 
            | TokenType.Name -> 
                let (nextToken, _) = peek 1

                match nextToken with
                    | TokenType.Assignment -> 
                        take TokenType.Name |> ignore
                        take TokenType.Assignment |> ignore
                        take TokenType.Name |> ignore
                    | _ -> 
                        take TokenType.Name |> ignore    
        
            | TokenType.LeftBracket -> this.list() 
        
            | _ -> raise (InvalidSyntax (String.Format("Invalid element syntax. Found {0}", Tokenizer.getTokenName (tokenizerConsumer.current()))))

    