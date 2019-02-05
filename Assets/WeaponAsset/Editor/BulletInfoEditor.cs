using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BulletInfo))]
public class BulletInfoEditor : Editor
{
    public AnimationCurve a = AnimationCurve.Linear(0, -360f, 5, 360f);
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BulletInfo bulletInfo = (BulletInfo)target;

        EditorGUILayout.CurveField("a on angle", a);

        if(GUILayout.Button("TestBtn"))
        {
            Debug.Log("12122");
            bulletInfo.EditorTest();
        }
        
    }
}