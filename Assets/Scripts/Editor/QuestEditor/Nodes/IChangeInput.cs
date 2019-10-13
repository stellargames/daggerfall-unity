using UnityEditor;

public interface IChangeInput
{
    void InputChanged(string fieldName, SerializedProperty property);
}