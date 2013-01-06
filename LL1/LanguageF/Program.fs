// Learn more about F# at http://fsharp.net

open System
open System.Text.RegularExpressions
open Lexer
open Tokenizer

(* 

Tests for LL(1) simple lexer/tokenizer

*)

let sourceCode = @"[long name,b, 

  [c,second long name]
  
  ]"

let tokenizer = new Tokenizer(new Lexer(sourceCode))

let rec printList l = 
    let getName item = 
        match item with
            | TokenType.Name a -> a
            | TokenType.Comma -> "COMMA"
            | TokenType.EOF -> "EOF"
            | TokenType.LeftBracket -> "LBRACK"
            | TokenType.RightBracket -> "RBRACK"

    match l with 
        | [] -> Console.Write("no tokens")
        | h::[] -> Console.Write("{0}", getName h)  
        | h::t -> Console.Write("{0},", getName h)
                  printList t                 

printList (tokenizer.tokens)

Console.ReadKey() |> ignore