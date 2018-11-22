using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToInGameSence : MonoBehaviour {

    private void Awake()
    {
        TutorialManager.Instance.StartShake(2, 2, 1);
    }
    void Update () {
        // 활성화 되어 클릭 당하면 실행
        if (Input.GetKeyDown(KeyCode.V))
        {
            SceneManager.LoadScene("InGameScene");
        }
	}
}
