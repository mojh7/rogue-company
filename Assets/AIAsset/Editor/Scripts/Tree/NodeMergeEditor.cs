using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BT;

public class NodeMergeEditor : EditorWindow
{

    Task parentTask;
    Task[] childTask;
    int childNum;
    private string path = "Assets/";
    private string assetPathAndName;

    [MenuItem("Custom/NodeMergeEditor")]
    static void ShowEditor()
    {
        NodeMergeEditor editor = EditorWindow.GetWindow<NodeMergeEditor>();
    }

    private void OnGUI()
    {

        BeginWindows();
        parentTask = (Task)EditorGUILayout.ObjectField("parentTask", parentTask, typeof(Task), allowSceneObjects: true);
        EndWindows();
        EditorGUI.BeginChangeCheck();
        childNum = EditorGUILayout.IntField("childNum", childNum);
        if (EditorGUI.EndChangeCheck())
        {
            childTask = new Task[childNum];
        }
        for (int i = 0; i < childNum; i++)
        {
            childTask[i] = (Task)EditorGUILayout.ObjectField("childTask", childTask[i], typeof(Task), allowSceneObjects: true);
        }
        if (GUILayout.Button("Merge"))
            Merge();
    }

    void Merge()
    {
        if (parentTask == null || childNum == 0 || childTask == null)
            return;
        assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + parentTask.name+"Copy" + ".asset");
        string parentPath = AssetDatabase.GetAssetPath(parentTask);
        AssetDatabase.CopyAsset(parentPath, assetPathAndName);
        for (int i = 0; i < childNum; i++)
        {
            string childPath = AssetDatabase.GetAssetPath(childTask[i]);
            Task c = childTask[i].Clone();
            c.name = c.GetType().ToString() + "Added";
            AssetDatabase.AddObjectToAsset(c, assetPathAndName);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
