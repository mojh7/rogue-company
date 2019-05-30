using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject startNew;
    [SerializeField] private Text touch;
    [SerializeField] private Image titleTextImage;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Image modeButton;
    [SerializeField] private Sprite[] modeImages;
    [SerializeField] private RectTransform titleTransform;
    private Vector3 titleScale;
    private bool isHide = true;

    int mode = 0; //0 = normal, 1 = rush

    #region unityFunc
    private void Awake()
    {
        InitTitle();
    }
    private void Update()
    {
        if (isHide)
        {
            Color color = touch.color;
            color.a = color.a - Time.deltaTime;

            if (color.a < 0)
            {
                color.a = 0.0f;
                isHide = false;
            }

            touch.color = color;
        }
        else
        {
            Color color = touch.color;
            color.a = color.a + Time.deltaTime;

            if (color.a > 1)
            {
                color.a = 1.0f;
                isHide = true;
            }
            touch.color = color;
        }
    }
    #endregion

    #region func
    public void ChangeMode()
    {
        if (mode >= 1)
        {
            mode = 0;
        }
        else
        {
            mode++;
        }

        GameStateManager.Instance.SetMode((GameStateManager.GameMode)mode);
        InitTitle();
    }

    public void StartClicked()
    {
        LoadLobby();
    }

    public void ReStartClicked()
    {
        LoadInGame();
    }
    
    private void InitTitle()
    {
        // TODO : 삭제 예정
        int tempMusicNumber = Random.Range(0, 2);
        if (tempMusicNumber == 0)
            AudioManager.Instance.PlayMusic(0);
        else
            AudioManager.Instance.PlayMusic(8);

        GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.TITLE);
        modeButton.sprite = modeImages[(int)GameStateManager.Instance.GetMode()];

        if (GameDataManager.Instance.LoadData(GameDataManager.UserDataType.INGAME))
            restartButton.SetActive(true);
        else
            restartButton.SetActive(false);

        titleScale = titleTransform.localScale;
        //StartCoroutine(TitleAnimation());
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void LoadLobby()
    {
        GameStateManager.Instance.SetLoadsGameData(false);
        GameDataManager.Instance.ResetData(GameDataManager.UserDataType.INGAME);
        GameStateManager.Instance.LoadLobby();
    }

    public void LoadInGame()
    {
        GameStateManager.Instance.SetLoadsGameData(true);
        GameStateManager.Instance.LoadInGame();
    }
    #endregion

    IEnumerator TitleAnimation()
    {
        float time = 0;
        while (true)
        {
            titleTransform.sizeDelta = new Vector2(890, 222) * (1f + 0.05f * Mathf.Sin(time));
            //titleTransform.localScale = titleScale * (1f + 0.05f * Mathf.Sin(time));
            time += Time.fixedDeltaTime * 4.5f;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime * 2f);
        }
    }
}

