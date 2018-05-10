using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponData;
using DelegateCollection;

/*
 * 
 */


// 아직 enum 정의 제대로 안해놓음.

//
//public enum ShotType { Charged, SemiAutoMatic, Automatic, Burst }
//public enum TouchType { Normal, Charged }


public enum WeaponState { Idle, Attack, Reload, Charge, Switch }
public enum Owner { Player, Enemy, Object  }
/* # 
 *  -
 * # WeaponView
 *  - 무기 외형 관리(scale, sprite, animation) 
 */

public class Weapon : MonoBehaviour {

    #region Variables
    public WeaponInfo info;
    public WeaponView weaponView;
    public Animator animator;
    // enum State
    public WeaponState weaponState; // 무기 상태

    private WeaponManager weaponManager;
    private Transform objTransform;
    private SpriteRenderer spriteRenderer;

    private DelGetDirDegree ownerDirDegree;   // 소유자 각도
    private DelGetPosition ownerDirVec; // 소유자 각도 벡터(vector3)
    private DelGetPosition ownerPos;    // 소유자 초기 위치(vector3)

    private float chargedTime;
    private bool canChargedAttack;  // 차징 공격 가능 여부, 초기에 true 상태
    private float ChargedAttackCooldown;        // 차지 공격 쿨타임
    private float chargedDamageIncreaseRate;    // 풀 차징 공격 데미지 상승률
    private Coroutine chargingUpdate;

    private BuffManager ownerBuff;

    // 무기 테스트용
    public int weaponId;
    
    #endregion
    #region getter
    public DelGetDirDegree GetownerDirDegree() { return ownerDirDegree; }
    public DelGetPosition GetOwnerDirVec() { return ownerDirVec; }
    public DelGetPosition GetOwnerPos() { return ownerPos; }
    public BuffManager GetOwnerBuff() { return ownerBuff; }
    public WeaponState GetWeaponState() { return weaponState; }
    #endregion
    #region setter
    public void SetownerDirDegree(DelGetDirDegree ownerDirDegree)
    {
        this.ownerDirDegree = ownerDirDegree;
    }
    public void SetOwnerDirVec(DelGetPosition ownerDirVec)
    {
        this.ownerDirVec = ownerDirVec;
    }
    public void SetOwnerPos(DelGetPosition ownerPos)
    {
        this.ownerPos = ownerPos;
    }

    public void SetOwnerBuff(BuffManager ownerBuff)
    {
        this.ownerBuff = ownerBuff;
    }
    #endregion

    #region UnityFunction

    #endregion
    #region Function
    // 무기 정보 받아오기, weaponView class 초기화
    public void Init(WeaponManager weaponManager)
    {
        weaponState = WeaponState.Idle;

        animator = GetComponent<Animator>();
        objTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        canChargedAttack = true;
        ChargedAttackCooldown = 20f;
        chargedTime = 0f;

        // id에 따른 무기 정보 받아오기
        info = DataStore.Instance.GetWeaponInfo(weaponId);
        weaponView = new WeaponView(objTransform, spriteRenderer);
        weaponView.Init(info.sprite, info.scaleX, info.scaleY);
        
        // 공격 패턴(bulletPattern) 초기화
        for (int i = 0; i < info.bulletPatternsLength; i++)
        {
            //bulletPatterns.Add(info.bulletPatterns[i].Clone());
            info.bulletPatterns[i].Init(this);
        }

        this.weaponManager = weaponManager;

        // 무기 고유 변수들 초기화
        canChargedAttack = true;
        chargedTime = 0;
        if(info.weaponType == WeaponType.Gun || info.weaponType == WeaponType.ShotGun || info.weaponType == WeaponType.Laser)
        {
            chargedDamageIncreaseRate = 0.6f;
        }
        else if(info.weaponType == WeaponType.Blow || info.weaponType == WeaponType.Strike || info.weaponType == WeaponType.Swing)
        {
            chargedDamageIncreaseRate = 0.4f;
        }
    }

    // StartAttack -> Attack -> PatternCycle -> Reload
    // 공격 시도, onwer 따라 다름
    
    // Player : 공격 버튼 터치 했을 때, 함수명 바뀔 예정
    // Enemy  :
    public void StartAttack()
    {
        // 공격 가능 여부 판단 후 공격(탄 부족, 무기 스왑 도중, 공격 딜레이, 기타 공격 불가능 상태)
        if (weaponState == WeaponState.Idle)
        {
            // TouchMode에 따른 다른 행동 Normal : 일반 공격 실행, Charge : 차징
            if (info.touchMode == TouchMode.Normal)
            {
                // 공격 함수 실행
                Attack(0f);
            }
            else if(info.touchMode == TouchMode.Charge && weaponState == WeaponState.Idle)
            {
                weaponState = WeaponState.Charge;
                // 차징 실행
                if (chargingUpdate == null)
                {
                    chargingUpdate = StartCoroutine("ChargingUpdate");
                }
            }
        }
    }

