# Список тестов

## Класс Lexer

- [x] Разбор SELECT без FROM: `SELECT 2025;`
- [x] Разбор FROM: `SELECT first_name FROM student;`
- [ ] Поддержка множественных полей: `SELECT first_name, last_name, email FROM student;`
- [ ] Определение ключевых слов без учёта регистра: `select first_name, last_name FrOM student;`
- [ ] Оператор сложения: `SELECT count + 1 FROM counter;`


---

- [x] Разбор if без else: ` if (x = 1)  { x = x + 1 } `
- [x] Разбор if c else: `if (x = 1) { x = x + 1 } else { x = x + 10 }`
- [x] Разбор if c else if: `if (x = 1) { x = x + 1 } else if (x = 10) { x = x + 10 }`
- [x] Разбор while do: ` while (x < 10) do { x = x + 1 } `
- [x] Разбор write: `write ("hello world")  `
- [x] Разбор writeln: `writeln ("hello world")`
- [x] Разбор read: `read (a)`
- [x] Разбор and: `if (x = 1 and y = 2) `
- [x] Разбор or: `if (x = 1 or x = 2) `