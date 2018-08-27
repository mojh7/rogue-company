using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviourSingleton<GameStateManager> {

    enum GameState { NOTSTARTED, GAMEOVER, PLAYING, CLEAR, ENDING }
    GameState gameState = GameState.NOTSTARTED;

    // 0531
    private bool loadsGameData = false;
    public bool GetLoadsGameData() { return loadsGameData; }
    public void SetLoadsGameData(bool _loadsGameData) { loadsGameData = _loadsGameData; }

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
    #endregion

    #region Func
    public void LoadInGame()
    {
        SceneDataManager.SetNextScene("InGameScene");
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
