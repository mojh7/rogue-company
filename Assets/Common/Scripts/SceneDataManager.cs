using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataManager : MonoBehaviour {

    public static string NextScene;

    public static void SetNextScene(string _str) { NextScene = _str; }

    private void Start()
    {
        NextScene = "SelectScene";
    }
}
