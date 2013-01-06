LL1
===

A simple LL(1) test lexer and tokenizer for things of the form

```
list := [elements]

elements := element (, element)*

element := NAME | list

NAME = \d+\w+
```

This is just a practice out of the LL(1) Recrusive-Descent Lexer from Language Implementation Patterns by Terence Parr.  I wanted to do it in F# to utilize active patterns and just for kicks :)

The output for input

```
let sourceCode = @"[long name,b, 

  [c,second long name]
  
  ]"
```

Is

```
LBRACK,long name,COMMA,b,COMMA,LBRACK,c,COMMA,second long name,RBRACK,RBRACK,EOF
```

There is also a parser that validates the BNF 