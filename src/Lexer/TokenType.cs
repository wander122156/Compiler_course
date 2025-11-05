namespace Blang.Lexer;

public enum TokenType
{
    /// <summary>
    ///  Тип true.
    /// </summary>
    True,

    /// <summary>
    ///  Тип false.
    /// </summary>
    False,

    /// <summary>
    ///  Тип int.
    /// </summary>
    Int,

    /// <summary>
    ///  Тип string.
    /// </summary>
    String,

    /// <summary>
    ///  Ключевое слово const.
    /// </summary>
    Const,

    /// <summary>
    ///  Ключевое слово while.
    /// </summary>
    While,

    /// <summary>
    ///  Ключевое слово do.
    /// </summary>
    Do,

    /// <summary>
    ///  Ключевое слово write.
    /// </summary>
    Write,

    /// <summary>
    ///  Ключевое слово Writeln.
    /// </summary>
    Writeln,

    /// <summary>
    ///  Ключевое слово Read.
    /// </summary>
    Read,

    /// <summary>
    ///  Ключевое слово if.
    /// </summary>
    If,

    /// <summary>
    ///  Ключевое слово else.
    /// </summary>
    Else,

    /// <summary>
    ///  Ключевое слово and.
    /// </summary>
    And,

    /// <summary>
    ///  Ключевое слово or.
    /// </summary>
    Or,

    /// <summary>
    ///  Идентификатор (имя символа).
    /// </summary>
    Identifier,

    /// <summary>
    ///  Литерал числа.
    /// </summary>
    NumericLiteral,

    /// <summary>
    ///  Литерал строки.
    /// </summary>
    StringLiteral,

    /// <summary>
    ///  Оператор присваивания =.
    /// </summary>
    Assignment,

    /// <summary>
    ///  Оператор нестрогого сравнения ==.
    /// </summary>
    LooseEquality,

    /// <summary>
    ///  Оператор нестрогого неравенства !=.
    /// </summary>
    NotEqual,

    /// <summary>
    ///  Оператор отрицания.
    /// </summary>
    Not,

    /// <summary>
    ///  Оператор сложения.
    /// </summary>
    PlusSign,

    /// <summary>
    ///  Оператор вычитания.
    /// </summary>
    MinusSign,

    /// <summary>
    ///  Оператор умножения.
    /// </summary>
    MultiplySign,

    /// <summary>
    ///  Оператор деления.
    /// </summary>
    DivideSign,

    /// <summary>
    ///  Оператор деления по модулю.
    /// </summary>
    ModuloSign,

    /// <summary>
    ///  Оператор возведения в степень.
    /// </summary>
    ExponentiationSign,

    /// <summary>
    ///  Оператор сравнения "меньше".
    /// </summary>
    LessThan,

    /// <summary>
    ///  Оператор сравнения "меньше или равно".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    ///  Оператор сравнения "больше".
    /// </summary>
    GreaterThan,

    /// <summary>
    ///  Оператор сравнения "больше или равно".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    ///  Открывающая круглая скобка '('.
    /// </summary>
    OpenParenthesis,

    /// <summary>
    ///  Закрывающая круглая скобка ')'.
    /// </summary>
    CloseParenthesis,

    /// <summary>
    ///  Открывающая фигурная скобка '{'.
    /// </summary>
    OpenBraces,

    /// <summary>
    ///  Закрывающая фигурная скобка '}'.
    /// </summary>
    CloseBraces,

    /// <summary>
    ///  Запятая ','
    /// </summary>
    Comma,

    /// <summary>
    ///  Разделитель ';'
    /// </summary>
    Semicolon,

    /// <summary>
    ///  Конец файла.
    /// </summary>
    EndOfFile,

    /// <summary>
    ///  Недопустимая лексема.
    /// </summary>
    Error,
}