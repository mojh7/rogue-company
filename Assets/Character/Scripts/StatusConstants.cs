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
    #endregion

    //public static int damageCount = graduallyDamageCycle;
    private void Awake()
    {
        // 독, 화상 데미지 점화식 ( fd / 2 ) ( n + 1 )
        PoisonInfo = new StatusConstant(0.05f, 2f, 4);
        BurnInfo = new StatusConstant(0.05f, 2f, 4);
        DelayStateInfo = new StatusConstant(0.5f, 3f, 3);

        NagInfo = new StatusConstant(0.5f, 4f, 2, 5f);
        ClimbingInfo = new StatusConstant(0.45f, 2.5f, 2, 4f);
        GraveyardShiftInfo = new StatusConstant(0f, 2f, 2);
        FreezeInfo = new StatusConstant(0f, 2f, -1);
        ReactanceInfo = new StatusConstant(0f, 4f, -1);

        GraduallyDamageCycle = 0.1f;
        // (int)(poisonInfo.effectiveTime / graduallyDamageCycle); 왜 30안나오고 29 나오지?
        GraduallyDamageCountMax = 20;
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
