# Список тестов

## Требования к синтаксическому анализатору

1. Выражения могут содержать числа, операции, вызовы функций и скобки
2. Операторы могут иметь разный приоритет: например, у "*" приоритет выше, чем у "+"
3. Операторы могут иметь одинаковый приоритет: например, "*", "/", "%" имеют равный приоритет
4. Арифметические операции являются левоассоциативными: `3 * 7 / 3` означает `(3 * 7) / 3`, а не `3 * (7 / 3)`

## Список сценариев

- [x] Разбор write без завершающего разделителя: `write ("hello")`
- [x] Разбор write без с завершающим разделителем: `write ("hello"); `
- [x] Разбор write со списком выражений: `write ("hello", "asd"); `
- [ ] Разбор if без else: ` if (True) {} `
- [ ] Разбор сложения и вычитания: `if (1 + 4 - 2 == 3) {};`
- [ ] Разбор вызова функций: 
`x = min(7, 10 - 4)`
`x = max(1, 3)`
`x = abs(-4)`
- [ ] Учёт левой ассоциативности при вычитании: `x = 1.128 - 8 + 7.5`
- [ ] Разбор умножения и деления: `x = 4 * 7.5 / 5, 18 * 2`
- [ ] Разбор с разными приоритетами операторов: `x = 4 + 10 * 2, 16 - 7 / 4`
- [ ] Разбор скобок: `x = (4 + (5 / 4)) * 2`
- [ ] Деление по модулю: `x = 5 % 2`
- [ ] Возведение в степень: `x = 2 ^ 3 ** 4`
- [ ] Разбор с разными приоритетами для возведения в степень: `x = 4 + 10 ^ 3`
- [ ] Поддержка констант: 
`s = Pi * 3 ^ 2 `
`s = Pi + MathE `

## Грамматика в нотации EBNF(от аналитика, изменённая)

````
program = { statement, [ ";" ] } ;

(* ключевый слова *)
statement = write_statement 
          | read_statement 
          | assignment_statement
          | if_statement
          | while_statement
          | compound_statement ;

write_statement = "write", "( ", [ expression_list ], " )" ;
read_statement = "read", "(", identifier, ")" ;
assignment_statement = identifier, ":=", expression ;
if_statement = "if", "(", condition, ")", statement, [ "else", statement ] ;
while_statement = "while", "(", condition, ")", statement ;

(* Выражения *)

expression = term_expression, { ("+" | "-"), term_expression } ;  
term_expression = factor_expression, { ("*" | "/" | "%"), factor_expression } ;
factor_expression = [ "+" | "-" ], exponentiation_expression ;
exponentiation_expression = simple_expression, [ "^", exponentiation_expression ] ;
simple_expression = number | string | identifier | function_call | "(", expression, ")" | const_expression ;

function_call = identifier, "(", [ expression_list ], ")" ;
expression_list = expression, { ",", expression } ;

const_expression = "Pi" | "MathE" | "true" | "false" ;

````