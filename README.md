LL1
===

A simple LL(1) test lexer and tokenizer for things of the form

```
list := [token|list]

token := \d+\w+
```

This is just a practice out of the LL(1) Recrusive-Descent Lexer from Language Implementation Patterns by Terence Parr.  I wanted to do it in F# to utilize active patterns and just for kicks :)