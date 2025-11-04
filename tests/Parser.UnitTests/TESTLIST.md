Заметка лекц 8
в c# можно сделать изначально object а затем провести диспетчеризацию по типу объекта или двойуную диспетч
(реализуется с помощью visitor или match)
храним в стеке значения присваивания переменных, при этом именно послденего присваивания(последний statement)
возваращаем последнее значение в стеке
# Список тестов

## Требования к синтаксическому анализатору

1. Выражения могут содержать числа, операции, вызовы функций и скобки
2. Операторы могут иметь разный приоритет: например, у "*" приоритет выше, чем у "+"
3. Операторы могут иметь одинаковый приоритет: например, "*", "/", "%" имеют равный приоритет
4. Арифметические операции являются левоассоциативными: `3 * 7 / 3` означает `(3 * 7) / 3`, а не `3 * (7 / 3)`

## Список сценариев

### Разбор выражений

- [x] Разбор арифметических выражений с учётом приоритета: `1 + 2 * 3` → 7
- [x] Разбор выражений с операторами разного приоритета: `2 + 3 * 4 % 2 - 2.5` → 11.5
- [x] Проверка левоассоциативности вычитания: `10 - 3 - 2` → 5
- [x] Разбор унарных плюса и минуса: `+5 + -4` → 1
- [x] Разбор нескольких унарных операторов: `--5` → 5
- [x] Разбор выражения с унарными и бинарными операторами: `-5 + 3 * -2` → -11

### Разбор условий

- [x] Разбор if без else: ` if (true) {} ` → true
- [x] Проверка выполнения тела if при TRUE условии: ` if (true) { 1 + 5 - 3 }  ` → 3
- [x] Проверка выполнения тела if при FALSE условии: ` if (false) { 1 + 5 - 3 } ` → false
- [x] Разбор оператора "меньше": `if (3 < 5) {}` → true
- [x] Разбор выражений со сравнениями и арифметикой: `if (1 + 2 < 5) {}` → true
- [x] Разбор операторов всех приоритетов: `if (1 + 2 * 3 < 7) {}` → false
- [ ] Проверка левоассоциативности сравнений: `if (1 < 2 < 3) {}` → true
- [ ] Разбор арифметических выражений с учётом скобок: `(1 + 2) * 3` → 9
- [ ] Разбор выражения с множественными скобками: `((1 + 2) * (3 - 1)) - 2` → 4
- [x] Поддержка встроенных констант: 
`Pi`→  3.14...
`MathE * 1 `→ 2.71...
- [x] Разбор вызова встроенных функций: 
`min(7, 10 - 4, 8)`→ 6
`max(1, 3)`→ 3
`abs(-4)`→ 4
`pow(5, 3)`→ 125

### Инструкции и переменные
- [ ] Объявление одной изменяемой переменной: `int x = 3; x + 1` → 4
- [ ] Объявление нескольких изменяемых переменных: `int x = 1, y = 2, z = 3; x + y * z` → 7
- [ ] Множественное присваивание: `int x, y; x = y = 10 ; x + y` → 20
- [ ] Разбор выражений, разделённых точками с запятой: `1 + 2; 2 * 5; 4.5` → `[3, 10, 4.5]`
- [ ] Поддержка пользовательских констант: 
`const int s = 2 * 7 `
- [ ] Разбор write без завершающего разделителя: `write ("hello")`
- [ ] Разбор write без с завершающим разделителем: `write ("hello"); `
- [ ] Разбор write со списком выражений: ` write ("hello", "asd") `; 
- [ ] Разбор оператора присваивания: ` x = "qwerty" ` 


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
assignment_statement = identifier, "=", expression ;
while_statement = "while", "(", condition, ")", statement ;

if_statement = "if", "(", condition, ")", compound_statement, [ "else", statement ] ;

(* Условия *)
compound_statement = "{", { statement, [ ";" ] }, "}" ;
condition = expression, [ comparison_operator, expression ] ;
comparison_operator = "==" | "!=" | "<" | ">" | "<=" | ">=" ;

(* Выражения *)
expression = term_expression, { ("+" | "-"), term_expression } ;  
term_expression = factor_expression, { ("*" | "/" | "%"), factor_expression } ;
factor_expression = ("+" | "-"), factor_expression
                    | exponentiation_expression
exponentiation_expression = simple_expression, [ "^", exponentiation_expression ] ;
simple_expression = number | string | identifier | function_call | "(", expression, ")" | const_expression ;

function_call = identifier, "(", [ expression_list ], ")" ;
expression_list = expression, { ",", expression } ;

const_expression = "Pi" | "MathE" | "true" | "false" ;

````

## Проблемные места (Вопросы преподу)
Что делать с тестом где тело if выполняет больше 1 statement
- [ ] Возвращать массив результатов [Row]
- [x] Возвращать результат последнего statement 

Почему BinOpExpr - ?expression? - AstNode
(ConcreteElement - expression - IElement)