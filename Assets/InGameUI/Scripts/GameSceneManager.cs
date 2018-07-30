using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameScene의 Main UI Manager 입니다.
/// </summary>
public class GameSceneManager : MonoBehaviour {

    #region variues


    #endregion

    // 애들 각각의 컴포넌트들을 받아와서 업데이트 시키기.

    void Start ()
    {
        Screen.SetResolution(Screen.width, Screen.height, true);
    }
}
