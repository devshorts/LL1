LLk
===

A simple lexer and grammar validator for a lanague of the form.  It supports arbitrary lookahead as well as alternative testing via backtracking.

```
root := list | assign

list := [elements]   

assign := list = list

elements := element (, element)*

element := NAME | list | NAME = NAME

NAME = (\d+\w*)+
```

This is just a practice out of the LL(k) Recrusive-Descent Lexer from Language Implementation Patterns by Terence Parr.  I wanted to do it in F# to utilize active patterns and just for kicks :)

There is also a parser that validates the BNF 
                  

Sample application
====

```fsharp
open System
open System.Text.RegularExpressions
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
```

Outputs

```

Code valid! For: [long name,b,z, [ test = too, be = see, [third list]]] = [secondlist]

Could not find any suitable form
The code is invalid! For: [long name] = [foo, [xyz, test]
```