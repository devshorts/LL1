LLk
===

A simple LL(k) test lexer and tokenizer for things of the form

```
list := [elements]

elements := element (, element)*

element := NAME | list | NAME = NAME

NAME = \d+\w+
```

This is just a practice out of the LL(k) Recrusive-Descent Lexer from Language Implementation Patterns by Terence Parr.  I wanted to do it in F# to utilize active patterns and just for kicks :)

There is also a parser that validates the BNF 