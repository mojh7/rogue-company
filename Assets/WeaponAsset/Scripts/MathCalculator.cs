using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각종 수학적 계산 모음 class
/// </summary>
public static class MathCalculator  {

    private const float DegToRad = Mathf.PI / 180;

    /// <summary> 
    /// Vector 값을 각도로 변환하여 반환한다.
    /// </summary>
    public static float GetDegFromVector(this Vector3 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }


    /// <summary>
    /// Vector값을 degree만큼 회전한 새로운 Vector3 값을 반환한다.
    /// </summary>
    public static Vector3 VectorRotate(Vector3 _vector, float _degrees)
    {
        return _vector.RotateRadians(_degrees * DegToRad);
    }


    /// <summary>
    /// Vector값을 radian 만큼 회전하여 새로운 Vector3 값을 반환한다. 
    /// </summary>
    public static Vector3 RotateRadians(this Vector3 _vector, float _radians) // this 키워드는 확장 메소드 검색 static에서만 사용됨
    {
        var ca = Mathf.Cos(_radians);
        var sa = Mathf.Sin(_radians);
        return new Vector3(ca * _vector.x - sa * _vector.y, sa * _vector.x + ca * _vector.y, 0);
    }

    /// <summary>
    /// vector방향으로 회전하는 Quaternion 계산
    /// </summary>
    public static Quaternion GetRotFromVector(Vector3 _vector)
    {
        Quaternion newRotation = Quaternion.LookRotation(_vector, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.y = 0.0f;
        return newRotation;
    }
    
}
