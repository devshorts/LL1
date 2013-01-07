module Lexer

open System

type Lexer(source:String) =     
    let _current = 
                if source.Length > 0 then
                    Some (((source.Chars 0)).ToString())
                else 
                    None

    member this.current = _current

    member this.consume() = new Lexer(source.Substring 1)        

    member this.source = source