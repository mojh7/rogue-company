using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolObjectContoller : MonoBehaviour {

    bool isInit = false;
    bool isPatrol;
    Vector3 src, dest;
    [SerializeField]
    Vector3 destVector;
    [SerializeField]
    float period;
    Coroutine routine;

    Vector3 nullVector;

    public void SetDestVector(Vector3 src ,Vector3 destVector)
    {
        this.isInit = true;
        this.destVector = destVector;
        isPatrol = false;
        this.src = src;
        dest = src + destVector;
        Patrol();
    }

    public void ReAlign()
    {
        isPatrol = false;
        if (!isInit)
            src = this.transform.localPosition;
        dest = src + destVector;
        Patrol();
    }

    private void OnEnable()
    {
        isPatrol = false;
        if(!isInit)
            src = this.transform.localPosition;
        dest = src + destVector;
        Patrol();
    }
    private void OnDisable()
    {
        isPatrol = false;
        StopCoroutine(routine);
    }
    private void OnDestroy()
    {
        isPatrol = false;
        StopCoroutine(routine);
    }
    void Patrol()
    {
        if (isPatrol)
            return;
        isPatrol = true;
        routine = StartCoroutine(CoroutinePatrol(src, dest, period));
    }

    IEnumerator CoroutinePatrol(Vector3 src, Vector3 dest, float period)
    {
        float periodHalf = period * .5f;
        float time = 0;
        while (isPatrol)
        {
            time += Time.deltaTime;
            if(time >= period)
            {
                time -= period;
            }
            else if (time > periodHalf)
            {
                transform.localPosition = Vector3.Lerp(dest, src, (time - periodHalf) / periodHalf);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(src, dest, time / periodHalf);
            }
            yield return YieldInstructionCache.WaitForSeconds(.05f);
        }
    }
}
