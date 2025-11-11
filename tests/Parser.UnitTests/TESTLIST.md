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
- [x] Разбор арифметических выражений с учётом скобок: `(1 + 2) * 3` → 9
- [x] Разбор выражения с множественными скобками: `((1 + 2) * (3 - 1)) - 2` → 4
- [x] Поддержка встроенных констант: 
`Pi`→  3.14...
`MathE * 1 `→ 2.71...
- [x] Разбор вызова встроенных функций: 
`min(7, 10 - 4, 8)`→ 6
`max(1, 3)`→ 3
`abs(-4)`→ 4
`pow(5, 3)`→ 125

### Инструкции и переменные
- [x] Несколько выражений через точку с запятой: `1 + 2; 3 * 4; 5.5` → 3, 12, 5.5
- [x] Объявление одной переменной без инициализации: `num x` → 0 
- [x] Объявление одной переменной с инициализацией: `num x = 3` → 3
- [x] Объявление нескольких переменных: `num x = 1, y = 2, z = 3` → 3
- [x] Объявление переменных с последующим выражением: `num x = 1, y = 2, z = 3; x + y * z` → 3, 7
- [x] Присваивание объявленным переменным: `num x, y; x = 10; y = 12; x + y` → 0, 10, 12, 22
- [x] Переприсваивание переменных: `num a = 1, b = 2; a = 5; b = a + 1` → 2, 5, 6

### Ввод-Вывод
- [ ] Разбор write без завершающего разделителя: `write ("hello")` → hello
- [ ] Разбор write со списком выражений: ` write ("hello, ", "i am ", "Blang" ) ` → hello, i am Blang
- [ ] Writeln с выражением: `writeln ("sum:", 2 + 2)` → sum: 4
- [ ] Разбор read: `read (a)` → 
- [ ] Read с несколькими переменными: `read (a, b, c)` →
- [ ] Readln чтение нескольких строк: `readln(first); readln(second)` →
    
### Константы
- [x] Объявление константы: `const num c = 3` → 3
- [x] Переменная перекрывает константу: `const num c = 3.14159; num c = 2; 4.0 * c * 4.0` → 3.14159, 2, 50.26548

### Обработка ошибок
- [x] Неопределенная переменная без объявлений: `x + 1` → исключение
- [x] Неопределенная переменная в выражении: `num x, y; x + y + z` → исключение
- [x] Неправильный идентификатор: `num 123` → исключение
- [ ] Read без аргументов: `read ()`
- [ ] Write без аргументов: `write ()`
- [ ] Выражение в read: `read (1 + 1)`

- [ ] Объявление нескольких изменяемых переменных: `num x = 1, y = 2, z = 3; x + y * z` → 7
- [ ] Множественное присваивание: `num x, y; x = y = 10 ; x + y` → 20
- [ ] Разбор выражений, разделённых точками с запятой: `1 + 2; 2 * 5; 4.5` → 3, 10, 4.5
- [ ] Поддержка пользовательских констант: 
`const num s = 2 * 7 `


## Грамматика в нотации EBNF(от аналитика, изменённая)

````
program = { statement, [ ";" ] } ;

(* ключевый слова *)
statement =  
          | variable_declaration
          | const_defenition
          | assignment
          | if_statement
          | write_statement 
          | writeln_statement
          | read_statement
          | readln_statement
          | while_statement
          | compound_statement
          | expression (временно)

write_statement = "write", "( ", [ expression_list ], " )" ;
writeln_statement = "writeln", "(" expression_list ")" ;
read_statement = "read", "(", identifier, {"," ,identifier } ")" ;
readln_statement = "readln", "(", identifier, {"," ,identifier } ")"
while_statement = "while", "(", condition, ")", statement ;

variable_declaration = "num", identifier, [ "=", expression ], { ",", identifier, [ "=", expression ] }
constant_definition = "const", "num", identifier, "=", expression ;
assignment = identifier, "=", expression ;

if_statement = "if", "(", condition, ")", compound_statement, [ "else", statement ] ;

(* Условия *)
compound_statement = "{", { statement, [ ";" ] }, "}" ;
condition = expression, [ comparison_operator, expression ] ;
comparison_operator = "==" | "!=" | "<" | ">" | "<=" | ">=" ;

(* Выражения *)
expression = multiplicative_expression, { ("+" | "-"), multiplicative_expression } ;  
multiplicative_expression = unary_expression, { ("*" | "/" | "%"), unary_expression } ;
unary_expression = ("+" | "-"), unary_expression
                    | exponentiation_expression
exponentiation_expression = primary_expression, [ "^", exponentiation_expression ] ;
primary_expression = number | string | identifier | function_call | "(", expression, ")" | const_expression ;

function_call = identifier, "(", [ expression_list ], ")" ;
expression_list = expression, { ",", expression } ;

const_expression = "Pi" | "MathE" ;

````

## Проблемные места (Вопросы преподу)
Что делать со списком выражений?
- [x] возвращать последнее выражение?
- [ ] возвращать все

Почему BinOpExpr - ?expression? - AstNode
(ConcreteElement - expression - IElement)