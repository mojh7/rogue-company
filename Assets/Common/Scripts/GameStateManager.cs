using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviourSingleton<GameStateManager> {

    enum GameState { NOTSTARTED, GAMEOVER, PLAYING, CLEAR, ENDING }
    GameState gameState = GameState.NOTSTARTED;

    #region UnityFunc
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
    }
    #endregion
}
