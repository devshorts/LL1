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
    | Assignment
    | EOF

let getTokenName item = 
    match item with
        | TokenType.Name name -> name
        | TokenType.Comma -> "COMMA"
        | TokenType.EOF -> "EOF"
        | TokenType.Assignment -> "="
        | TokenType.LeftBracket -> "LBRACK"
        | TokenType.RightBracket -> "RBRACK"
 
 
let rec printList l = 
    match l with 
        | [] -> Console.Write("no tokens")
        | h::[] -> Console.Write("{0}", getTokenName h)  
        | h::t -> Console.Write("{0},", getTokenName h)
                  printList t                 

type Tokenizer(lexer:Lexer) as this =
   
    let test predicate character =
        if Option.isNone character then
                None
        else
            let input = Option.get character
            if predicate input then 
                Some character
            else 
                None
            
    let (|EOF|_|) character = 
        if Option.isNone character then
            Some character
        else
            None
    
    let (|Letter|_|) character = 
        test (fun input -> Regex.IsMatch(input, "[a-z]|[A-Z]|[0-9]")) character

    let (|Comma|_|) character = 
        test (fun input -> input = ",") character

    let (|LeftBracket|_|) character = 
        test (fun input -> input = "[") character

    let (|RightBracket|_|) character = 
        test (fun input -> input = "]") character

    let (|Assignment|_|) character = 
        test (fun input -> input = "=") character

    let (|WhiteSpace|_|) character = 
        test (fun input -> String.IsNullOrWhiteSpace input) character

    let mutable computedTokens = []

    do
        computedTokens <- this.computeTokens
                                  
    member this.tokens = computedTokens

    // use the lookahead match to construct the next token
    member private this.computeTokens = 
        let rec tokenize' input src = 

            // helper recursive functions
            let next item = tokenize' (lexer.consume()) (item::src)
            let nextWithCurrent item = tokenize' lexer.current (item::src)
            let ignoreToken() = tokenize' (lexer.consume()) src

            match input with
                | Letter _ -> nextWithCurrent (TokenType.Name(this.getName()))
                | Comma _ -> next TokenType.Comma
                | LeftBracket _ -> next TokenType.LeftBracket
                | RightBracket _ -> next TokenType.RightBracket
                | Assignment _ -> next TokenType.Assignment
                | WhiteSpace _ -> ignoreToken()
                | EOF _ -> TokenType.EOF::src
                | _ -> raise UnexpectedToken 

        List.rev (tokenize' lexer.current [])

    // if the lookahead match found a letter, build out the name (and include whitespace)
    member private this.getName() = 
        let rec getName' current tokens =
            match current with
                | Letter _ | WhiteSpace _ -> getName' (lexer.consume()) (current::tokens)
                | _ -> tokens    

        let name = List.fold(fun acc i -> (Option.get i) + acc) "" (getName' lexer.current [])
        name.Trim()
