using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButtton : MonoBehaviour
{
    
    private Player.PlayerType playerType;
    [SerializeField]
    private Text characterNameText;

    public void Init(Player.PlayerType playerType)
    {
        this.playerType = playerType;
        characterNameText.text = Player.PLAYER_TYPE_NAME[(int)playerType];
    }

    public void SelectCharcter()
    {
        CharacterSelectInfoView.Instance.ShowCharacterInfoView(playerType);
    }
}
