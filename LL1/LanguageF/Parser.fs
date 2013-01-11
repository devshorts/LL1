module Parser

open TokenizerConsumer
open Tokenizer
open System
open Utils

(* Validates syntax *)

type ParseTree = 
    | List of ParseTree list
    | Assignment of ParseTree * ParseTree
    | Token of Token
    | Empty

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
        with
            | Tokenizer.UnexpectedToken -> Console.WriteLine("There was an unexpected token found")
                                           ParseTree.Empty
            | InvalidSyntax(error) -> Console.WriteLine(error)
                                      ParseTree.Empty

    member private this.root func = 
        func() |> ignore
        take TokenType.EOF |> ignore

    member private this.assign() = 
        let left = this.list()

        take TokenType.Assignment |> ignore

        let right = this.list()

        ParseTree.Assignment(left, right)

    member private this.list() = 
        take TokenType.LeftBracket |> ignore

        let tokens = this.elements()
        
        take TokenType.RightBracket |> ignore

        ParseTree.List(tokens)
        
    member private this.elements() = 
        let element = this.element() 

        let rec getAllElements l = 
            match tokenizerConsumer.currentToken() with 
                | TokenType.Comma -> 
                                        take TokenType.Comma |> ignore
                                        let elem = this.element()
                                        getAllElements (elem::l)
                | _ -> List.rev l

        element::(getAllElements [])

    member private this.element()  = 
        match tokenizerConsumer.currentToken() with 
            | TokenType.Name -> 
                let (nextToken, _) = peek 1

                match nextToken with
                    | TokenType.Assignment -> 
                        let left = take TokenType.Name 

                        take TokenType.Assignment |> ignore

                        let right = take TokenType.Name 
                        
                        ParseTree.Assignment(ParseTree.Token left, ParseTree.Token right)

                    | _ -> ParseTree.Token(take TokenType.Name)
        
            | TokenType.LeftBracket -> this.list() 
        
            | _ -> raise (InvalidSyntax (String.Format("Invalid element syntax. Found {0}", Tokenizer.getTokenName (tokenizerConsumer.current()))))

    