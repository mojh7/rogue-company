using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * bridge logo -> team logo -> title -> lobby -> character select -> ingame
 * 
 */ 

public class GameStateManager : MonoBehaviourSingleton<GameStateManager>
{

    enum GameState { NOTSTARTED, GAMEOVER, PLAYING, CLEAR, ENDING }
    public enum GameMode { NORMAL, RUSH }
    public enum GameScene { LOGO = 0, TITLE = 1, LOBBY = 2, IN_GAME = 3, BOSS_RUSH = 4}

    GameState gameState = GameState.NOTSTARTED;
    [SerializeField]
    GameMode gameMode = GameMode.NORMAL;
    [SerializeField]
    GameScene gameScene = GameScene.LOGO;
    
    // 새 게임, 로드 게임 구분
    private bool loadsGameData = false;
    public bool GetLoadsGameData() { return loadsGameData; }
    public GameScene GetGameScene() { return gameScene; }
    public GameMode GetMode()
    {
        return gameMode;
    }

    // 인게임씬에서 바로 시작할 때 설정해줄 디버깅 용
    public void SetGameScene(GameScene gameScene) { this.gameScene = gameScene; }
    public void SetLoadsGameData(bool _loadsGameData) { loadsGameData = _loadsGameData; }
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
    public bool IsInGame()
    {
        return gameScene == GameScene.IN_GAME || gameScene == GameScene.BOSS_RUSH;
    }


    public void LoadInGame()
    {
        if(!GameDataManager.Instance.isFirst)
        {
            SceneDataManager.SetNextScene("TutorialScene");
            SceneManager.LoadScene("LoadingScene");
            return;
        }
        if (GameMode.NORMAL == gameMode)
        {
            gameScene = GameScene.IN_GAME;
            SceneDataManager.SetNextScene("InGameScene");
        }
        else if(GameMode.RUSH == gameMode)
        {
            gameScene = GameScene.BOSS_RUSH;
            SceneDataManager.SetNextScene("BossRushScene");
        }
        SceneManager.LoadScene("LoadingScene");
    }
    public void LoadTitle()
    {
        gameScene = GameScene.TITLE;
        SceneManager.LoadScene("TitleScene");
        //SceneDataManager.SetNextScene("TitleScene");
        //SceneManager.LoadScene("LoadingScene");
    }
    public void LoadLobby()
    {
        gameScene = GameScene.LOBBY;
        SceneManager.LoadScene("LobbyScene");
    }

    public void GameOver()
    {
        gameState = GameState.GAMEOVER;
        GameDataManager.Instance.ResetData(GameDataManager.UserDataType.INGAME);
    }
    #endregion
}
