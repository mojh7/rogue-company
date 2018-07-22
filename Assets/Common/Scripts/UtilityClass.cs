using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityClass
{
    /// <summary>
    /// 레이어 체크
    /// </summary>
    /// <param name="layer">체크를 위한 레이어 Integer</param>
    /// <param name="layers">체크를 위한 레이어 그룹 Integers</param>
    /// <returns>체크 여부</returns>
    public static bool CheckLayer(int layer, params int[] layers)
    {
        LayerMask mainLayer = 1 << layer;
        LayerMask comparedLayer = 0;

        for(int i=0;i<layers.Length;i++)
        {
            comparedLayer |= (1 << layers[i]);
        }

        if((mainLayer & comparedLayer) == mainLayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 확률 반환
    /// </summary>
    /// <param name="percent">퍼센트 1~100</param>
    /// <returns>참과 거짓</returns>
    public static bool CoinFlip(int percent)
    {
        return Random.Range(0, 100) < percent;
    } 

}
