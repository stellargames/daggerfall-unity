using UnityEditor;

namespace QuestEditor.Nodes
{
    public interface IWatchInput
    {
        void InputChanged(string fieldName, SerializedProperty property);
    }
}