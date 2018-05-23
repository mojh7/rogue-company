using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviourSingleton<GameStateManager> {

    enum GameState { NOTSTARTED, GAMEOVER, PLAYING, CLEAR, ENDING }
    GameState gameState = GameState.NOTSTARTED;
    Player.PlayerType m_playerType;
    float floor;

    #region setter
    public void SetPlayerType(Player.PlayerType _playerType) { m_playerType = _playerType; }
    #endregion
    
    #region UnityFunc
    private void Start()
    {
        DontDestroyOnLoad(this);
        Title.Instance.LoadLogoTitle();
    }
    #endregion

    #region Func

    #endregion
}
