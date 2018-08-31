using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Select : MonoBehaviour {

    public GameObject panel;
    public Text text;
    [SerializeField]
    Image soccerImage;
    Player.PlayerType m_playerType;
    Color white, dest;
    private void Start()
    {
        white = Color.white;
        dest = white * 0.8f;

        StartCoroutine(CoroutineColorLerp());
    }

    IEnumerator CoroutineColorLerp()
    {
        while(true)
        {
            UtilityClass.ColorLerp(this, soccerImage, white, dest, 1);
            yield return YieldInstructionCache.WaitForSeconds(1.1f);
            UtilityClass.ColorLerp(this, soccerImage, dest, white, 1);
            yield return YieldInstructionCache.WaitForSeconds(1.1f);
        }
    }

    public void OpenInfo(string _str)
    {
        Player.PlayerType playerType = (Player.PlayerType)System.Enum.Parse(typeof(Player.PlayerType), _str);
        m_playerType = playerType;
        switch (m_playerType)
        {
            case Player.PlayerType.MUSIC:
                text.text = "대학교 시절 \n밴드 활동";
                break;
            case Player.PlayerType.SOCCER:
                text.text = "대학교 축구 동아리에서의 추억";
                break;
            case Player.PlayerType.FISH:
                text.text = "아버지와의 낚시";
                break;
            case Player.PlayerType.ARMY:
                text.text = "동기들과 행복하게\n웃고있는 군생활";
                break;
            default:
                break;
        }
        panel.SetActive(true);
    }

    public void SelectPlayerType()
    {
        GameDataManager.Instance.SetPlayerType(m_playerType);
        GameStateManager.Instance.LoadInGame();
    }

    public void ExitInfo()
    {
        // info 가 켜져있을 때, 판넬을 클릭하면 종료하게 한다.
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
    }
}
