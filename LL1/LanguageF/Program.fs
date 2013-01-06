// Learn more about F# at http://fsharp.net

open System
open System.Text.RegularExpressions
open Lexer
open Parser
open Tokenizer

(* 

Tests for LL(1) simple lexer/tokenizer

*)

let sourceCode = @"[long name,b, 

  [c,second long name
  
  ]"

let tokenizer = new Tokenizer(new Lexer(sourceCode))

let rec printList l = 
    match l with 
        | [] -> Console.Write("no tokens")
        | h::[] -> Console.Write("{0}", Tokenizer.getTokenName h)  
        | h::t -> Console.Write("{0},", Tokenizer.getTokenName h)
                  printList t                 

printList (tokenizer.tokens)

let parser = new Parser(tokenizer)

parser.list() |> ignore

Console.WriteLine("Code valid!") |> ignore

Console.ReadKey() |> ignore