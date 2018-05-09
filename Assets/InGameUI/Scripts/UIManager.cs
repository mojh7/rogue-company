using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * ui manager 한개에서 ingame ui랑 기타 시작화면, 설정화면 ? 등등 다른 ui도 관리 할지
 * 
 * ingame 용이랑 기타 ui랑 구분 할지 아직 모르겠음.
 * 
 * ingame 용이랑 구분
 */
public class UIManager : MonoBehaviourSingleton<UIManager> {
    
    [SerializeField]
    private Joystick virtualJoystick;
    public Canvas canvas;
    public GameObject ingamePanel;
    
    #region getter
    public Joystick GetJoystick { get { return virtualJoystick; } }
    #endregion

    #region function
    public void ToggleUI()
    {
        ingamePanel.SetActive(!ingamePanel.activeSelf);
    }
    public void InitForPlayGame()
    {
        //canvas.worldCamera = FindObjectOfType<Camera>(); /*왜 있는겨 -유성- */
    }
    #endregion
}