    // 실제 공격 함수, 무기 상태를 Attack으로 바꾸고 공격 패턴 한 사이클 실행.
    public void Attack(float damageIncreaseRate)
    {
        weaponState = WeaponState.Attack;
        // 공격 애니메이션 실행
        // 공격 타입에 따른 공격 실행 (원거리 : 탄 뿌리기 후 cost(탄) 소모)
        PlayAttackAnimation();
        StartCoroutine(PatternCycle(damageIncreaseRate));
    }


    // 공격 시도 중지, onwer랑 공격 방식에 따라 달라짐.
    // player : 공격 버튼 터치 후 땠을 때, 차징 공격같은 경우 오히려 여기서 공격. 함수 명 바뀔 예정
    //          레이저 같은 경우 공격 중지
    public void StopAttack()
    {
        if (info.touchMode == TouchMode.Normal)
        {
            for (int i = 0; i < info.bulletPatternsLength; i++)
            {
                info.bulletPatterns[i].StopAttack();
            }
        }
        else if (info.touchMode == TouchMode.Charge && weaponState == WeaponState.Charge)
        {
            
            //Debug.Log(chargedTime + ", " + info.chargeTime);
            // 차징 중지 후 공격 실행, 풀 차징 시에만 추가 효과 얻음
            if (chargedTime + 0.01f >= info.chargeTime)
            {
                // 풀 차징
                Attack(chargedDamageIncreaseRate);
            }
            else
            {
                // 풀 차징하지 않음
                Attack(0f);
            }
            chargedTime = 0f;
            // 차징 update coroutine 종료
            if(chargingUpdate != null)
            {
                StopCoroutine(chargingUpdate);
                chargingUpdate = null;
            }
            // 차징 UI off
            weaponManager.UpdateChargingUI(0);
        }
    }

    // 재장전
    public void Reload()
    {
        // 공격 딜레이에 재 장전
        StartCoroutine(ReloadTime());
    }

    // 공격 애니메이션 play
    public void PlayAttackAnimation()
    {        
        // 근거리는 휘두르기, 찌르기 애니메이션
        // 총은 반동 주기
        switch (info.attackAniType)
        {
            case AttackAniType.Blow:
                animator.SetTrigger("blow");
                break;
            case AttackAniType.Strike:
                animator.SetTrigger("strike");
                break;
            case AttackAniType.Shot:
                animator.SetTrigger("shot");
                break;
            default:
                break;
        }
    }

    // 공격 패턴 한 사이클.
    private IEnumerator PatternCycle(float damageIncreaseRate)
    {
        // 공격 한 사이클 실행
        for (int i = 0; i < info.bulletPatternsLength; i++)
        {
            for(int j = 0; j < info.bulletPatterns[i].GetExeuctionCount(); j++)
            {
                info.bulletPatterns[i].StartAttack(damageIncreaseRate);
                if(info.bulletPatterns[i].GetDelay() > 0)
                {
                    yield return YieldInstructionCache.WaitForSeconds(info.bulletPatterns[i].GetDelay());
                }
            }
        }
        Reload(); 
    } 

    // 재장전 코루틴, cooldown 시간 이후의 WeaponState가 Idle로 됨.
    private IEnumerator ReloadTime()
    {
        weaponState = WeaponState.Reload;
        yield return YieldInstructionCache.WaitForSeconds(info.cooldown);
        weaponState = WeaponState.Idle;
    }

    // 차징 시작
    private IEnumerator ChargingUpdate()
    {
        // 2.019999, 2와 같은 부동 소수점 계산 문제 생겨서 info.chargeTime에다가 WaitForseconds / 2 값 만큼 빼줬음 
        while (chargedTime < info.chargeTime - 0.01f)
        {   
            chargedTime += 0.02f;
            weaponManager.UpdateChargingUI(chargedTime / info.chargeTime);
            yield return YieldInstructionCache.WaitForSeconds(0.02f);
        }
    }

    // 차징 공격 쿨타임 코루틴, 20초
    private IEnumerator EnterChargingAttackCooldown()
    {
        canChargedAttack = false;
        yield return YieldInstructionCache.WaitForSeconds(ChargedAttackCooldown);
        canChargedAttack = true;
    }
    #endregion
}