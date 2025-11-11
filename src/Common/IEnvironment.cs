namespace Blang.Common;

/// <summary>
/// Представляет окружение для выполнения программы.
/// Прежде всего это функции ввода/вывода.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Вызывается после вычисления результата очередной инструкции программы.
    /// </summary>
    public void AddResult(RuntimeValue result);

    /// <summary>
    /// Вывод переданного аргумента.
    /// </summary>
    public void Write(RuntimeValue value);

    /// <summary>
    /// Вывод переданного аргумента и переход на новую строку.
    /// </summary>
    public void Writeln(RuntimeValue value);

    /// <summary>
    /// Чтение аргумента.
    /// </summary>
    public RuntimeValue Read();

    /// <summary>
    /// Чтение аргумента и переход на новую строку.
    /// </summary>
    // public RuntimeValue ReadLine();
}