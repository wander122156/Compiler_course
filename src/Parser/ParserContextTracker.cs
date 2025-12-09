namespace Blang.Parser;

[Flags]
public enum ParserContext
{
    /// <summary>
    /// нет контекста
    /// </summary>
    None = 0,

    /// <summary>
    /// Флаг нахождения в функции
    /// </summary>
    Function = 1 << 0,

    /// <summary>
    /// Флаг нахождения в цикле
    /// </summary>
    Loop = 1 << 1,
}

public class ParserContextTracker
{
    private readonly Stack<ParserContext> _contextStack = new();

    public ParserContextTracker()
    {
        _contextStack.Push(ParserContext.None);
    }

    /// <summary>
    /// Входит в новый контекст (добавляет к текущему)
    /// </summary>
    public void Enter(ParserContext context)
    {
        ParserContext current = _contextStack.Peek();
        _contextStack.Push(current | context);
    }

    /// <summary>
    /// Выходит из текущего контекста
    /// </summary>
    public void Exit()
    {
        if (_contextStack.Count <= 1)
        {
            throw new InvalidOperationException("Cannot exit from base context");
        }

        _contextStack.Pop();
    }

    /// <summary>
    /// Проверяет, находится ли парсер в указанном контексте
    /// </summary>
    public bool IsIn(ParserContext context)
    {
        ParserContext current = _contextStack.Peek();
        return (current & context) == context;
    }

    /// <summary>
    /// Проверяет, находится ли парсер внутри функции
    /// </summary>
    public bool IsInFunction() => IsIn(ParserContext.Function);

    /// <summary>
    /// Проверяет, находится ли парсер внутри цикла
    /// </summary>
    public bool IsInLoop() => IsIn(ParserContext.Loop);

    /// <summary>
    /// Возвращает текущий комбинированный контекст
    /// </summary>
    public ParserContext Current => _contextStack.Peek();

    /// <summary>
    /// Сбрасывает трекер до начального состояния (для тестов)
    /// </summary>
    public void Reset()
    {
        _contextStack.Clear();
        _contextStack.Push(ParserContext.None);
    }
}