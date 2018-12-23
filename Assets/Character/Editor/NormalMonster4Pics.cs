using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Collections.Generic;

public class NormalMonster4Pics : EditorWindow
{
    Sprite multipleSprite;
    Sprite[] sprites;

    static NormalMonster4Pics window;
    [MenuItem("Custom/NormalMonster4Pics")]

    static public void ShowWindow()
    {
        // 윈도우 생성
        window = (NormalMonster4Pics)EditorWindow.GetWindow(typeof(NormalMonster4Pics));
    }

    void OnGUI()
    {
        BeginWindows();
        multipleSprite = (Sprite)EditorGUILayout.ObjectField("multipleSprite", multipleSprite, typeof(Sprite), allowSceneObjects: true);
        if (GUILayout.Button("CreateAnim"))
            CreateAnim();
        EndWindows();

    }

    void CreateAnim()
    {
        if (!multipleSprite)
            return;
        Sprite[] sprites = GetSprites(multipleSprite);
        AnimationClipSettings animClipSett = new AnimationClipSettings();
        animClipSett.loopTime = true;

        AnimationClip clip = new AnimationClip();
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        int size = 2;
        clip.frameRate = 2;   // FPS
        ObjectReferenceKeyframe[] keyIdle = new ObjectReferenceKeyframe[size];
        for (int i = 0; i < size; i++)
        {
            keyIdle[i] = new ObjectReferenceKeyframe();
            keyIdle[i].time = i / clip.frameRate;
        }
        keyIdle[0].value = sprites[0];
        keyIdle[1].value = sprites[1];

        animClipSett.stopTime = size / clip.frameRate;
        animClipSett.keepOriginalPositionY = true;

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyIdle);
        AnimationUtility.SetAnimationClipSettings(clip, animClipSett);
        AssetDatabase.CreateAsset(clip, "assets/" + multipleSprite.name + "_idle.anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        clip = new AnimationClip();
        spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        size = 3;
        clip.frameRate = 3;   // FPS
        keyIdle = new ObjectReferenceKeyframe[size];
        for (int i = 0; i < size; i++)
        {
            keyIdle[i] = new ObjectReferenceKeyframe();
            keyIdle[i].time = i / clip.frameRate;
        }
        keyIdle[0].value = sprites[1];
        keyIdle[1].value = sprites[2];
        keyIdle[2].value = sprites[3];

        animClipSett.stopTime = size / clip.frameRate;
        animClipSett.keepOriginalPositionY = true;

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyIdle);
        AnimationUtility.SetAnimationClipSettings(clip, animClipSett);
        AssetDatabase.CreateAsset(clip, "assets/" + multipleSprite.name + "_walk.anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    Sprite[] GetSprites(Sprite sprite)
    {
        string path = AssetDatabase.GetAssetPath(sprite);
        Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        List<Sprite> l = new List<Sprite>(objects.Length);
        foreach (var i in objects)
        {
            var s = i as Sprite;
            l.Add(s);
        }
        return l.ToArray();
    }

}
