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
    [SerializeField]
    private GameObject lockImg;

    private bool unlock;

    public void Init(Player.PlayerType playerType, Sprite folderSprite, Sprite characterSprite, bool unlock)
    {
        characterTextBkg.SetActive(false);
        this.playerType = playerType;
        characterNameText.text = Player.PLAYER_TYPE_NAME[(int)playerType];
        folderImage.sprite = folderSprite;
        charcterImage.sprite = characterSprite;
        if(unlock == true)
        {
            UnLockCharacter();
        }
        else
        {
            LockCharacter();
        }
    }

    public void LockCharacter()
    {
        unlock = false;
        lockImg.SetActive(true);
        folderImage.color = Color.black;
        charcterImage.color = Color.black;
    }

    public void UnLockCharacter()
    {
        unlock = true;
        lockImg.SetActive(false);
        folderImage.color = Color.white;
        charcterImage.color = Color.white;
    }


    public void UnSelectCharacterBtn()
    {
        characterTextBkg.SetActive(false);
        characterNameText.color = Color.black;
    }

    public void SelectCharcter()
    {
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
        CharacterSelect.Instance.unSelectAllCharacterBtn();
        characterTextBkg.SetActive(true);
        characterNameText.color = Color.white;
        CharacterSelectInfoView.Instance.ShowCharacterInfoView(playerType, unlock);
    }
}
