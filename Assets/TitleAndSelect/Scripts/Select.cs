using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Select : MonoBehaviour {

    public GameObject panel;
    public Text text;
    Player.PlayerType m_playerType;
    public void OpenInfo(string _str)
    {
        Player.PlayerType playerType = (Player.PlayerType)System.Enum.Parse(typeof(Player.PlayerType), _str);
        m_playerType = playerType;
        switch (m_playerType)
        {
            case Player.PlayerType.MUSIC:
                text.text = "대학교 시절 밴드 활동";
                break;
            case Player.PlayerType.SOCCER:
                text.text = "고등학교 친구들과 축구";
                break;
            case Player.PlayerType.FISH:
                text.text = "아버지와의 낚시";
                break;
            case Player.PlayerType.ARMY:
                text.text = "동기들과 행복하게 웃고있는 군생활";
                break;
            default:
                break;
        }
        panel.SetActive(true);
    }

    public void StartInGame()
    {
        GameStateManager.Instance.SetPlayerType(m_playerType);
        SceneDataManager.SetNextScene("InGameScene");
        SceneManager.LoadScene("LoadingScene");
    }
}
