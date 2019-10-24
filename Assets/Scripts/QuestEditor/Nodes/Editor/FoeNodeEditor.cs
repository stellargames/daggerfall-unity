using QuestEditor.Nodes.Resources;

namespace QuestEditor.Nodes.Editor
{
    [CustomNodeEditor(typeof(FoeNode))]
    public class FoeNodeEditor : ResourceNodeEditor
    {
        public FoeNodeEditor()
        {
            PropertyChanged = PropertyChangeHandler;
        }

        private void PropertyChangeHandler(string propertyName)
        {
            var node = target as FoeNode;
            if (node == null) return;
            if ( (node.customDisplayName == false || string.IsNullOrEmpty(node.displayName))
                 && (propertyName == "foeType" || propertyName == "gender")
            )
            {
                node.UpdateDisplayName();
            }
            else if (propertyName == "displayName")
            {
                node.customDisplayName = true;
            }
        }
    }
}