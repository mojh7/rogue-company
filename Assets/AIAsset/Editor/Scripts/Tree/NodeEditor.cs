using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BT
{
    public class NodeEditor : EditorWindow
    {
        public Vector2 scrollPos = Vector2.zero;

        private List<TaskNode> windows = new List<TaskNode>();

        private Vector2 mousePos;

        private TaskNode selectedNode;

        private TaskNode rootNode;

        private string path = "Assets/";
        private string assetPathAndName;

        private bool makeTransitionMode = false;
        static NodeEditor editor;
        static NodeDataEditor dataEditor;
        [MenuItem("Custom/Node Eidtor")]
        static void ShowEditor()
        {
            editor = EditorWindow.GetWindow<NodeEditor>();
            dataEditor = EditorWindow.GetWindow<NodeDataEditor>();
            dataEditor.Init(editor);
        }

        private void OnGUI()
        {
            Event e = Event.current;

            mousePos = e.mousePosition + scrollPos;

            if (e.button == 1 && !makeTransitionMode)
            {
                if (e.type == EventType.MouseDown)
                {
                    bool clickedOnWindow = false;

                    for (int i = 0; i < windows.Count; i++)
                    {
                        if (windows[i] == null)
                            continue;
                        if (windows[i].windowRect.Contains(mousePos))
                        {
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
            if (rootNode == null)
            {
                rootNode = CreateRoot();
                windows.Add(rootNode);
            }
            scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos, new Rect(0, 0, 10000, 10000));
            BeginWindows();
            if (makeTransitionMode && selectedNode != null)
            {
                Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);

                DrawNodeCurve(selectedNode.windowRect, mouseRect);

                Repaint();
            }
            foreach (TaskNode n in windows)
            {
                if (n != null)
                    n.DrawCurves();
            }
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i] == null)
                    continue;
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
            }
            EndWindows();
            GUI.EndScrollView();
        }

        public void LoadTree(Task task)
        {
            if (task == null)
                return;
            windows.Clear();
            rootNode = TaskToObjectNode(task, 0, 120);
        }
        TaskNode TaskToObjectNode(Task task, float x, float y)
        {
            TaskNode taskNode = new TaskNode(task);
            taskNode.windowRect = new Rect(x, y, 200, 120);

            System.Type type = task.GetType();
            switch (type.ToString())
            {
                case "Service":
                case "Selector":
                case "Sequence":
                case "SubSelector":
                    {
                        CompositeTask compositeTask = task as CompositeTask;
                        int childNum = compositeTask.GetChildren().Count;
                        for(int i=0;i< childNum; i++)
                        {
                            taskNode.AddChild(TaskToObjectNode(compositeTask.GetChildren()[i], x + 300, y + 150 * i));
                        }
                    }
                    break;
                case "Root":
                case "Bool":
                case "DistanceDecorate":
                case "TimeDecorate":
                case "HealthDecorate":
                    {
                        DecorateTask decorateTask = task as DecorateTask;
                        taskNode.AddChild(TaskToObjectNode(decorateTask.GetChildren(), x + 300, y));
                    }
                    break;
            }
            windows.Add(taskNode);
            return taskNode;
        }
        public void SaveTree(string nodeTitle)
        {
            Task parent = rootNode.CreateBehaviorNode();
            if (nodeTitle == "" || nodeTitle == null)
                nodeTitle = "Task";
            assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + nodeTitle + ".asset");

            RecursionNode(rootNode, parent);
            AssetDatabase.CreateAsset(parent, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = parent;
        }
        public void RemoveTree()
        {
            windows.Clear();
        }
        void RecursionNode(TaskNode taskNode, Task parent)
        {
            for (int i = 0; i < taskNode.childrens.Count; i++)
            {
                Task child = taskNode.childrens[i].CreateBehaviorNode();
                child.name = taskNode.childrens[i].windowTitle;
                AssetDatabase.AddObjectToAsset(child, assetPathAndName);
                parent.AddChild(child);
                RecursionNode(taskNode.childrens[i], child);
            }
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
            menu.AddItem(new GUIContent("Add Time Object Node"), false, ContextCallBack, "timeObjectNode");
            menu.AddItem(new GUIContent("Add Action Object Node"), false, ContextCallBack, "actionObjectNode");
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
            if(clb.Equals("timeObjectNode"))
            {
                TaskNode taskNode = new TaskNode();
                taskNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 120);
                taskNode.taskType = TaskType.DecorateTask;
                taskNode.decorateTask = EDecorateTask.TimeDecorate;
                windows.Add(taskNode);
            }
            else if(clb.Equals("actionObjectNode"))
            {
                TaskNode taskNode = new TaskNode();
                taskNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 120);
                taskNode.taskType = TaskType.ActionTask;
                windows.Add(taskNode);
            }
            else if (clb.Equals("objectNode"))
            {
                TaskNode taskNode = new TaskNode();
                taskNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 120);

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
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectedWindow = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    TaskNode selNode = windows[selectedWindow];
                    for(int i=0;i<selNode.parents.Count;i++)
                    {
                        selNode.parents[i].childrens.Remove(selNode);
                    }
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

        void OnDestroy()
        {
            if(dataEditor)
                dataEditor.Close();
        }
    }
    
    public class NodeDataEditor :EditorWindow
    {
        private TaskNode rootNode;

        private Task rootTask;

        private new string title;

        private string path = "Assets/";
        private string assetPathAndName;

        private bool makeTransitionMode = false;
        static NodeEditor editor;

        private void OnGUI()
        {

            title = EditorGUILayout.TextField("Title", title);
            rootTask = (Task)EditorGUILayout.ObjectField("rootTask", rootTask, typeof(Task), allowSceneObjects: true);
            if (GUILayout.Button("Load Tree"))
            {
                LoadTree();
            }
            if (GUILayout.Button("Save Tree"))
            {
                SaveTree();
            }
            if (GUILayout.Button("Remove Tree"))
            {
                RemoveTree();
            }
        }

        public void Init(NodeEditor nodeEditor)
        {
            editor = nodeEditor;
        }
        void LoadTree()
        {
            editor.LoadTree(rootTask);
            editor.Focus();
        }
        void SaveTree()
        {
            editor.SaveTree(title);
            editor.Focus();
        }

        void RemoveTree()
        {
            editor.RemoveTree();
            editor.Focus();
        }

        void OnDestroy()
        {
            if(editor)
                editor.Close();
        }
    }
}
