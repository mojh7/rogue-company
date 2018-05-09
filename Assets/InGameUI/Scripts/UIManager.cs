using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * ui manager 한개에서 ingame ui랑 기타 시작화면, 설정화면 ? 등등 다른 ui도 관리 할지
 * 
 * ingame 용이랑 기타 ui랑 구분 할지 아직 모르겠음.
 * 
 * 
 */ 
public class UIManager : MonoBehaviour {
    
    [SerializeField]
    private Joystick virtualJoystick;
    public Canvas canvas;
    private static UIManager instance;
    
    #region getter
    public static UIManager Instance { get { return instance; } }

    public Joystick GetJoystick { get { return virtualJoystick; } }
    #endregion

    #region unityFunction
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }
    #endregion

    #region function
    public void InitForPlayGame()
    {
        canvas.worldCamera = FindObjectOfType<Camera>();
        
    }
    #endregion
}
