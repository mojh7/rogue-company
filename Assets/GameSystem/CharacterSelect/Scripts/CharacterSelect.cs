using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelect : MonoBehaviourSingleton<CharacterSelect>
{
    public delegate void UnSelectAllCharacterBtn();
    public UnSelectAllCharacterBtn unSelectAllCharacterBtn;


    [SerializeField]
    private GameObject selectWindowUI;

    [SerializeField]
    private Transform contentsParentObj;
    [SerializeField]
    private GameObject characterSelectBtnPrefab;
    private CharacterSelectButtton[] characterSelectBtnList;

    private Dictionary<string, bool> characterUnLockState;

    [Header("character button 생성을 위한 변수")]
    [SerializeField]
    private Sprite[] folderSprites;
    [SerializeField]
    private Sprite[] characterSprites;


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
        unSelectAllCharacterBtn();
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    public void CloseCharacterSelectWindow()
    {
        selectWindowUI.SetActive(false);
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    private void Init()
    {
        GameDataManager.Instance.LoadData(GameDataManager.UserDataType.USER);

        int length = (int)Player.PlayerType.END;
        characterSelectBtnList = new CharacterSelectButtton[length];
        GameObject createdobj;
        // weapon contents 생성
        for (int i = 0; i < length; i++)
        {
            createdobj = Instantiate(characterSelectBtnPrefab);
            createdobj.name = "characterSelect_" + i;
            createdobj.transform.SetParent(contentsParentObj);
            characterSelectBtnList[i] = createdobj.GetComponent<CharacterSelectButtton>();
            characterSelectBtnList[i].Init((Player.PlayerType)i, folderSprites[i], characterSprites[i]);
            unSelectAllCharacterBtn += characterSelectBtnList[i].UnSelectCharacterBtn;
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
