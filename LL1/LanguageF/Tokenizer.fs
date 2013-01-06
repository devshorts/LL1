module Tokenizer

open System
open System.Text.RegularExpressions
open Lexer


exception UnexpectedToken

type TokenType = 
    | Name of String
    | Comma 
    | LeftBracket 
    | RightBracket
    | EOF

       
type Tokenizer(lexer:Lexer) =
    // the lookahead match

    let (|Letter|Comma|LeftBracket|RightBracket|EOF|WhiteSpace|) i = 
        if Option.isNone i then
            EOF
        else 
            let input = Option.get i

            if input = "," then
                Comma
            else if input = "[" then 
                LeftBracket
            else if input = "]" then
                RightBracket
            else if String.IsNullOrWhiteSpace input then
                WhiteSpace
            else if Regex.IsMatch(input, "[a-z]|[A-Z]|[0-9]") then 
                Letter
            else 
                raise UnexpectedToken                

    // use the lookahead match to construct the next token
    member this.tokens = 
        let rec tokenize' input src = 

            // helper recursive functions
            let next item = tokenize' (lexer.consume()) (item::src)
            let nextWithCurrent item = tokenize' lexer.current (item::src)
            let ignoreToken() = tokenize' (lexer.consume()) src

            match input with
                | Letter -> nextWithCurrent (TokenType.Name(this.getName()))
                | Comma -> next TokenType.Comma
                | LeftBracket -> next TokenType.LeftBracket
                | RightBracket -> next TokenType.RightBracket
                | WhiteSpace -> ignoreToken()
                | EOF -> TokenType.EOF::src

        List.rev (tokenize' lexer.current [])

    // if the lookahead match found a letter, build out the name (and include whitespace)
    member this.getName() = 
        let rec getName' current tokens =
            match current with
                | Letter | WhiteSpace -> getName' (lexer.consume()) (current::tokens)
                | _ -> tokens    

        List.fold(fun acc i -> (Option.get i) + acc) "" (getName' lexer.current [])
