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

- [x] Разбор `if без else: if (true) {}` → []
- [x] Проверка выполнения тела if при TRUE условии: `if (true) { write(1 + 5 - 3) }` → 3
- [x] Проверка выполнения тела if при FALSE условии: `if (false) { write(1 + 5 - 3) }` → []
- [x] Разбор оператора "меньше": `if (3 < 5) { write(1) }` → 1
- [x] Разбор выражений со сравнениями и арифметикой: `if (1 + 2 * 6^2 < 321) { write(1) }` → 1
- [x] Разбор if с else (обе ветки): `if (true) { write(1) } else { write(2) }` → 1
- [x] Разбор if с else (ветка else): `if (false) { write(1) } else { write(2) }` → 2
- [x] Вложенные if-else: `if (true) { if (false) { write(1) } else { write(2) } }` → 2
- [x] Условие с переменной: `num x = 10; if (x > 5) { write(1) } `→ 1
- [x] Оператор равенства: `if (5 == 5) { write(1) }` → 1
- [x] Оператор неравенства: `if (5 != 3) { write(1) }` → 1
- [x] Оператор "больше или равно": `if (5 >= 5) { write(1) }` → 1
- [x] Оператор "меньше или равно": `if (3 <= 5) { write(1) }` → 1
- [x] Несколько statements в теле: `if (true) { write(1); write(2) }` → 1, 2
- [x] Несколько statements в else: `if (false) { write(1); write(2) } else { write(3); write(4) }` → 3, 4
- [x] Ошибка: отсутствует скобка условия: `if true) { write(1) }`
- [x] Ошибка: отсутствует открывающая фигурная скобка: `if (true) write(1) }`
- [x] Ошибка: неверное выражение условия: `if (true false) { write(1) }`

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

#### Write операции
- [x] Разбор write без завершающего разделителя: `write ("hello")` → hello
- [x] Разбор write со списком выражений: `write ("hello, ", "i am ", "Blang")` → Blang (возвращает последнее значение)
- [x] Writeln с выражением: `writeln ("sum:", 2 + 2)` → 4 (возвращает последнее значение)
- [x] Write с арифметическим выражением: `write(2 * 3 + 1)` → 7
- [x] Write с переменной: `num x = 5; write(x * 2)` → 10
- [x] Writeln без аргументов: `writeln()` → null

#### Read операции
- [x] Разбор read с одной переменной: `num a; read(a)` → считывает 42 в переменную a
- [x] Read с несколькими переменными: `num a, b, c; read(a, b, c)` → считывает 10, 20, 30 в переменные
- [x] Readln с несколькими строками: `num first, second; readln(first); readln(second)` → считывает 100, 200 построчно
- [x] Read с дробными числами: `num a; read(a)` → считывает 3.14
- [x] Readln с числами: `num number; readln(number)` → считывает 123.456
    
### Константы
- [x] Объявление константы: `const num c = 3` → 3
- [x] Переменная перекрывает константу: `const num c = 3.14159; num c = 2; 4.0 * c * 4.0` → 3.14159, 2, 50.26548

### Обработка ошибок
- [x] Неопределенная переменная без объявлений: `x + 1` → ArgumentException
- [x] Неопределенная переменная в выражении: `num x, y; x + y + z` → ArgumentException
- [x] Неправильный идентификатор: `num 123` → UnexpectedLexemeException
- [x] Read неопределенной переменной: `read(undefinedVar)` → ArgumentException
- [x] Write неопределенной переменной: `write(undefinedVar)` → ArgumentException
- [x] Отсутствие скобок в write: `write "hello"` → UnexpectedLexemeException
- [x] Отсутствие запятой в multiple read: `num a, b; read(a b)` → UnexpectedLexemeException

### Тесты для For циклов

- [x] Выполнение тела for цикла: `for (num i = 0; i < 3; i = i + 2) { write(i) }` → [0, 1, 2]
- [x] For цикл с одним проходом: `for (num i = 5; i < 6; i = i + 1) { write(i) }` → [5]
- [x] For цикл без итераций: `for (num i = 10; i < 5; i = i + 1) { write(i) }` → []

