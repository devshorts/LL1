// Learn more about F# at http://fsharp.net

open System
open Lexer
open Parser
open Tokenizer

let invalidSource = @"[long name] = [foo, [xyz, test]"

let sourceCode = @"[long name,b,z, [ test = too, be = see, [third list]]] = [secondlist]"

let tokenizer = new Tokenizer(new Lexer(invalidSource))

let validTokenizer = new Tokenizer(new Lexer(sourceCode))

let invalidParser = new Parser(tokenizer)

let validParser = new Parser(validTokenizer)

for p in [validParser; invalidParser] do
    Console.WriteLine()

    if p.validate() then
        Console.WriteLine("Code valid! For: {0}", p.source)
    else
        Console.WriteLine("The code is invalid! For: {0}", p.source)

Console.ReadKey() |> ignore