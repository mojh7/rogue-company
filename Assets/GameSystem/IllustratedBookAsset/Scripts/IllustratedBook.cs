using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

// https://www.google.com/search?q=%EB%A1%9C%EB%93%9C%EC%98%A4%EB%B8%8C%EB%8D%98%EC%A0%84&source=lnms&tbm=isch&sa=X&ved=0ahUKEwiGu_ea5PbfAhVMZt4KHSxzBPIQ_AUIDigB&biw=767&bih=744&dpr=1.25#imgrc=-i46cM91R3lJUM:
// 선택 할 수 있는 공간 55~65%, 보여주는 쪽 30~35% 정도 크기

// 무기랑 아이템은 고정, 로드오브던전 항목별로 도감 하나씩 있음.
// 무기 도감, 아이템 도감 따로 하면 무기 도감에서 탭을 무기 타입별로 둬도 되긴 하는데
// 무기 타입이 많아서 가로 길이 넘 길어짐.

// 초기의 ? 모양으로 해주고 던전에서 첫 등장시 혹은 처음 사용시 도감에서 ? 해제하여
// 상세내용 보여주기

// info view 쪽
// 이름, 탄약량, 레이팅, 무기 타입, 쿨타임?, 공격력(dps로 보여주는게 나을듯)
// + 부가 설명(없어도 되고 있어도 될 것 같음)

// 도감, 콜렉팅 기능 분리 혹은 도감내에 포함. 아마 분리 시킬듯?
/*
 * TODO : 초기화, 분류(weapon, item)별 다르게 보여주기, 해당 콘텐츠 내용 view, 
 * 해당 분류에서 구분해서 보여주기(S~E rating 별로 하던가, 등급 오름 내린차순 둘중 하나)
 */

public class IllustratedBook : MonoBehaviour
{
    public enum IllustratedBookType { WEAPON, ITEM, MONSTER, BOSS_MONSTER }
    public enum BookSortingType { ALL_RATING, S, A, B, C , D, E }   // 구분 어떻게 보여줄지

    #region variables
    [SerializeField]
    private GameObject bookUI;
    [SerializeField]
    private Transform contentsParentObj;
    [SerializeField]
    private GameObject contentsPrefab;

    private IllustratedBookType illustratedBookType;
    private BookSortingType bookSortingType;
    private IllustratedBookContents[] weaponContentsList;
    private List<int>[] weaponIndexbyRating;
    private IllustratedBookContents[] itemContentsList;


    #endregion

    #region unityfunc
    void Awake()
    {
        InitBook();
    }
    #endregion

    #region func
    public void OpenBook()
    {
        bookUI.SetActive(true);
    }
    public void CloseBook()
    {
        bookUI.SetActive(false);
    }

    private void InitBook()
    {
        illustratedBookType = IllustratedBookType.WEAPON;
        bookSortingType = BookSortingType.ALL_RATING;
        weaponContentsList = new IllustratedBookContents[WeaponsData.Instance.GetWeaponInfosLength()];
        itemContentsList = new IllustratedBookContents[ItemsData.Instance.GetMiscItemInfosLength()];

        int ratingLength = (int)Rating.E;
        weaponIndexbyRating = new List<int>[ratingLength];
        
        GameObject createdobj;
        // weapon contents 생성
        for (int i = 0; i < WeaponsData.Instance.GetWeaponInfosLength(); i++)
        {
            createdobj = Instantiate(contentsPrefab);
            createdobj.name = "weaponContents_" + i;
            createdobj.transform.SetParent(contentsParentObj);
            weaponContentsList[i] = createdobj.GetComponent<IllustratedBookContents>();
            weaponContentsList[i].Init(WeaponsData.Instance.GetWeaponInfo(i, CharacterInfo.OwnerType.PLAYER));
            createdobj.transform.localScale = new Vector3(1, 1, 1);
        }

        bookUI.SetActive(false);
    }

    private void ChangeCategory(int type)
    {
        illustratedBookType = (IllustratedBookType)type;
    }

    public void ShowContentsInfo()
    {
        switch (illustratedBookType)
        {
            case IllustratedBookType.WEAPON:
                break;
            default:
                break;
        }
    }
    #endregion
}