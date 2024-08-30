using System;

namespace SV;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed class LabelAttribute(string value, int index = 0) : Attribute
{
    public string Value { get; } = value;

    public int Index { get; } = index;
}