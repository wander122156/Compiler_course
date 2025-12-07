using Blang.Common;

using ValueType = Blang.Common.RuntimeValue.ValueType;

namespace Blang.Semantic;

/// <summary>
/// Контекст функции для семантического анализа
/// </summary>
public sealed class FunctionContext
{
    /// <summary>
    /// Имя функции
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Тип возвращаемого значения
    /// </summary>
    public ValueType ReturnType { get; set; }

    /// <summary>
    /// Флаг, указывающий что функция содержит return statement
    /// </summary>
    public bool HasReturn { get; set; }

    public FunctionContext(string name, ValueType returnType)
    {
        Name = name;
        ReturnType = returnType;
        HasReturn = false;
    }
}
