using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNode;
using XNodeEditor;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;

namespace QuestEditor.Nodes.Editor
{
    public static class NodeEditorGuiLayout
    {
        private static readonly Dictionary<Object, Dictionary<string, ReorderableList>> ReorderableListCache =
            new Dictionary<Object, Dictionary<string, ReorderableList>>();

        private static int reorderableListIndex = -1;

        /// <summary> Make a field for a serialized property. Automatically displays relevant node port. </summary>
        public static void PropertyField(SerializedProperty property, bool includeChildren = true)
        {
            PropertyField(property, null, includeChildren);
        }

        /// <summary> Make a field for a serialized property. Automatically displays relevant node port. </summary>
        private static void PropertyField(SerializedProperty property, GUIContent label, bool includeChildren = true)
        {
            if (property == null) throw new NullReferenceException();
            Node node = property.serializedObject.targetObject as Node;
            Debug.Assert(node != null, "node != null");
            NodePort port = node.GetPort(property.name);
            PropertyField(property, label, port, includeChildren);
        }

        /// <summary> Make a field for a serialized property. Manual node port override. </summary>
        public static void PropertyField(SerializedProperty property, GUIContent label, NodePort port, bool includeChildren = true)
        {
            if (property == null) throw new NullReferenceException();

            // If property is not a port, display a regular property field
            if (port == null) EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
            else
            {
                Rect rect = new Rect();

                float spacePadding = 0;
                SpaceAttribute spaceAttribute;
                if (NodeEditorUtilities.GetCachedAttrib(port.node.GetType(), property.name, out spaceAttribute)) spacePadding = spaceAttribute.height;

                // If property is an input, display a regular property field and put a port handle on the left side
                if (port.direction == NodePort.IO.Input)
                {
                    // Get data from [Input] attribute
                    Node.ShowBackingValue showBacking = Node.ShowBackingValue.Unconnected;
                    Node.InputAttribute inputAttribute;
                    bool dynamicPortList = false;
                    if (NodeEditorUtilities.GetCachedAttrib(port.node.GetType(), property.name, out inputAttribute))
                    {
                        dynamicPortList = inputAttribute.dynamicPortList;
                        showBacking = inputAttribute.backingValue;
                    }

                    //Call GUILayout.Space if Space attribute is set and we are NOT drawing a PropertyField
                    bool useLayoutSpace = dynamicPortList ||
                                          showBacking == Node.ShowBackingValue.Never ||
                                          (showBacking == Node.ShowBackingValue.Unconnected && port.IsConnected);
                    if (spacePadding > 0 && useLayoutSpace)
                    {
                        GUILayout.Space(spacePadding);
                        spacePadding = 0;
                    }

                    if (dynamicPortList)
                    {
                        Type type = GetType(property);
                        DynamicPortList(property.name, type, property.serializedObject, port.direction, inputAttribute.connectionType);
                        return;
                    }

                    switch (showBacking)
                    {
                        case Node.ShowBackingValue.Unconnected:
                            // Display a label if port is connected
                            if (port.IsConnected) EditorGUILayout.LabelField(label ?? new GUIContent(property.displayName));
                            // Display an editable property field if port is not connected
                            else EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
                            break;
                        case Node.ShowBackingValue.Never:
                            // Display a label
                            EditorGUILayout.LabelField(label ?? new GUIContent(property.displayName));
                            break;
                        case Node.ShowBackingValue.Always:
                            // Display an editable property field
                            EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
                            break;
                    }

                    rect = GUILayoutUtility.GetLastRect();
                    rect.position = rect.position - new Vector2(16, -spacePadding);
                    // If property is an output, display a text label and put a port handle on the right side
                }
                else if (port.direction == NodePort.IO.Output)
                {
                    // Get data from [Output] attribute
                    Node.ShowBackingValue showBacking = Node.ShowBackingValue.Unconnected;
                    Node.OutputAttribute outputAttribute;
                    bool dynamicPortList = false;
                    if (NodeEditorUtilities.GetCachedAttrib(port.node.GetType(), property.name, out outputAttribute))
                    {
                        dynamicPortList = outputAttribute.dynamicPortList;
                        showBacking = outputAttribute.backingValue;
                    }

                    //Call GUILayout.Space if Space attribute is set and we are NOT drawing a PropertyField
                    bool useLayoutSpace = dynamicPortList ||
                                          showBacking == Node.ShowBackingValue.Never ||
                                          (showBacking == Node.ShowBackingValue.Unconnected && port.IsConnected);
                    if (spacePadding > 0 && useLayoutSpace)
                    {
                        GUILayout.Space(spacePadding);
                        spacePadding = 0;
                    }

                    if (dynamicPortList)
                    {
                        Type type = GetType(property);
                        DynamicPortList(property.name, type, property.serializedObject, port.direction, outputAttribute.connectionType);
                        return;
                    }

                    switch (showBacking)
                    {
                        case Node.ShowBackingValue.Unconnected:
                            // Display a label if port is connected
                            if (port.IsConnected)
                                EditorGUILayout.LabelField(label ?? new GUIContent(property.displayName), NodeEditorResources.OutputPort,
                                    GUILayout.MinWidth(30));
                            // Display an editable property field if port is not connected
                            else EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
                            break;
                        case Node.ShowBackingValue.Never:
                            // Display a label
                            EditorGUILayout.LabelField(label ?? new GUIContent(property.displayName), NodeEditorResources.OutputPort,
                                GUILayout.MinWidth(30));
                            break;
                        case Node.ShowBackingValue.Always:
                            // Display an editable property field
                            EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
                            break;
                    }

                    rect = GUILayoutUtility.GetLastRect();
                    rect.position = rect.position + new Vector2(rect.width, spacePadding);
                }

                rect.size = new Vector2(16, 16);

                XNodeEditor.NodeEditor editor = NodeEditor.GetEditor(port.node, NodeEditorWindow.current);
                Color backgroundColor = editor.GetTint();
                Color col = NodeEditorWindow.current.graphEditor.GetPortColor(port);
                DrawPortHandle(rect, backgroundColor, col);

                // Register the handle position
                Vector2 portPos = rect.center;
                XNodeEditor.NodeEditor.portPositions[port] = portPos;
            }
        }

