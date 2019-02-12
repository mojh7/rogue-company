using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectInfoView : MonoBehaviourSingleton<CharacterSelectInfoView>
{
    [SerializeField]
    private GameObject infoViewUI;
    [SerializeField]
    private Text characterTypeName;
    [SerializeField]
    private Text statsText;
    [SerializeField]
    private Text descriptionText;
    [SerializeField]
    private GameObject startBtn;
    [SerializeField]
    private GameObject buyBtn;
    private Player.PlayerType playerType;

    private readonly string[] CHARACTER_DESCRIPTION = new string[]
    {
        "대학교 축구 동아리에서의 추억", "대학교 시절 밴드 활동", "아버지와의 낚시", "동기들과 행복하게 웃고있는 군생활"
    };
    private PlayerData playerData;

    private void Awake()
    {
        infoViewUI.SetActive(false);
    }

    public void ShowStartState()
    {
        characterTypeName.text = Player.PLAYER_TYPE_NAME[(int)playerType];
        startBtn.SetActive(true);
        buyBtn.SetActive(false);
    }

    public void ShowBuyState()
    {
        characterTypeName.text = Player.PLAYER_TYPE_NAME[(int)playerType] + " (잠금)";
        startBtn.SetActive(false);
        buyBtn.SetActive(true);
    }

    public void ClickBuyBtn()
    {
        // 캐릭터 구매 버튼 클릭
        Debug.Log("구매 버튼 클릭");
    }

    public void ShowCharacterInfoView(Player.PlayerType playerType, bool unlock)
    {
        infoViewUI.SetActive(true);
        this.playerType = playerType;
        int type = (int)playerType;
        playerData = GameDataManager.Instance.GetPlayerData(playerType);
        statsText.text = "HP       : " + playerData.HpMax + "\nSTAMINA  : " + playerData.StaminaMax + "\nCRITICAL : " + (playerData.CriticalChance*100) + "%";
        descriptionText.text = CHARACTER_DESCRIPTION[type];
        if (unlock)
            ShowStartState();
        else
            ShowBuyState();
    }

    public void SelectCharacter()
    {
        CharacterSelect.Instance.SelectCharacter(playerType);
    }

    public void CloseSelectInfoView()
    {
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
        infoViewUI.SetActive(false);
    }
}
