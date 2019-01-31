using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelect : MonoBehaviourSingleton<CharacterSelect>
{

    [SerializeField]
    private GameObject selectWindowUI;

    [SerializeField]
    private Transform contentsParentObj;
    [SerializeField]
    private GameObject characterSelectBtnPrefab;

    private CharacterSelectButtton[] characterSelectBtnList;

    #region unityFunc
    void Awake()
    {
        Init();
    }
    #endregion

    #region func

    public void OpenCharacterSelectWindow()
    {
        selectWindowUI.SetActive(true);
        characterSelectBtnList[0].SelectCharcter();
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    public void CloseCharacterSelectWindow()
    {
        selectWindowUI.SetActive(false);
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    private void Init()
    {
        int length = (int)Player.PlayerType.END;
        characterSelectBtnList = new CharacterSelectButtton[length];
        GameObject createdobj;
        // weapon contents 생성
        for (int i = length - 1; i >= 0; i--)
        {
            createdobj = Instantiate(characterSelectBtnPrefab);
            createdobj.name = "characterSelect_" + i;
            createdobj.transform.SetParent(contentsParentObj);
            characterSelectBtnList[i] = createdobj.GetComponent<CharacterSelectButtton>();
            characterSelectBtnList[i].Init((Player.PlayerType)i);
            createdobj.transform.localScale = new Vector3(1, 1, 1);
        }
        selectWindowUI.SetActive(false);
    }

    public void SelectCharacter(Player.PlayerType playerType)
    {
        GameDataManager.Instance.SetPlayerType(playerType);
        GameStateManager.Instance.LoadInGame();
    }
    #endregion
}
