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

        if(GUILayout.Button("EditorTestBtn_init_func"))
        {
            Debug.Log("Editor 테스트, init 함수 실행");
            bulletInfo.InitFieldValue();
        }
        
    }
}