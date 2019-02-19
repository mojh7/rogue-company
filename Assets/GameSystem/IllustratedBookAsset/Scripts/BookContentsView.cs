using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeaponAsset;

/*
 * 무기 이름, 공격력, 쿨타임, 탄약,
 * 
 * 
 */

public class BookContentsView : MonoBehaviourSingleton<BookContentsView>
{

    [SerializeField]
    private Image contentImg;
    [SerializeField]
    private Text contentInfoTxt;
    [SerializeField]
    private Text descriptionTxt;

    private string[] weaponTypeString = new string[]
    {
        "권총", "산탄총", "기관총", "저격 소총", "레이저", "활",
        "창", "몽둥이", "스포츠용품", "검", "청소 도구", "주먹 장착 무기",
        "폭발형 무기", "", "지팡이", "근거리 특수무기", "원거리 특수무기"
    };

    public void ShowContentsInfo(WeaponInfo info)
    {

        contentImg.sprite = info.sprite;

        contentInfoTxt.text =
            "이름 : " + info.weaponName +
            "\n종류 : " + weaponTypeString[(int)info.weaponType - 1] +
            "\n등급 : " + info.rating +
            "\nDPS : " + info.dps +
            "\n공격 속도 : " + info.attackSpeed +
            "\n치명타 확률 : " + (info.criticalChance*100) + " %";

        // 근거리, 소요 스태미나 표시
        if (Weapon.IsMeleeWeapon(info))
        {
            contentInfoTxt.text += "\n소모 스태미나 : " + info.staminaConsumption;
        }
        // 원거리 탄창
        else
        {
            contentInfoTxt.text += "\n탄약량 : " + ((info.ammoCapacity == -1) ? "∞" : info.ammoCapacity.ToString());
        }

        // 보류
        /*
        if ("" == info.description)
            descriptionTxt.text = "무기 설명 필요함";
        else
            descriptionTxt.text = "설명 : " + info.description;
        */
    }

    public void ShowContentsInfo(UsableItemInfo info)
    {

        contentImg.sprite = info.Sprite;

        contentInfoTxt.text =
            "이름 : " + info.ItemName +
            "\n등급 : " + info.Rating +
            "\n설명 : " + info.Notes +
            "\n\n# 비밀 노트 : " + info.Memo;
    }

}
