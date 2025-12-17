# Грамматика языка blang

## Синтаксис выражений

Выражения могут содержать:

- литерал числа
- операторы(бинарные, унарные)
- скобки
- Константы(ключевые слова)
- Вызовы встроенных функций

## Операторы

**Арифметические операторы**

| Символы | Операция                 |
|---------|--------------------------|
| +       | Сложение или унарный "+" |
| -       | Разность или унарный "-" |
| *       | Произведение             |
| /       | Деление                  |
| ^       | Возведение в степень     |
| %       | Деление с остатком       |

**Сравнительные операторы**

| Символы | Значение         |
|---------|------------------|
| ==      | Равенство        |
| !=      | Не равенство     |
| ">"     | Больше           |
| <       | Меньше           |
| '>=     | Больше или равно |
| <=      | Меньше или равно |

## Ключевые слова

Ключевыми словами являются константы:

- Число "пи" - Pi. Равняется 3,14.
- Число "Эйлера" - MathE. Равняется 2,7182.

## Приоритет операторов

| Приоритет по убыванию | Оператор             |
|-----------------------|----------------------|
| 4                     | '^'                  |
| 3                     | '*', '/', '%'        |
| 2                     | '+', '-'             |
| 1                     | '>', '<', '>=', '<=' |
| 0                     | '==', '!='           |

## Грамматика в нотации EBNF

````
program = statement, { ";", statement }, [ ";" ] ;

(* ключевый слова *)
statement =
          | if_statement
          | expression ;
if_statement = "if", "(", condition, ")", compound_statement, [ "else", statement ] ;

(* Условия *)
compound_statement = "{", { statement, [ ";" ] }, "}" ;
condition = expression, [ comparison_operator, expression ] ;
comparison_operator = "==" | "!=" | "<" | ">" | "<=" | ">=" ;

(* Выражения *)
expression = term_expression, { ("+" | "-"), term_expression } ;  
term_expression = factor_expression, { ("*" | "/" | "%"), factor_expression } ;
factor_expression = [ "+" | "-" ], exponentiation_expression ;
exponentiation_expression = simple_expression, { ("^"), exponentiation_expression } ;
simple_expression = number | "(", expression, ") | const_expression;
const_expression = "Pi" | "MathE";
number = digit, {digit}, [".", {digit}];
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9";

function_call = identifier, "(", [ expression_list ], ")" ;
expression_list = expression, { ",", expression } ;

(* Список *)
list_of_numbers = number, { ",", number };

````


