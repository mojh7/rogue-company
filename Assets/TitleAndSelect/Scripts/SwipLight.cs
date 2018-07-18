using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipLight : MonoBehaviour {

    [SerializeField]
    Vector3 leftPos, rightPos;
    Vector3 centerPos;
    [SerializeField]
    float time;
    bool isToLeft;
    float elapsedTime;


    private void Start()
    {
        elapsedTime = 0;
        centerPos = new Vector3((leftPos + rightPos).x, 0.5f * leftPos.y, leftPos.z);
        isToLeft = true;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        Swip();
        if(elapsedTime > time)
        {
            elapsedTime = 0;
            isToLeft = !isToLeft;
        }
    }
    void Swip()
    {
        if(isToLeft)
        {
            this.transform.position = BezierCurve(elapsedTime / time, leftPos, centerPos, rightPos);
        }
        else
        {
            this.transform.position = BezierCurve(elapsedTime / time, rightPos, centerPos, leftPos);
        }
    }

    Vector3 BezierCurve(float t, Vector3 p0, Vector3 p1)
    {
        return ((1 - t) * p0) + ((t) * p1);
    }

    Vector3 BezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector3 pa = BezierCurve(t, p0, p1);
        Vector3 pb = BezierCurve(t, p1, p2);
        return BezierCurve(t, pa, pb);
    }
}
