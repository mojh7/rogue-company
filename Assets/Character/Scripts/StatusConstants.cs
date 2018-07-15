using UnityEngine;

public class StatusConstants : MonoBehaviourSingleton<StatusConstants>
{
    private StatusConstant poisonInfo;
    private StatusConstant burnInfo;
    private StatusConstant nagInfo;
    private StatusConstant delayStateInfo;
    private float graduallyDamageCycle;
    private int graduallyDamageCountMax;
    private Vector2[] nagDirVector;

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
        poisonInfo = new StatusConstant(0.05f, 3f, 3);
        burnInfo = new StatusConstant(0.05f, 3f, 3);
        nagInfo = new StatusConstant(0.5f, 4f, 2);
        nagDirVector = new Vector2[8]
        { new Vector2(0, 1), new Vector2(0, -1), new Vector2(0, -1), new Vector2(0, 1),
         new Vector2(-1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-1, 0)};
        delayStateInfo = new StatusConstant(0.5f, 3f, 3);
        graduallyDamageCycle = 0.1f;
        // (int)(poisonInfo.effectiveTime / graduallyDamageCycle); 왜 29 나오지??
        graduallyDamageCountMax = 30;
        /*
        Debug.Log("poison : " + poisonInfo.value + ", " + poisonInfo.effectiveTime + ", " + poisonInfo.overlapCountMax);
        Debug.Log("burn : " + burnInfo.value + ", " + burnInfo.effectiveTime + ", " + burnInfo.overlapCountMax);
        Debug.Log("nag : " + nagInfo.value + ", " + nagInfo.effectiveTime + ", " + nagInfo.overlapCountMax);
        Debug.Log("delayState : " + poisonInfo.value + ", " + poisonInfo.effectiveTime + ", " + poisonInfo.overlapCountMax);
        Debug.Log("nagDirVector[0] : " + nagDirVector[0] + ", [3] : " + nagDirVector[3]);
        Debug.Log("지속 데미지 주기 : " + graduallyDamageCycle);
        Debug.Log("지속 데미지 횟수 : " + graduallyDamageCountMax);
        */
    }
}
