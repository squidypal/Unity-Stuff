// Assets/Scripts/Attributes/ConditionalFieldAttribute.cs
using UnityEngine;

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string FieldToCheck { get; private set; }
    public object CompareValue { get; private set; }

    public ConditionalFieldAttribute(string fieldToCheck, object compareValue)
    {
        FieldToCheck = fieldToCheck;
        CompareValue = compareValue;
    }
}