using UnityEngine;

public struct StatusConstant
{
    public float value;         // 각 상태이상 별로 세기, 값
    public float effectiveTime; // 효과 유지 시간
    public int overlapCountMax; // 중첩 횟수 최대치
    public float range;
    public StatusConstant(float value, float effectiveTime, int overlapCountMax, float range = 0)
    {
        this.value = value;
        this.effectiveTime = effectiveTime;
        this.overlapCountMax = overlapCountMax;
        this.range = range;
    }
}


public class StatusConstants : MonoBehaviourSingleton<StatusConstants>
{
    /*
    private StatusConstant poisonInfo;
    private StatusConstant burnInfo;
    //private StatusConstant nagInfo;
    private StatusConstant delayStateInfo;
    private float graduallyDamageCycle;
    private int graduallyDamageCountMax;
    private Vector2[] nagDirVector;
    */

    #region property
    public StatusConstant PoisonInfo { get; private set; }
    public StatusConstant BurnInfo { get; private set; }
    public StatusConstant DelayStateInfo { get; private set; }
    public StatusConstant NagInfo { get; private set; }
    public StatusConstant ClimbingInfo { get; private set; }
    public StatusConstant GraveyardShiftInfo { get; private set; }
    public StatusConstant FreezeInfo { get; private set; }
    public StatusConstant ReactanceInfo { get; private set; }

    public float GraduallyDamageCycle { get; private set; }
    public int GraduallyDamageCountMax { get; private set; }
    public float GraduallyDamagePerUnit { get; private set; }
    #endregion

    //public static int damageCount = graduallyDamageCycle;
    private void Awake()
    {
        // 독, 화상 데미지 점화식 ( fd / 2 ) ( n + 1 )
        PoisonInfo = new StatusConstant(0.05f, 2f, 4);
        BurnInfo = new StatusConstant(0.05f, 2f, 4);

        FreezeInfo = new StatusConstant(0f, 1f, -1);

        GraduallyDamageCycle = 0.1f;
        // (int)(poisonInfo.effectiveTime / graduallyDamageCycle); 왜 30안나오고 29 나오지?
        GraduallyDamageCountMax = 30;
        // 1 / 1초당 처리 회수 => 지속 데미지 1회 적용 당 데미지 비율
        GraduallyDamagePerUnit = GraduallyDamageCycle / 1f;
        /*
        Debug.Log("poison : " + PoisonInfo.value + ", " + PoisonInfo.effectiveTime + ", " + PoisonInfo.overlapCountMax);
        Debug.Log("burn : " + BurnInfo.value + ", " + BurnInfo.effectiveTime + ", " + BurnInfo.overlapCountMax);
        Debug.Log("nag : " + NagInfo.value + ", " + NagInfo.effectiveTime + ", " + NagInfo.overlapCountMax);
        Debug.Log("delayState : " + PoisonInfo.value + ", " + PoisonInfo.effectiveTime + ", " + PoisonInfo.overlapCountMax);
        Debug.Log("nagDirVector[0] : " + NagDirVector[0] + ", [3] : " + NagDirVector[3]);
        Debug.Log("지속 데미지 주기 : " + GraduallyDamageCycle);
        Debug.Log("지속 데미지 횟수 : " + GraduallyDamageCountMax);
        */
    }
}

/*
 * 상태이상 처리
 * 
 * 공격계 (독, 화상)
 *  - 상태이상 공격 텀 A초 고정(현재 A = 0.5)
 *  - 한 틱당 공격력 D, 총 N회 적용(N = 6 고정, 총 3초 걸리는 꼴)
 *  - 중첩 되려고 할 때 처리, (기존 상태이상 공격력 * 남은 적용 횟수) 와 (새로운 상태이상 공격력 * 적용 횟수)의 합 중에서
 *  - 높은 걸 적용????
 * 
 * 제어계 (스턴, 빙결)
 *  - n초 적용
 *  - 새로운 것이 들어왔을 때 기존 상태이상에 상관없이 초기화 되어 새로운 상태이상으로 적용 
 */ 

static class StatusConstantTest
{
    public const float STATUS_ATTACKING_TERM = 0.5f;
    public const int STATUS_ATTACKING_COUNT_MAX = 6;
}
