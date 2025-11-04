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
}