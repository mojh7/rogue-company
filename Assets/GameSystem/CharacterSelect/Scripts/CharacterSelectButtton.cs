using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButtton : MonoBehaviour
{
    

    private Player.PlayerType playerType;
    [SerializeField]
    private Image folderImage;
    [SerializeField]
    private Image charcterImage;
    [SerializeField]
    private Text characterNameText;
    [SerializeField]
    private GameObject characterTextBkg;

    public void Init(Player.PlayerType playerType, Sprite folderSprite, Sprite characterSprite)
    {
        characterTextBkg.SetActive(false);
        this.playerType = playerType;
        characterNameText.text = Player.PLAYER_TYPE_NAME[(int)playerType];
        folderImage.sprite = folderSprite;
        charcterImage.sprite = characterSprite;
    }

    public void UnSelectCharacterBtn()
    {
        characterTextBkg.SetActive(false);
        characterNameText.color = Color.black;
    }

    public void SelectCharcter()
    {
        CharacterSelect.Instance.unSelectAllCharacterBtn();
        characterTextBkg.SetActive(true);
        characterNameText.color = Color.white;
        CharacterSelectInfoView.Instance.ShowCharacterInfoView(playerType);
    }
}
