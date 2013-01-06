module Parser

open Tokenizer
open System
open Utils

exception InvalidSyntax of string 

type ParserBase(tokenizer:Tokenizer) = 
    let mutable index = 0

    let tokens = List.toArray tokenizer.tokens

    member internal this.consume() = 
        index <- index + 1
        this.current()

    member internal this.current() = 
        tokens.[index]

    member internal this.lookForward n = 
        tokens.[index + n]

    member internal this.consumeAndAdvance token = 
        let invalidEx token curr = 
            InvalidSyntax (String.Format("Expecting token type {0} but found {1}", (Tokenizer.getTokenName token), (Tokenizer.getTokenName (this.current()))))

        if this.current() = token then
            this.consume()
        else
            match token with 
                | Name _ -> 
                    match this.current() with 
                        | Name _ -> this.consume()
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
            match this.current() with 
                | TokenType.Comma -> this.consumeAndAdvance TokenType.Comma |> ignore
                                     this.element()
                                     getAllElements()
                | _ -> ()

        getAllElements()

    member private this.element() = 
        match this.current() with 
            | TokenType.Name _ -> 
                match this.lookForward 1 with
                    | TokenType.Assignment _ -> 
                        this.consumeAndAdvance (TokenType.Name("")) |> ignore
                        this.consumeAndAdvance TokenType.Assignment |> ignore
                        this.consumeAndAdvance (TokenType.Name("")) |> ignore
                    | _ -> 
                        this.consumeAndAdvance (TokenType.Name("")) |> ignore    
            | TokenType.LeftBracket -> this.list() 
            | _ -> raise (InvalidSyntax (String.Format("Invalid element syntax. Found {0}", Tokenizer.getTokenName (this.current()))))

    