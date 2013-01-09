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