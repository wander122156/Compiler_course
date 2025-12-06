namespace Blang.Semantic;

#pragma warning disable RCS1194

/// <summary>
/// Исключение из-за несовместимых типов данных в программе.
/// </summary>
public class TypeException : Exception
{
    public TypeException(string message)
        : base($"Type error: {message}")
    {
    }
}
#pragma warning disable RCS1194