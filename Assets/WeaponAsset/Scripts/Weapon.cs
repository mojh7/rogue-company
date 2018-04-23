using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponData;
using DelegateCollection;


// 아직 enum 정의 제대로 안해놓음.

//public enum WeaponType { Blow, Strike, Swing, Gun, laser }
//public enum ShotType { Charged, SemiAutoMatic, Automatic, Burst }
//public enum TouchType { Normal, Charged }


public enum WeaponState { Idle, Attack, Reload, Charge, Switch }

/* # 
 *  -
 * # WeaponView
 *  - 무기 외형 관리(scale, sprite, animation) 
 */

public class Weapon : MonoBehaviour {

    #region Variables
    public WeaponInfo info;
    public WeaponView weaponView;
    public Animator ani;
    // enum State
    public WeaponState weaponState; // 무기 상태
    public TouchType touchType;     // 터치 타입 일반, 차징 공격

    private Transform objTransform;
    private SpriteRenderer spriteRenderer;
    private DelGetDirDegree ownerDirDegree;   // 소유자 각도
    private DelGetPosition ownerDirVec; // 소유자 각도 벡터(vector3)
    private DelGetPosition ownerPos;    // 소유자 초기 위치(vector3)
    private List<BulletPattern> bulletPatterns;
    private int bulletPatternsLength;

    // 무기 테스트용
    public int weaponId;
    
    #endregion
    #region getter
    public DelGetDirDegree GetownerDirDegree() { return ownerDirDegree; }
    public DelGetPosition GetOwnerDirVec() { return ownerDirVec; }
    public DelGetPosition GetOwnerPos() { return ownerPos; }
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
    #endregion

    #region UnityFunction
    void Awake()
    {
        //touchType = TouchType.Normal;
        weaponState = WeaponState.Idle;

        objTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bulletPatterns = new List<BulletPattern>();
        weaponView = new WeaponView(objTransform, spriteRenderer);
    }

    #endregion
    #region Function
    // 무기 정보 받아오기, weaponView class 초기화
    public void Init()
    {   
        info = DataStore.Instance.GetWeaponInfo(weaponId);
        weaponView.Init(DataStore.Instance.GetWeaponSprite(info.spriteId), info.scaleX, info.scaleY);
        bulletPatternsLength = info.originalBulletPatterns.Length;
        // 공격 패턴(bulletPattern) 초기화
        for (int i = 0; i < bulletPatternsLength; i++)
        {
            bulletPatterns.Add(info.originalBulletPatterns[i].Clone());
            bulletPatterns[i].Init(this);
        }
    }

    // StartAttack -> Attack -> PatternCycle -> Reload
    // 공격 시도, onwer 따라 다름
    // Player : 공격 버튼 터치 했을 때, 함수명 바뀔 예정
    // Enemy  :
    public void StartAttack()
    {
        if (weaponState == WeaponState.Idle)
        {
            // 공격 함수 실행
            weaponState = WeaponState.Attack;
            Attack();
        }
    }

    // 실제 공격 함수, 무기 상태를 Attack으로 바꾸고 공격 패턴 한 사이클 실행.
    public void Attack()
    {
        // 공격 가능 여부 판단 후 공격(탄 부족, 무기 스왑 도중, 공격 딜레이, 기타 공격 불가능 상태)
        // 공격 애니메이션 실행
        // 공격 타입에 따른 공격 실행 (원거리 : 탄 뿌리기 후 cost(탄) 소모)
        ani.SetInteger("AttackType", 3);
        StartCoroutine(PatternCycle());
    }

    // 공격 시도 중지, onwer랑 공격 방식에 따라 달라짐.
    // player : 공격 버튼 터치 후 땠을 때, 차징 공격같은 경우 오히려 여기서 공격. 함수 명 바뀔 예정
    //          레이저 같은 경우 공격 중지
    public void StopAttack()
    {
        for (int i = 0; i < bulletPatternsLength; i++)
        {
            bulletPatterns[i].StopAttack();
        }
    }

    // 재장전
    public void Reload()
    {
        // 공격 딜레이에 재 장전
        StartCoroutine(ReloadTime());
    }

    // 공격 애니메이션
    public void AttackAnimation()
    {
        // 근거리는 휘두르기, 찌르기 애니메이션
        // 총은 반동 주기
    }

    // 공격 패턴 한 사이클.
    private IEnumerator PatternCycle()
    {
        // 공격 한 사이클 실행
        for (int i = 0; i < bulletPatternsLength; i++)
        {
            for(int j = 0; j < bulletPatterns[i].GetExeuctionCount(); j++)
            {
                bulletPatterns[i].StartAttack();
                if(bulletPatterns[i].GetDelay() > 0)
                {
                    yield return YieldInstructionCache.WaitForSeconds(bulletPatterns[i].GetDelay());
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
        ani.SetInteger("AttackType", 0);
    }
    #endregion
}