using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// MainScene 총 관리 하는 UI
/// </summary>
public class GameSceneUI : MonoBehaviour {

    private float currentTime;

    private void Awake()
    {
        currentTime = 0;
    }

    void Update () {
        if (Stamina.Instance.StaminaState())
        {
            currentTime += Time.deltaTime;
            if (currentTime > 3)
            {
                Stamina.Instance.StaminaPlus();
                currentTime = 0;
            }
        }
    }
}
