using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectInfoView : MonoBehaviourSingleton<CharacterSelectInfoView>
{
    [SerializeField]
    private Text characterTypeName;

    private Player.PlayerType playerType;

    public void ShowCharacterInfoView(Player.PlayerType playerType)
    {
        this.playerType = playerType;
        int type = (int)playerType;
        characterTypeName.text = Player.PLAYER_TYPE_NAME[type];
    }

    public void SelectCharacter()
    {
        CharacterSelect.Instance.SelectCharacter(playerType);
    }
}
