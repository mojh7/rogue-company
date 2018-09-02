using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviourSingleton<GameStateManager> {

    enum GameState { NOTSTARTED, GAMEOVER, PLAYING, CLEAR, ENDING }
    public enum GameMode { NORMAL, RUSH }
    GameState gameState = GameState.NOTSTARTED;
    [SerializeField]
    GameMode gameMode = GameMode.NORMAL;
    // 0531
    private bool loadsGameData = false;
    public bool GetLoadsGameData() { return loadsGameData; }
    public void SetLoadsGameData(bool _loadsGameData) { loadsGameData = _loadsGameData; }

    public GameMode GetMode()
    {
        return gameMode;
    }
    public void SetMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    #region UnityFunc
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        DontDestroyOnLoad(this);
        Logo.Instance.LoadLogo();
    }
    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
    #endregion

    #region Func
    public void LoadInGame()
    {
        if(GameMode.NORMAL == gameMode)
        {
            SceneDataManager.SetNextScene("InGameScene");
        }
        else if(GameMode.RUSH == gameMode)
        {
            SceneDataManager.SetNextScene("BossRushScene");
        }
        SceneManager.LoadScene("LoadingScene");
    }
    public void LoadTitle()
    {
        SceneDataManager.SetNextScene("TitleScene");
        SceneManager.LoadScene("LoadingScene");
    }
    public void LoadSelect()
    {
        SceneManager.LoadScene("SelectScene");
    }
    public void GameOver()
    {
        gameState = GameState.GAMEOVER;
        //GameDataManager.Instance.ResetData();
    }
    #endregion
}
