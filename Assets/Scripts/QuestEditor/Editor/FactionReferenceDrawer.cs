using System.Collections.Generic;
using System.Linq;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using UnityEditor;
using UnityEngine;

namespace QuestEditor.Editor
{
    [CustomPropertyDrawer(typeof(FactionReference))]
    public class FactionReferenceDrawer : PropertyDrawer
    {
        private readonly Dictionary<int, bool> expandedIds = new Dictionary<int, bool>();
        private float currentHeight;
        private DaggerfallUnity dfUnity;
        private Dictionary<int, FactionFile.FactionData> factionDict;
        private IEnumerable<Faction> factionTree;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!Init()) return;

            SerializedProperty idProperty = property.FindPropertyRelative("id");

            label = EditorGUI.BeginProperty(position, label, property);
            label.text = string.Format("{0}: {1}", label.text, GetNameForId(idProperty.intValue));

            // Variable to determine our property height.
            float startHeight = position.y;

            // Reset the height changed by overriding GetPropertyHeight.
            position.height = EditorGUIUtility.singleLineHeight;

            // Create a collapsible label.
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                position = DrawFactions(position, factionTree);
                UpdateSelectedId(idProperty);
                EditorGUI.indentLevel--;
            }

            currentHeight = position.y - startHeight;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            if (property.isExpanded && Init())
                height += currentHeight;
            return height;
        }

        private static void UpdateSelectedId(SerializedProperty idProperty)
        {
            string selected = GUI.GetNameOfFocusedControl();
            int selectedId = string.IsNullOrEmpty(selected) ? 0 : int.Parse(selected);
            if (selectedId != 0)
                idProperty.intValue = selectedId < 0 ? 0 : selectedId;
        }

        private string GetNameForId(int id)
        {
            return factionDict.ContainsKey(id) ? factionDict[id].name : "[None]";
        }

        private Rect DrawFactions(Rect position, IEnumerable<Faction> factions)
        {
            foreach (Faction faction in factions)
            {
                var label = new GUIContent(faction.Data.name);
                // We associate the id to the label through the control name.
                GUI.SetNextControlName(faction.Data.id.ToString());
                position = faction.HasChildren() ? DrawExpandableItem(position, faction, label) : DrawPlainItem(position, label);
            }

            return position;
        }

        private Rect DrawPlainItem(Rect position, GUIContent label)
        {
            EditorGUI.SelectableLabel(position, label.text);
            position.y += EditorGUIUtility.singleLineHeight;
            return position;
        }

        private Rect DrawExpandableItem(Rect position, Faction faction, GUIContent label)
        {
            if (!expandedIds.ContainsKey(faction.Data.id)) return position;

            expandedIds[faction.Data.id] = EditorGUI.Foldout(position, expandedIds[faction.Data.id], label);
            position.y += EditorGUIUtility.singleLineHeight;

            if (!expandedIds[faction.Data.id]) return position;

            EditorGUI.indentLevel++;
            position = DrawFactions(position, faction.Children);
            EditorGUI.indentLevel--;

            return position;
        }

        private bool Init()
        {
            if (!dfUnity)
                dfUnity = DaggerfallUnity.Instance;

            if (!dfUnity.IsReady || string.IsNullOrEmpty(dfUnity.Arena2Path))
                return false;

            if (factionTree != null) return true;

            var factionFile = new FactionFile(dfUnity.ContentReader.GetFactionFilePath(), FileUsage.UseMemory, true);

            // Generate a traversable tree structure from the faction.
            factionDict = factionFile.FactionDict;
            factionDict.Add(-1, new FactionFile.FactionData {id = -1, name = "[None]"});

            factionTree = Faction.GenerateTree(factionDict.Values.OrderBy(x => x.name));

            // Keep track of each item's expanded state
            var factionsWithChildren = factionDict.Values.Where(factionData => factionData.children != null && factionData.children.Count > 0);
            foreach (FactionFile.FactionData expandableFaction in factionsWithChildren) expandedIds.Add(expandableFaction.id, false);

            return true;
        }
    }

    internal class Faction
    {
        public FactionFile.FactionData Data { get; private set; }
        public IEnumerable<Faction> Children { get; private set; }

        public bool HasChildren()
        {
            return Children != null && Children.Any();
        }

        public static IEnumerable<Faction> GenerateTree(IEnumerable<FactionFile.FactionData> factions, int id = 0)
        {
            var dataList = factions.ToList();
            foreach (FactionFile.FactionData factionData in dataList.Where(f => f.parent == id))
                yield return new Faction
                {
                    Data = factionData,
                    Children = GenerateTree(dataList, factionData.id)
                };
        }
    }
}