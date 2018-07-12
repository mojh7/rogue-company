using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BT
{
    public class NodeEditor : EditorWindow
    {
        private List<TaskNode> windows = new List<TaskNode>();

        private Vector2 mousePos;

        private TaskNode selectedNode;

        private TaskNode rootNode;

        private new string title;

        private string path = "Assets/";
        private string assetPathAndName;

        private bool makeTransitionMode = false;
        [MenuItem("Custom/Node Eidtor")]
        static void ShowEditor()
        {
            NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
        }

        private void OnGUI()
        {
            Event e = Event.current;

            mousePos = e.mousePosition;

            if (e.button == 1 && !makeTransitionMode)
            {
                if (e.type == EventType.MouseDown)
                {
                    bool clickedOnWindow = false;
                    int selectedWindow = -1;

                    for (int i = 0; i < windows.Count; i++)
                    {
                        if (windows[i] == null)
                            continue;
                        if (windows[i].windowRect.Contains(mousePos))
                        {
                            selectedWindow = i;
                            clickedOnWindow = true;
                            break;
                        }
                    }

                    if (!clickedOnWindow)
                    {
                        ShowMenu(e);
                    }
                    else
                    {
                        ShowSubMenu(e);
                    }
                }
            }
            else if (e.button == 0 && e.type == EventType.MouseDown && makeTransitionMode)
            {
                bool clickedOnWindow = false;
                int selectedWindow = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i] == null)
                        continue;
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectedWindow = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow && !windows[selectedWindow].Equals(selectedNode))
                {
                    windows[selectedWindow].SetInput((TaskNode)selectedNode, mousePos);
                    makeTransitionMode = false;
                    selectedNode = null;
                }

                if (!clickedOnWindow)
                {
                    makeTransitionMode = false;
                    selectedNode = null;
                }

                e.Use();
            }
            else if (e.button == 0 && e.type == EventType.MouseDown && !makeTransitionMode)
            {
                bool clickedOnWindow = false;
                int selectedWindow = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i] == null)
                        continue;
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectedWindow = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    TaskNode nodeToChange = windows[selectedWindow].ClickedOnInput(mousePos);

                    if (nodeToChange != null)
                    {
                        selectedNode = nodeToChange;
                        selectedNode.childrens.Remove(windows[selectedWindow]);
                        makeTransitionMode = true;
                    }
                }
            }
            if (makeTransitionMode && selectedNode != null)
            {
                Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);

                DrawNodeCurve(selectedNode.windowRect, mouseRect);

                Repaint();
            }
            foreach (TaskNode n in windows)
            {
                if(n!= null)
                    n.DrawCurves();
            }
            if (rootNode == null)
            {
                rootNode = CreateRoot();
                windows.Add(rootNode);
            }
            BeginWindows();
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i] == null)
                    continue;
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
            }
            title = EditorGUILayout.TextField("Title", title);
            if (GUILayout.Button("Save Tree"))
            {
                SaveTree();
            }
            EndWindows();
        }

        void SaveTree()
        {
            Task parent = rootNode.CreateBehaviorNode();
            if (title == "" || title == null)
                title = "Task";
            assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + title + ".asset");

            RecursionNode(rootNode, parent);
            AssetDatabase.CreateAsset(parent, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = parent;
        }
    void RecursionNode(TaskNode taskNode, Task parent)
        {
            for (int i = 0; i < taskNode.childrens.Count; i++)
            {
                Task child = taskNode.childrens[i].CreateBehaviorNode();
                child.name = child.GetType().ToString();
                AssetDatabase.AddObjectToAsset(child, assetPathAndName);
                parent.AddChild(child);
                RecursionNode(taskNode.childrens[i], child);
            }
        }
        Task CreateNode(TaskNode taskNode)
        {
            return taskNode.CreateBehaviorNode();
        }
        TaskNode CreateRoot()
        {
            TaskNode taskNode = new TaskNode(true);
            taskNode.windowRect = new Rect(100, 100, 200, 100);
            return taskNode;
        }
        void ShowMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Object Node"), false, ContextCallBack, "objectNode");

            menu.ShowAsContext();
            e.Use();
        }
        void ShowSubMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Make Transition"), false, ContextCallBack, "makeTransition");
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Delete Node"), false, ContextCallBack, "deleteNode");
            menu.ShowAsContext();
            e.Use();
        }
        void DrawNodeWindow(int id)
        {
            windows[id].DrawWindow();
            GUI.DragWindow();
        }
        void ContextCallBack(object obj)
        {
            string clb = obj.ToString();
            if (clb.Equals("objectNode"))
            {
                TaskNode taskNode = new TaskNode();
                taskNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

                windows.Add(taskNode);
            }
            else if (clb.Equals("makeTransition"))
            {
                bool clickedOnWindow = false;
                int selectedWindow = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectedWindow = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    selectedNode = windows[selectedWindow];
                    makeTransitionMode = true;
                }
            }
            else if (clb.Equals("deleteNode"))
            {
                bool clickedOnWindow = false;
                int selectedWindow = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos) && windows[i] != rootNode)
                    {
                        selectedWindow = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    TaskNode selNode = windows[selectedWindow];
                    if (selNode.parent != null)
                        selNode.parent.childrens.Remove(selNode);
                    windows.RemoveAt(selectedWindow);
                    foreach (TaskNode n in windows)
                    {
                        n.NodeDeleted(selNode);
                    }
                }
            }
        }
        public static void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Color shadowCol = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        }
    }
}
