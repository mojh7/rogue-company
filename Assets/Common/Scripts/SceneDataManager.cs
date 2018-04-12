using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataManager : MonoBehaviour {

    public static string NextScene;

    private void Start()
    {
        NextScene = "Game";
    }
}
