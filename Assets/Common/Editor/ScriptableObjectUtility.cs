using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}


public class WeaponInfoUtilityEditor : EditorWindow
{
    WeaponInfo parent;
    BulletPatternInfo[] bulletPatternInfos;
    BulletInfo[] bulletInfos;
    private string path = "Assets/";
    private string assetPathAndName;

    int childNum, childnum2;

    [MenuItem("Custom/ScriptableObjectUtilityEditor")]
    static void ShowEditor()
    {
        WeaponInfoUtilityEditor editor = EditorWindow.GetWindow<WeaponInfoUtilityEditor>();
    }

    private void OnGUI()
    {

        BeginWindows();
        parent = (WeaponInfo)EditorGUILayout.ObjectField("WeaponInfo", parent, typeof(WeaponInfo), allowSceneObjects: true);

        EditorGUI.BeginChangeCheck();
        childNum = EditorGUILayout.IntField("childNum", childNum);
        if (EditorGUI.EndChangeCheck())
        {
            bulletPatternInfos = new BulletPatternInfo[childNum];
        }
        for (int i = 0; i < childNum; i++)
        {
            bulletPatternInfos[i] = (BulletPatternInfo)EditorGUILayout.ObjectField("BulletPatternInfo", bulletPatternInfos[i], typeof(BulletPatternInfo), allowSceneObjects: true);
        }

        EditorGUI.BeginChangeCheck();
        childnum2 = EditorGUILayout.IntField("childNum", childnum2);
        if (EditorGUI.EndChangeCheck())
        {
            bulletInfos = new BulletInfo[childnum2];
        }
        for (int i = 0; i < childnum2; i++)
        {
            bulletInfos[i] = (BulletInfo)EditorGUILayout.ObjectField("BulletPatternInfo", bulletInfos[i], typeof(BulletInfo), allowSceneObjects: true);
        }
        if (GUILayout.Button("Merge"))
            Merge();
        EndWindows();
    
    }

    void Merge()
    {
        if (parent == null || childNum == 0 || bulletPatternInfos == null)
            return;
        assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + parent.name + "Copy" + ".asset");
        parent.Init();
        WeaponInfo weaponInfo = parent.Clone();
        string parentPath = AssetDatabase.GetAssetPath(parent);
        AssetDatabase.CopyAsset(parentPath, assetPathAndName);
        for (int i = 0; i < childNum; i++)
        {
            System.Type type=  bulletPatternInfos[i].GetType();
            if(type == typeof(MultiDirPatternInfo))
            {
                MultiDirPatternInfo c = ScriptableObject.CreateInstance<MultiDirPatternInfo>();
                EditorUtility.CopySerialized(bulletPatternInfos[i], c);


                c.name = bulletPatternInfos[i].name + "Added";
                AssetDatabase.AddObjectToAsset(c, assetPathAndName);

            }
            else if(type == typeof(SpreadPatternInfo))
            {
                SpreadPatternInfo c = ScriptableObject.CreateInstance<SpreadPatternInfo>();
                EditorUtility.CopySerialized(bulletPatternInfos[i], c);


                c.name = bulletPatternInfos[i].name + "Added";
                AssetDatabase.AddObjectToAsset(c, assetPathAndName);

            }
            else if(type == typeof(RowPatternInfo))
            {
                RowPatternInfo c = ScriptableObject.CreateInstance<RowPatternInfo>();
                EditorUtility.CopySerialized(bulletPatternInfos[i], c);


                c.name = bulletPatternInfos[i].name + "Added";
                AssetDatabase.AddObjectToAsset(c, assetPathAndName);

            }
        }
        for (int i = 0; i < childnum2; i++)
        {
            BulletInfo c = ScriptableObject.CreateInstance<BulletInfo>();

            EditorUtility.CopySerialized(bulletInfos[i], c);
            c.name = bulletInfos[i].name + "Added";
            AssetDatabase.AddObjectToAsset(c, assetPathAndName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}