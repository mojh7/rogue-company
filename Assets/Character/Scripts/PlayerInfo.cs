using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 캐릭터 여러 개 만들 때 방법 및 관리 고민
 * 
 * 1. Player 클래스 상속 받은 자식 클래스 여러개 사용, 캐릭터(음악, 운동, 군인 등등)하나 하나씩 자식 클래스로 두어서 관리
 * 
 * 2. Player Class 한 개로만 쓰고 따로 Player Info 클래스로 만들어서 scriptable class로 정보 관리
 * 
 * 3. 1 + 2번 혼합 방식
 */ 


public class PlayerInfo : ScriptableObject {

    /*
     * playerType ( 군인, 밴드, 낚시, 축구 ) 
     * 
     * PlayerSprite
     * 
     * 체력 (최대 체력 개념이 있어야하나 없어야 하나)
     * hp
     * 
     * 이동속도
     * moveSpeed
     * 
     * 허기
     * Hunger
     * 
     * 패시브
     * passiveItem;
     * 
     * 액티브
     * activeSkill
     * 
     * 첫 시작 기본 무기
     * startingWeaponId
     * 
     * 운 (= 크리티컬 확률 ?)
     * ciriticalChance
     * 
     * 
     */
}