#### Тесты с разными шагами
- [x] For цикл с отрицательным шагом: `for (num i = 5; i > 0; i = i - 1) { write(i) }` → [5, 4, 3, 2, 1]
- [x] For цикл с умножением как шагом: `for (num i = 1; i < 10; i = i * 2) { write(i) }` → [1, 2, 4, 8]

- [x] Вложенные for циклы: → [0, 1, 10, 11]
```
for (num i = 0; i < 2; i = i + 1) 
{
  for (num j = 0; j < 2; j = j + 1) 
  {
    write(i * 10 + j)
  }
}
```
-[x] For цикл с внешней переменной:  → [0, 1, 2]
```
num count = 3;
for (num i = 0; i < count; i = i + 1) { write(i) }
```
-[x] For цикл с изменением внешней переменной: → [6]
```
num sum = 0;
for (num i = 1; i <= 3; i = i + 1) { sum = sum + i }
write(sum)
``` 
-[x] Локальная переменная цикла не видна снаружи: → exeption
```
for (num i = 0; i < 3; i = i + 1) { write(i) }
write(i)
```
### Тесты для While циклов

- [ ] Выполнение тела while цикла: `num i = 0; while (i < 3) { write(i); i = i + 1 }` → [0, 1, 2]
- [ ] While цикл с одним проходом: `num i = 5; while (i < 6) { write(i); i = i + 1 }` → [5]
- [ ] While цикл без итераций: `while (false) { write(1) }` → []

#### Тесты с разными условиями
- [ ] While цикл с отрицательным изменением: `num i = 5; while (i > 0) { write(i); i = i - 1 }` → [5, 4, 3, 2, 1]
- [ ] Вложенные while циклы: → [0, 1, 10, 11]
- [ ] While цикл с изменением внешней переменной: → [6]
```
num sum = 0;
num i = 1;  
while (i <= 3) { sum = sum + i; i = i + 1 }
write(sum)
```
## Грамматика в нотации EBNF(от аналитика, изменённая)

````
program = statement, { ";", statement }, [ ";" ] ;

(* ключевый слова *)
statement = variable_declaration
          | const_defenition
          | assignment
          | if_statement
          | function_declaration
          | write_statement 
          | writeln_statement
          | read_statement
          | readln_statement
          | while_statement
          | for_statement
          | compound_statement
          | return__statement

(* Объявления и переменные *)
variable_declaration = "num", identifier, [ "=", expression ], { ",", identifier, [ "=", expression ] }
constant_definition = "const", "num", identifier, "=", expression ;
assignment = identifier, "=", expression ;

function_declaration = "func", "num", identifier, "(", [ parameter_list ], ")", compound_statement ;
parameter_list = "num", identifier, { ",", "num", identifier } ;
return__statement = "return", [ expression ] ;

(* Ввод-Вывод*)
write_statement = "write", "( ", [ expression_list ], " )" ;
writeln_statement = "writeln", "(" expression_list ")" ;
read_statement = "read", "(", identifier, {"," ,identifier } ")" ;
readln_statement = "readln", "(", identifier, {"," ,identifier } ")"

(* Циклы и инструкции*)
if_statement = "if", "(", condition, ")", statement_or_block, [ "else", statement_or_block ]
    statement_or_block = compound_statement | statement

while_statement = "while", "(", condition, ")", compound_statement ;
for_statement = "for", "(", for_initialization, ";", for_condition, ";", for_increment, ")", compound_statement
    for_initialization = variable_declaration | assignment    
    for_condition = expression    
    for_increment = assignment;

(* Условия *)
condition = expression, [ comparison_operator, expression ] ;
comparison_operator = "==" | "!=" | "<" | ">" | "<=" | ">=" ;

compound_statement = "{", statement, { ";", statement }, [ ";" ], "}"

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