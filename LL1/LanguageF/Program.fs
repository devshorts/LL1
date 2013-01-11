// Learn more about F# at http://fsharp.net

open System
open Lexer
open Parser
open Tokenizer
open TokenizerConsumer

let invalidSource = @"[long name, = [foo]"

let sourceCode = @"[long name,b,z, [ test = too, be = see, [third list]]] = [secondlist]"

let tokenizer = new Tokenizer(new Lexer(invalidSource))

let validTokenizer = new Tokenizer(new Lexer(sourceCode))

let invalidParser = new Parser(tokenizer)

let validParser = new Parser(validTokenizer)

for p in [ invalidParser; validParser ] do
    Console.WriteLine()

    let tree = p.validate()
    match tree with
        | ParseTree.Empty -> Console.WriteLine("The code is invalid! For: {0}", p.source)
        | _ -> Console.WriteLine("Code valid!")
               printTree tree
        

Console.ReadKey() |> ignore