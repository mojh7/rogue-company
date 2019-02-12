using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyingManager : MonoBehaviourSingleton<BuyingManager>
{
    private static readonly int[] CHARACTER_PRICE = new int[] { 0, 150, 200, 300 };

    public enum BuyingType { NONE, CHARACTER };

    // ui
    [SerializeField]
    private GameObject buyingUI;
    [SerializeField]
    private Text descriptionTxt;
    [SerializeField]
    private Text goldTxt;
    [SerializeField]
    private Text priceTxt;

    private BuyingType buyingType;
    private Player.PlayerType playerType;

    private const string BUYING_GUIDANCE_TEXT = " 정말 구매하시겠습니까?";
    private string description;

    // value
    [SerializeField]
    private int price;
    [SerializeField]
    

    private void Awake()
    {
        buyingUI.SetActive(false);
    }

    public void ShowCharacterBuying(Player.PlayerType type)
    {
        buyingType = BuyingType.CHARACTER;
        playerType = type;
        price = CHARACTER_PRICE[(int)type];
        description = Player.PLAYER_TYPE_NAME[(int)type] + " 캐릭터를" + BUYING_GUIDANCE_TEXT;
        ShowBuyingUI();
    }

    public void BuyingConfirmed()
    {
        switch(buyingType)
        {
            case BuyingType.CHARACTER:
                if (false == GameDataManager.Instance.UseMoeny(price))
                {
                    BuyingFailed();
                    return;
                }
                CharacterSelect.Instance.UnlockCharacter(playerType);
                CharacterSelectInfoView.Instance.ShowUnlockState();
                NoticeManager.Instance.ShowNotice(Player.PLAYER_TYPE_NAME[(int)playerType] + " 캐릭터가 구매되었습니다.");
                CloseBuyingUI();
                break;
            default:
                break;
        }
    }

    public void BuyingFailed()
    {
        // TODO : 돈 부족 sfx
        NoticeManager.Instance.ShowNotice("포인트가 부족합니다.");
    }

    public void UpdateCurrentGold()
    {
        goldTxt.text = GameDataManager.Instance.GetGold().ToString();
    }

    public void ShowBuyingUI()
    {
        UpdateCurrentGold();
        descriptionTxt.text = description;
    }

    public void CloseBuyingUI()
    {
        buyingUI.SetActive(false);
    }
}
