using UnityEngine;

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

    #region
    public StatusConstant PoisonInfo { get; private set; }
    public StatusConstant BurnInfo { get; private set; }
    public StatusConstant NagInfo { get; private set; }
    public StatusConstant DelayStateInfo { get; private set; }
    public float GraduallyDamageCycle { get; private set; }
    public int GraduallyDamageCountMax { get; private set; }
    public Vector2[] NagDirVector { get; private set; }
    #endregion

    //public static int damageCount = graduallyDamageCycle;
    private void Awake()
    {
        // 독, 화상 데미지 점화식 ( fd / 2 ) ( n + 1 )
        PoisonInfo = new StatusConstant(0.05f, 3f, 3);
        BurnInfo = new StatusConstant(0.05f, 3f, 3);
        NagInfo = new StatusConstant(0.5f, 4f, 2);
        NagDirVector = new Vector2[8]
        { new Vector2(0, 1), new Vector2(0, -1), new Vector2(0, -1), new Vector2(0, 1),
         new Vector2(-1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-1, 0)};
        DelayStateInfo = new StatusConstant(0.5f, 3f, 3);
        GraduallyDamageCycle = 0.1f;
        // (int)(poisonInfo.effectiveTime / graduallyDamageCycle); 왜 29 나오지??
        GraduallyDamageCountMax = 30;
        /*
        DebugX.Log("poison : " + PoisonInfo.value + ", " + PoisonInfo.effectiveTime + ", " + PoisonInfo.overlapCountMax);
        DebugX.Log("burn : " + BurnInfo.value + ", " + BurnInfo.effectiveTime + ", " + BurnInfo.overlapCountMax);
        DebugX.Log("nag : " + NagInfo.value + ", " + NagInfo.effectiveTime + ", " + NagInfo.overlapCountMax);
        DebugX.Log("delayState : " + PoisonInfo.value + ", " + PoisonInfo.effectiveTime + ", " + PoisonInfo.overlapCountMax);
        DebugX.Log("nagDirVector[0] : " + NagDirVector[0] + ", [3] : " + NagDirVector[3]);
        DebugX.Log("지속 데미지 주기 : " + GraduallyDamageCycle);
        DebugX.Log("지속 데미지 횟수 : " + GraduallyDamageCountMax);
        */
    }
}
