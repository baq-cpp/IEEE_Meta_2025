using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MultiPaster
{
    public class MPWindow : EditorWindow
    {
        private static readonly Vector2 DefaultWindowSize = new Vector2(400f, 550f);
        private Vector2 scrollPosition = Vector2.zero;
        private bool isDragDropHovering = false;

        [MenuItem("Tools/Multi Paster")]
        public static void OpenWindow()
        {
            var window = GetWindow<MPWindow>("Multi Paster");
            window.position = new Rect(600f, 200f, DefaultWindowSize.x, DefaultWindowSize.y);
        }
        
        public List<Component> componentList = new List<Component>();
        private ReorderableList reorderComponentList;
        public List<GameObject> gameObjectList = new List<GameObject>();
        private ReorderableList reorderGOList;

        private void OnEnable()
        {
            reorderGOList = new ReorderableList(gameObjectList, typeof(GameObject), true, true, true, true);
            reorderGOList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Target List");
            reorderGOList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                gameObjectList[index] = (GameObject)EditorGUI.ObjectField(rect, gameObjectList[index], typeof(GameObject), true);
            };

            reorderComponentList = new ReorderableList(componentList, typeof(Component), true, true, true, true);
            reorderComponentList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Component List");
            reorderComponentList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                componentList[index] = (Component)EditorGUI.ObjectField(rect, componentList[index], typeof(Component), true);
            };
        }
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(20f);
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("MULTI PASTER", style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            DrawLine(Color.cyan);
            TargetComponentGUI();
            DrawLine(Color.yellow);
            TargetObjectGUI();
            DrawLine(Color.cyan);

            // MULTI PASTE
            EditorGUILayout.Space(30f);
            GUIStyle buttonStyle;
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.fixedHeight = 50;
            buttonStyle.fontSize = 15;
            buttonStyle.fontStyle = FontStyle.Bold;
            GUILayout.BeginHorizontal();
            EditorGUILayout.Space(20f);
            if (GUILayout.Button("MULTI  PASTE", buttonStyle))
            {
                MPMaster.MultiPaste(componentList, gameObjectList);
            }
            EditorGUILayout.Space(20f);
            GUILayout.EndHorizontal();


            EditorGUILayout.EndScrollView(); 
        }

        void TargetComponentGUI()
        {

            EditorGUILayout.Space(20f);
            
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.alignment = TextAnchor.MiddleCenter;
          
            // Handle drag and drop manually
            var dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drag and drop COMPONENTS here to add them to the list", boxStyle);
            GUILayout.Space(10); // Add a small space between the drop area and list

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(Event.current.mousePosition))
                        break;

                    isDragDropHovering = true;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            var component = draggedObject as Component;
                            if (component != null)
                            {
                                componentList.Add(component);
                            }
                        }

                        isDragDropHovering = false;
                    }

                    Event.current.Use();
                    break;

                case EventType.DragExited:
                    isDragDropHovering = false;
                    break;
            }

            // Display a hover message when dragging components over the drop area
            if (isDragDropHovering)
            {
                GUILayout.Label("Drop components to add them to the list", EditorStyles.helpBox);
            }
            reorderComponentList.DoLayoutList();
            if (GUILayout.Button("Remove All"))
            {
                componentList.Clear();
            }
        }
        void TargetObjectGUI()
        {

            EditorGUILayout.Space(20f);

            var dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            GUIStyle gUIStyle = new GUIStyle(GUI.skin.box);
            gUIStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Box(dropArea, "Drag and drop TARGETS here to add them to the list", gUIStyle);
            GUILayout.Space(10);

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(Event.current.mousePosition))
                        break;

                    isDragDropHovering = true;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            var targetObj = draggedObject as GameObject;
                            if (targetObj != null)
                            {
                                gameObjectList.Add(targetObj);
                            }
                        }

                        isDragDropHovering = false;
                    }

                    Event.current.Use();
                    break;

                case EventType.DragExited:
                    isDragDropHovering = false;
                    break;
            }

            // Display a hover message when dragging components over the drop area
            if (isDragDropHovering)
            {
                GUILayout.Label("Drop components to add them to the list", EditorStyles.centeredGreyMiniLabel);
            }
           
            reorderGOList.DoLayoutList();
            
            if (GUILayout.Button("Remove All"))
            {
                gameObjectList.Clear();
            }
   
        }

        void DrawLine(Color inColor)
        {
            EditorGUILayout.Space(15f);
            Rect lineRect = GUILayoutUtility.GetRect(position.width, 2);
            EditorGUI.DrawRect(lineRect, inColor);
        }
        
    }
}


