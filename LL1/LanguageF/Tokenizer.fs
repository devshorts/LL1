module Tokenizer

open System
open System.Text.RegularExpressions
open Lexer

exception UnexpectedToken

type TokenType = 
    | Name
    | Comma 
    | LeftBracket 
    | RightBracket
    | Assignment
    | EOF

type Token = TokenType * String

let getTokenTypeName tokenType = 
    match tokenType with
        | TokenType.Name -> "NAME"
        | TokenType.Comma -> "COMMA"
        | TokenType.EOF -> "EOF"
        | TokenType.Assignment -> "="
        | TokenType.LeftBracket -> "LBRACK"
        | TokenType.RightBracket -> "RBRACK"

let getTokenName item = 
    let (tokenType, name) = item
    match tokenType with 
        | Name -> name
        | _ -> getTokenTypeName tokenType           
 
 
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

    member this.source = lexer.source

    // use the lookahead match to construct the next token
    member private this.computeTokens = 
        let createToken token char = (token, Option.get char)

        let rec tokenize' (currentLexer:Lexer) src = 

            // helper recursive functions
            let next item = tokenize' (currentLexer.consume()) (item::src) 

            // don't consume the next character, assume it's been consumed
            // so just use the current
            let nextWithCurrent lexer item = tokenize' lexer (item::src)

            // consume but ignore, so pass the accumulator through
            let ignoreToken() = tokenize' (currentLexer.consume()) src

            let input = currentLexer.current

            match input with
                | Letter _ -> 
                        let (newLexer, token) = this.getName currentLexer
                        let token = createToken TokenType.Name token
                        nextWithCurrent newLexer token

                | Comma _ -> next (createToken TokenType.Comma input)
                | LeftBracket _ -> next (createToken TokenType.LeftBracket input)
                | RightBracket _ -> next (createToken TokenType.RightBracket input)
                | Assignment _ -> next (createToken TokenType.Assignment input)
                | WhiteSpace _ -> ignoreToken()
                | EOF _ -> (createToken TokenType.EOF (Some(String.Empty)))::src
                | _ -> raise UnexpectedToken 

        List.rev (tokenize' lexer [])

    // if the lookahead match found a letter, build out the name (and include whitespace)
    member private this.getName (lexer:Lexer) : Lexer * String option = 
        let rec getName' (currentLexer:Lexer) tokens =
            let current = currentLexer.current
            match current with
                | Letter _ | WhiteSpace _ -> getName' (currentLexer.consume()) (current::tokens)
                | _ -> (currentLexer, tokens)

        let (newLexer, nameList) = getName' lexer []
        let name = List.fold(fun acc i -> (Option.get i) + acc) "" nameList
        (newLexer, Some(name.Trim()))
