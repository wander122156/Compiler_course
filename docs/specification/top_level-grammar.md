
````
num a;
num b;
readln(a);
readln(b);
writeln(a+b)

````

````
num b;
read(b);
if( b > 0){
    writeln("Число положительное")
}
else{
    writeln("Число отрицательное")
}
````
Ключевые особенности:
- Блоки кода отделяются фигурными скобками
- Программа состоит из выражений и объявлений
- Явное определение переменной 
- Нельзя объявлять переменные с одинаковыми именами 
- Инструкция заканчивается знаком ";", исключениями являются случаи, когда строчка единственная в блоке кода и последняя в блоке кода
  

# EBNF 
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
          | return_statement
          | function_declaration

write_statement = "write", "( ", [ expression_list ], " )" ;
writeln_statement = "writeln", "(" expression_list ")" ;
read_statement = "read", "(", identifier, {"," ,identifier } ")" ;
readln_statement = "readln", "(", identifier, {"," ,identifier } ")";
while_statement = "while", "(", condition, ")", statement ;
if_statement = "if", "(", condition, ")", compound_statement, [ "else", ( if_statement | statement ) ];
for_statement = "for", "(", assignment_statement, expression, assignment_statement, ")", statement ;
return_statement = "vozvrat", [ expression ] ;
function_declaration = "func", identifier, "(", [ parameter_list ], ")", compound_statement ;
parameter_list = parameter, { ", ", parameter } ;
parameter = "num", identifier ;

variable_declaration = "num", identifier, [ "=", expression ], { ",", identifier, [ "=", expression ] };
constant_definition = "const", "num", identifier, "=", expression ;
assignment = identifier, "=", expression ;


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

