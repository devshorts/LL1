module Parser

open Tokenizer
open System
open Utils

exception InvalidSyntax of string 

type ParserBase(tokenizer:Tokenizer) = 
    
    let lookahead = 
        let rec lookahead' (tokens) =
            seq{
                match tokens with 
                    | h::[] -> yield h
                    | h::t -> yield h
                              yield! lookahead' t
            }

        let sequence = lookahead' tokenizer.tokens
        let enumerator = sequence.GetEnumerator()
        enumerator.MoveNext() |> ignore
        enumerator

    member internal this.consume() = lookahead.MoveNext()

    member internal this.current() = lookahead.Current

    member internal this.select token = 
        if this.current() = token then
            this.consume()
        else
            match token with 
                | Name _ -> 
                    match this.current() with 
                        | Name _ -> this.consume()
                        | _ -> raise (InvalidSyntax (String.Format("Expecting token type {0} but found {1}", (Tokenizer.getTokenName token), (Tokenizer.getTokenName (this.current())))))
                | _ -> raise (InvalidSyntax (String.Format("Expecting token type {0} but found {1}", (Tokenizer.getTokenName token), (Tokenizer.getTokenName (this.current())))))


(* Validates syntax *)

type Parser(tokenizer:Tokenizer) = 
    inherit ParserBase(tokenizer)

    member this.list() = 
        this.select TokenType.LeftBracket |> ignore
        this.elements()
        this.select TokenType.RightBracket |> ignore
        
    member private this.elements() = 
        this.element() |> ignore

        let rec getAllElements() = 
            match this.current() with 
                | TokenType.Comma -> this.select TokenType.Comma |> ignore
                                     this.element()
                                     getAllElements()
                | _ -> ()

        getAllElements()

    member private this.element() = 
        match this.current() with 
            | TokenType.Name _ -> this.select (TokenType.Name("")) |> ignore
            | TokenType.LeftBracket -> this.list() 
            | _ -> raise (InvalidSyntax (String.Format("Invalid element syntax. Found {0}", Tokenizer.getTokenName (this.current()))))

    