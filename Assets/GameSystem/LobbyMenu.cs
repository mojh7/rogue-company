using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LobbyMenu : MonoBehaviour {

    private enum MenuBarState { SHOW, HIDE }

    #region variables
    [SerializeField]
    private Button menuButton;
    [SerializeField]
    private RectTransform menuBarTransform;
    [SerializeField]
    private Vector2 menuBarTopPos;
    [SerializeField]
    private Vector2 menuBarBottomPos;
    private MenuBarState menuBarState;
    [SerializeField]
    private float autoScrollSpeed;

    private bool isAutoScrolling;
    #endregion

    #region unityFunc
    // Use this for initialization
    void Start ()
    {
        menuBarState = MenuBarState.HIDE;
        menuBarTransform.position = menuBarBottomPos;
        isAutoScrolling = false;
    }
    #endregion

    #region func

    public void ChangeMenuBar()
    {
        if (isAutoScrolling)
            return;
        StartCoroutine(AutoScrollMenuBarCoroutine());
    }

    #endregion

    #region coroutine
    private IEnumerator AutoScrollMenuBarCoroutine()
    {
        float time = 0;
        isAutoScrolling = true;
        Vector2 startPos, endPos;
        if (MenuBarState.SHOW == menuBarState)
        {
            menuBarState = MenuBarState.HIDE;
            startPos = menuBarTopPos;
            endPos = menuBarBottomPos;
        }
        else
        {
            menuBarState = MenuBarState.SHOW;
            startPos = menuBarBottomPos;
            endPos = menuBarTopPos;
        }
        while (time <= 1)
        {
            menuBarTransform.localPosition = Vector2.Lerp(startPos, endPos, time);
            time += Time.fixedDeltaTime * autoScrollSpeed;
            yield return YieldInstructionCache.WaitForSeconds(0.01f);
        }
        menuBarTransform.localPosition = Vector2.Lerp(startPos, endPos, 1);
        isAutoScrolling = false;
    }
    #endregion
}
