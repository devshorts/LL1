// Learn more about F# at http://fsharp.net

open System
open System.Text.RegularExpressions
open Lexer
open Parser
open Tokenizer

let invalidSource = @"[long name,b,
z, [

ffff asdf = zappos = , test]]"

let sourceCode = @"[long name,b,z, [ test]]"

let tokenizer = new Tokenizer(new Lexer(invalidSource))

let validTokenizer = new Tokenizer(new Lexer(sourceCode))

let invalidParser = new Parser(tokenizer)

let validParser = new Parser(validTokenizer)

for p in [validParser; invalidParser] do
    Console.WriteLine()

    if p.validate() then
        Console.WriteLine("Code valid!")
    else
        Console.WriteLine("The code is invalid")

Console.ReadKey() |> ignore