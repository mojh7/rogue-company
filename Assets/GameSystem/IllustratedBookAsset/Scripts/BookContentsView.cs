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
    private Image weaponImg;
    [SerializeField]
    private Text weaponInfoTxt;
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
    
        weaponImg.sprite = info.sprite;

        weaponInfoTxt.text =
            "무기 이름 : " + info.weaponName + "\n무기 종류 : " + weaponTypeString[(int)info.weaponType - 1] +
            "\nDPS       : " + "추후에 추가" +
            "\n공격 속도 : " + "추후에 추가" +
            "\n치명타 확률 : " + (info.criticalChance*100) + " %";

        // 근거리, 소요 스태미나 표시
        if (WeaponType.SPEAR == info.weaponType || WeaponType.CLUB == info.weaponType || WeaponType.SPORTING_GOODS == info.weaponType ||
            WeaponType.SWORD == info.weaponType || WeaponType.CLEANING_TOOL == info.weaponType || WeaponType.KNUCKLE == info.weaponType ||
            WeaponType.MELEE_SPECIAL == info.weaponType)
        {
            weaponInfoTxt.text += "\n소요 스태미나 : " + info.staminaConsumption;
        }
        // 원거리 집탄률 표시
        else if (WeaponType.PISTOL == info.weaponType || WeaponType.SHOTGUN == info.weaponType || WeaponType.MACHINEGUN == info.weaponType ||
            WeaponType.SNIPER_RIFLE == info.weaponType || WeaponType.LASER == info.weaponType || WeaponType.BOW == info.weaponType ||
            WeaponType.WAND == info.weaponType || WeaponType.RANGED_SPECIAL == info.weaponType || WeaponType.BOMB == info.weaponType)
        {
            weaponInfoTxt.text += "\n집탄률    : " + "추후에 추가";
        }

        if ("" == info.description)
            descriptionTxt.text = "무기 설명 필요함";
        else
            descriptionTxt.text = "설명 : " + info.description;

    }

}
