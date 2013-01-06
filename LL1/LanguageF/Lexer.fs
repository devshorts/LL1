module Lexer

open System

type Lexer(source:String) = 
    let mutable index = 0
    let mutable c = None
    let getNextChar() = 
        if index >= source.Length then
            None
        else 
            let c = source.Chars index
            index <- index + 1
            Some(c.ToString())
    do
        c <- getNextChar()

    member this.current = c
    member this.consume() = 
        c <- getNextChar()
        c