        private static Type GetType(SerializedProperty property)
        {
            Type parentType = property.serializedObject.targetObject.GetType();
            FieldInfo fi = parentType.GetFieldInfo(property.name);
            return fi.FieldType;
        }

        /// <summary> Make a simple port field. </summary>
        public static void PortField(NodePort port, params GUILayoutOption[] options)
        {
            PortField(null, port, options);
        }

        /// <summary> Make a simple port field. </summary>
        private static void PortField(GUIContent label, NodePort port, params GUILayoutOption[] options)
        {
            if (port == null) return;
            if (options == null) options = new [] { GUILayout.MinWidth(30) };
            Vector2 position = Vector3.zero;
            GUIContent content = label ?? new GUIContent(ObjectNames.NicifyVariableName(port.fieldName));

            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.direction == NodePort.IO.Input)
            {
                // Display a label
                EditorGUILayout.LabelField(content, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position - new Vector2(16, 0);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == NodePort.IO.Output)
            {
                // Display a label
                EditorGUILayout.LabelField(content, NodeEditorResources.OutputPort, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position + new Vector2(rect.width, 0);
            }

            PortField(position, port);
        }

        /// <summary> Make a simple port field. </summary>
        private static void PortField(Vector2 position, NodePort port)
        {
            if (port == null) return;

            Rect rect = new Rect(position, new Vector2(16, 16));

            XNodeEditor.NodeEditor editor = NodeEditor.GetEditor(port.node, NodeEditorWindow.current);
            Color backgroundColor = editor.GetTint();
            Color col = NodeEditorWindow.current.graphEditor.GetPortColor(port);
            DrawPortHandle(rect, backgroundColor, col);

            // Register the handle position
            Vector2 portPos = rect.center;
            XNodeEditor.NodeEditor.portPositions[port] = portPos;
        }

        private static void DrawPortHandle(Rect rect, Color backgroundColor, Color typeColor)
        {
            Color col = GUI.color;
            GUI.color = backgroundColor;
            GUI.DrawTexture(rect, NodeEditorResources.dotOuter);
            GUI.color = typeColor;
            GUI.DrawTexture(rect, NodeEditorResources.dot);
            GUI.color = col;
        }

        /// <summary> Is this port part of a DynamicPortList? </summary>
        public static bool IsDynamicPortListPort(NodePort port)
        {
            string[] parts = port.fieldName.Split(' ');
            if (parts.Length != 2) return false;
            Dictionary<string, ReorderableList> cache;
            if (ReorderableListCache.TryGetValue(port.node, out cache))
            {
                ReorderableList list;
                if (cache.TryGetValue(parts[0], out list)) return true;
            }

            return false;
        }

        /// <summary> Draw an editable list of dynamic ports. Port names are named as "[fieldName] [index]" </summary>
        /// <param name="fieldName">Supply a list for editable values</param>
        /// <param name="type">Value type of added dynamic ports</param>
        /// <param name="serializedObject">The serializedObject of the node</param>
        /// <param name="io">Whether this is input or output</param>
        /// <param name="connectionType">Connection type of added dynamic ports</param>
        /// <param name="typeConstraint"></param>
        /// <param name="onCreation">Called on the list on creation. Use this if you want to customize the created ReorderableList</param>
        private static void DynamicPortList(string fieldName, Type type, SerializedObject serializedObject, NodePort.IO io,
            Node.ConnectionType connectionType = Node.ConnectionType.Multiple,
            Node.TypeConstraint typeConstraint = Node.TypeConstraint.None, Action<ReorderableList> onCreation = null)
        {
            Node node = serializedObject.targetObject as Node;

            Debug.Assert(node != null, "node != null");
            var indexedPorts = node.DynamicPorts.Select(x =>
            {
                string[] split = x.fieldName.Split(' ');
                if (split.Length == 2 && split[0] == fieldName)
                {
                    int i  ;
                    if (int.TryParse(split[1], out i))
                    {
                        return new { index = i, port = x };
                    }
                }

                return new { index = -1, port = (NodePort) null };
            }).Where(x => x.port != null);
            List<NodePort> dynamicPorts = indexedPorts.OrderBy(x => x.index).Select(x => x.port).ToList();

            ReorderableList list = null;
            Dictionary<string, ReorderableList> rlc;
            if (ReorderableListCache.TryGetValue(serializedObject.targetObject, out rlc))
            {
                if (!rlc.TryGetValue(fieldName, out list))
                {
                }
            }

            // If a ReorderableList isn't cached for this array, do so.
            if (list == null)
            {
                SerializedProperty arrayData = serializedObject.FindProperty(fieldName);
                list = CreateReorderableList(fieldName, dynamicPorts, arrayData, type, serializedObject, io, connectionType, typeConstraint, onCreation);
                if (ReorderableListCache.TryGetValue(serializedObject.targetObject, out rlc)) rlc.Add(fieldName, list);
                else ReorderableListCache.Add(serializedObject.targetObject, new Dictionary<string, ReorderableList> { { fieldName, list } });
            }

            list.list = dynamicPorts;
            list.DoLayoutList();
        }

        private static ReorderableList CreateReorderableList(string fieldName, List<NodePort> dynamicPorts, SerializedProperty arrayData, Type type,
            SerializedObject serializedObject, NodePort.IO io, Node.ConnectionType connectionType, Node.TypeConstraint typeConstraint,
            Action<ReorderableList> onCreation)
        {
            bool hasArrayData = arrayData != null && arrayData.isArray;
            Node node = serializedObject.targetObject as Node;
            ReorderableList list = new ReorderableList(dynamicPorts, null, true, true, true, true);
            string label = arrayData != null ? arrayData.displayName : ObjectNames.NicifyVariableName(fieldName);

            list.drawElementCallback =
                ( rect, index, isActive, isFocused) =>
                {
                    Debug.Assert(node != null, "node != null");
                    NodePort port = node.GetPort(fieldName + " " + index);
                    if (hasArrayData)
                    {
                        Debug.Assert(arrayData != null, "arrayData != null");
                        if (arrayData.arraySize <= index)
                        {
                            EditorGUI.LabelField(rect, "Array[" + index + "] data out of range");
                            return;
                        }

                        SerializedProperty itemData = arrayData.GetArrayElementAtIndex(index);
                        EditorGUI.PropertyField(rect, itemData, true);
                    }
                    else
                    {
                        string portLabel = (port != null && port.IsConnected) ? port.Connection.node.name : " - Not connected - ";
                        EditorGUI.LabelField(rect, portLabel);
                    }

                    if (port != null)
                    {
                        Vector2 pos = rect.position + (port.IsOutput ? new Vector2(rect.width + 6, 0) : new Vector2(-36, 0));
                        PortField(pos, port);
                    }
                };
            list.elementHeightCallback = index =>
            {
                if (!hasArrayData) return EditorGUIUtility.singleLineHeight;
                Debug.Assert(arrayData != null, "arrayData != null");
                if (arrayData.arraySize <= index) return EditorGUIUtility.singleLineHeight;
                SerializedProperty itemData = arrayData.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(itemData);
            };
            list.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, label); };
            list.onSelectCallback = rl => { reorderableListIndex = rl.index; };
            list.onReorderCallback = rl =>
            {
                // Move up
                if (rl.index > reorderableListIndex)
                {
                    for (int i = reorderableListIndex; i < rl.index; ++i)
                    {
                        Debug.Assert(node != null, "node != null");
                        NodePort port = node.GetPort(fieldName + " " + i);
                        NodePort nextPort = node.GetPort(fieldName + " " + (i + 1));
                        port.SwapConnections(nextPort);

                        // Swap cached positions to mitigate twitching
                        Rect rect = NodeEditorWindow.current.portConnectionPoints[port];
                        NodeEditorWindow.current.portConnectionPoints[port] = NodeEditorWindow.current.portConnectionPoints[nextPort];
                        NodeEditorWindow.current.portConnectionPoints[nextPort] = rect;
                    }
                }
                // Move down
                else
                {
                    for (int i = reorderableListIndex; i > rl.index; --i)
                    {
                        Debug.Assert(node != null, "node != null");
                        NodePort port = node.GetPort(fieldName + " " + i);
                        NodePort nextPort = node.GetPort(fieldName + " " + (i - 1));
                        port.SwapConnections(nextPort);

                        // Swap cached positions to mitigate twitching
                        Rect rect = NodeEditorWindow.current.portConnectionPoints[port];
                        NodeEditorWindow.current.portConnectionPoints[port] = NodeEditorWindow.current.portConnectionPoints[nextPort];
                        NodeEditorWindow.current.portConnectionPoints[nextPort] = rect;
                    }
                }

                // Apply changes
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                // Move array data if there is any
                if (hasArrayData)
                {
                    Debug.Assert(arrayData != null, "arrayData != null");
                    arrayData.MoveArrayElement(reorderableListIndex, rl.index);
                }

                // Apply changes
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                NodeEditorWindow.current.Repaint();
                EditorApplication.delayCall += NodeEditorWindow.current.Repaint;
            };
            list.onAddCallback = rl =>
            {
                // Add dynamic port post-fixed with an index number
                string newName = fieldName + " 0";
                int i = 0;
                Debug.Assert(node != null, "node != null");
                while (node.HasPort(newName)) newName = fieldName + " " + (++i);

                if (io == NodePort.IO.Output) node.AddDynamicOutput(type, connectionType, Node.TypeConstraint.None, newName);
                else node.AddDynamicInput(type, connectionType, typeConstraint, newName);
                serializedObject.Update();
                EditorUtility.SetDirty(node);
                if (hasArrayData)
                {
                    Debug.Assert(arrayData != null, "arrayData != null");
                    arrayData.InsertArrayElementAtIndex(arrayData.arraySize);
                }

                serializedObject.ApplyModifiedProperties();
            };
            list.onRemoveCallback = rl =>
            {
                Debug.Assert(node != null, "node != null");
                var indexedPorts = node.DynamicPorts.Select(x =>
                {
                    string[] split = x.fieldName.Split(' ');
                    if (split.Length == 2 && split[0] == fieldName)
                    {
                        int i  ;
                        if (int.TryParse(split[1], out i))
                        {
                            return new { index = i, port = x };
                        }
                    }

                    return new { index = -1, port = (NodePort) null };
                }).Where(x => x.port != null);
                dynamicPorts = indexedPorts.OrderBy(x => x.index).Select(x => x.port).ToList();

                int index = rl.index;

                if (dynamicPorts[index] == null)
                {
                    UnityEngine.Debug.LogWarning("No port found at index " + index + " - Skipped");
                }
                else if (dynamicPorts.Count <= index)
                {
                    UnityEngine.Debug.LogWarning("DynamicPorts[" + index + "] out of range. Length was " + dynamicPorts.Count + " - Skipped");
                }
                else
                {
                    // Clear the removed ports connections
                    dynamicPorts[index].ClearConnections();
                    // Move following connections one step up to replace the missing connection
                    for (int k = index + 1; k < dynamicPorts.Count; k++)
                    {
                        for (int j = 0; j < dynamicPorts[k].ConnectionCount; j++)
                        {
                            NodePort other = dynamicPorts[k].GetConnection(j);
                            dynamicPorts[k].Disconnect(other);
                            dynamicPorts[k - 1].Connect(other);
                        }
                    }

                    // Remove the last dynamic port, to avoid messing up the indexing
                    node.RemoveDynamicPort(dynamicPorts[dynamicPorts.Count - 1].fieldName);
                    serializedObject.Update();
                    EditorUtility.SetDirty(node);
                }

                if (hasArrayData)
                {
                    Debug.Assert(arrayData != null, "arrayData != null");
                    if (arrayData.arraySize <= index)
                    {
                        UnityEngine.Debug.LogWarning("Attempted to remove array index " + index + " where only " + arrayData.arraySize + " exist - Skipped");
                        UnityEngine.Debug.Log(rl.list[0]);
                        return;
                    }

                    arrayData.DeleteArrayElementAtIndex(index);
                    // Error handling. If the following happens too often, file a bug report at https://github.com/Siccity/xNode/issues
                    if (dynamicPorts.Count <= arrayData.arraySize)
                    {
                        while (dynamicPorts.Count <= arrayData.arraySize)
                        {
                            arrayData.DeleteArrayElementAtIndex(arrayData.arraySize - 1);
                        }

                        UnityEngine.Debug.LogWarning("Array size exceeded dynamic ports size. Excess items removed.");
                    }

                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            };

            if (hasArrayData)
            {
                int dynamicPortCount = dynamicPorts.Count;
                while (dynamicPortCount < arrayData.arraySize)
                {
                    // Add dynamic port post-fixed with an index number
                    string newName = arrayData.name + " 0";
                    int i = 0;
                    Debug.Assert(node != null, "node != null");
                    while (node.HasPort(newName)) newName = arrayData.name + " " + (++i);
                    if (io == NodePort.IO.Output) node.AddDynamicOutput(type, connectionType, typeConstraint, newName);
                    else node.AddDynamicInput(type, connectionType, typeConstraint, newName);
                    EditorUtility.SetDirty(node);
                    dynamicPortCount++;
                }

                while (arrayData.arraySize < dynamicPortCount)
                {
                    arrayData.InsertArrayElementAtIndex(arrayData.arraySize);
                }

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }

            if (onCreation != null) onCreation(list);
            return list;
        }
    }
}