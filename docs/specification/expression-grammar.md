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

Описание операторов

| Написание | Семантика                         | Особенности                  |
|-----------|-----------------------------------|------------------------------|
| \+        | Сложение чисел                    | Левая ассоциативность        |
| \-        | Вычитание чисел или унарный минус | Левая ассоциативность        |
| \*        | Умножение чисел                   | Левая ассоциативность        |
| \/        | Деление чисел                     | Левая ассоциативность        |
| \%         | Деление с остатком                | Левая ассоциативность        |
| \^        | Возведение в степень              | Правая ассоциативность       |
| \=\=      | Равно                             | Нет ассоциативности          |
| \!\=      | Не равно                          | Нет ассоциативности          |
| \>        | Больше                            | Нет ассоциативности          |
| \<        | Меньше                            | Нет ассоциативности          |
| \>\=      | Больше или равно                  | Нет ассоциативности          |
| \<\=      | Меньше или равно                  | Нет ассоциативности          |
| \& \&     | Логическое «И»                    | Вычисления по короткой схеме |
| \|\|      | Логическое «ИЛИ»                  | Вычисления по короткой схеме |
| \!        | Логическое «НЕ»                   | Унарный, применяется к одному операнду|
| \=        | Присваивание                      | Не возвращает значения       |

## Приоритет операторов

| Приоритет по убыванию | Операторы                        |
|-----------------------|----------------------------------|
| 8                     | `^`                              |
| 7                     | унарные `+`, `-`                 |
| 6                     | `*`, `/`, `%`                    |
| 5                     | бинарные `+`, `-`                |
| 4                     | `<`, `>`, `<=`, `>=`             |
| 3                     | `==`, `!=`                       |
| 2                     | `!`                              |
| 1                     | `&&`                             |
| 0                     | `||`                             |

## Грамматика выражений в нотации EBNF

````
(* Выражения *)
expression = logical_or_expression ;
logical_or_expression = logical_and_expression, { "||", logical_and_expression } ;
logical_and_expression = logical_not_expression, { "&&", logical_not_expression } ;
logical_not_expression = "!", logical_not_expression
                       | comparison_expression ;

comparison_expression = additive_expression, [ comparison_operator, additive_expression ] ;
comparison_operator = "==" | "!=" | "<" | ">" | "<=" | ">=" ;

additive_expression = multiplicative_expression, { ("+" | "-"), multiplicative_expression } ;
multiplicative_expression = unary_expression, { ("*" | "/" | "%"), unary_expression } ;
unary_expression = ("+" | "-"), unary_expression
                 | exponentiation_expression ;
exponentiation_expression = primary_expression, [ "^", exponentiation_expression ] ;
primary_expression = number | string | identifier | function_call | "(", expression, ")" | const_expression ;

function_call = identifier, "(", [ expression_list ], ")" ;
expression_list = expression, { ",", expression } ;

const_expression = "Pi" | "MathE" ;

````


