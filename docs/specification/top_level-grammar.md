
````
num a;
num b;
readln(a);
readln(b);
writeln(a+b);

````

````
num b;
read(b);
if( b > 0){
    writeln("Число положительное");
}
else{
    writeln("Число отрицательное");
};
````
Ключевые особенности:
- Блоки кода отделяются фигурными скобками
- Программа состоит из выражений и объявлений
- Явное определение переменной 
- Нельзя объявлять переменные с одинаковыми именами 
- Инструкция заканчивается знаком ";"

# EBNF 
````
program = statement, { ";", statement }, [ ";" ] ;
type = "num" | "string" | "bool" ;

(* ключевый слова *)
statement = variable_declaration
          | const_defenition
          | assignment
          | function_declaration
          | if_statement
          | write_statement 
          | writeln_statement
          | read_statement
          | readln_statement
          | while_statement
          | do_while_statement
          | for_statement
          | compound_statement
          | return_statement
          | break_statement
          | continue_statement


(* Объявления и переменные *)
variable_declaration = type, identifier, [ "=", expression ], { ",", identifier, [ "=", expression ] } ;
constant_definition = "const", type, identifier, "=", expression ;
assignment = identifier, "=", expression ;

function_declaration = "func", type, identifier, "(", [ parameter_list ], ")", compound_statement ;
parameter_list = type, identifier, { ",", type, identifier } ;
return__statement = "return", expression ;

(* Ввод-Вывод*)
write_statement = "write", "( ", [ expression_list ], " )" ;
writeln_statement = "writeln", "(" expression_list ")" ;
read_statement = "read", "(", identifier, {"," ,identifier } ")" ;
readln_statement = "readln", "(", identifier, {"," ,identifier } ")" ;

(* Циклы и инструкции*)
if_statement = "if", "(", condition, ")", statement_or_block, [ "else", statement_or_block ] ;
    statement_or_block = compound_statement | statement ;

while_statement = "while", "(", condition, ")", compound_statement ;
do_while_statement = "do", compound_statement, "while", "(", condition, ")" ;
for_statement = "for", "(", for_initialization, ";", for_condition, ";", for_increment, ")", compound_statement ;
for_initialization = variable_declaration | assignment    
for_condition = expression    
for_increment = assignment;

break_statement = "break" ;
continue_statement = "continue" ;

compound_statement = "{", statement, { ";", statement }, [ ";" ], "}" ;

(* Условия *)
condition = expression, [ comparison_operator, expression ] ;
comparison_operator = "==" | "!=" | "<" | ">" | "<=" | ">=" ;

(* Выражения *)
expression = multiplicative_expression, { ("+" | "-"), multiplicative_expression } ;  
multiplicative_expression = unary_expression, { ("*" |  "/" | "%"), unary_expression } ;
unary_expression = ("+" | "-"), unary_expression
                    | exponentiation_expression ;
exponentiation_expression = primary_expression, [ "^", exponentiation_expression ] ;
primary_expression = number | string | identifier | function_call | "(", expression, ")" | const_expression ;

function_call = identifier, "(", [ expression_list ], ")" ;
expression_list = expression, { ",", expression } ;

const_expression = "Pi" | "MathE" ;

````
