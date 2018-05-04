using UnityEditor;
using UnityEngine;


public class WeaponEditor : EditorWindow {

    [MenuItem("Custom/Weapon")]
    public static void ShowWindow()
    {
        WeaponEditor window = (WeaponEditor)EditorWindow.GetWindow(typeof(WeaponEditor));
    }

    void OnGUI()
    {
        
    }

}
