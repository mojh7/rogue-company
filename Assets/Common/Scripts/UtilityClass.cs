using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class UtilityClass
{
    public static void ColorLerp(this MonoBehaviour me, UnityEngine.UI.Image spriteRenderer, Color src, Color dest, float time)
    {
        me.StartCoroutine(CoroutineLerp(spriteRenderer, src, dest, time));
    }
    private static IEnumerator CoroutineLerp(UnityEngine.UI.Image spriteRenderer, Color src, Color dest, float time)
    {
        float startTime = Time.time;
        float tempElapsedTime = 0;
        while (time >= tempElapsedTime)
        {
            tempElapsedTime = Time.time - startTime;

            spriteRenderer.color = Color.Lerp(src, dest, tempElapsedTime / time);

            yield return YieldInstructionCache.WaitForEndOfFrame;
        }

    }
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
    public static bool CheckLayer(int layer, LayerMask comparedLayer)
    {
        LayerMask mainLayer = 1 << layer;

        if ((mainLayer & comparedLayer) == mainLayer)
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
        return UnityEngine.Random.Range(0, 100) < percent;
    }
    /// <summary>
    /// 매개변수의 적 레이어 반환
    /// </summary>
    public static LayerMask GetEnemyLayer(Character me)
    {
        LayerMask enemyayer = 1;

        switch (me.GetOwnerType())
        {
            case CharacterInfo.OwnerType.Player:
                enemyayer = enemyayer << 13;
                break;
            case CharacterInfo.OwnerType.Enemy:
                enemyayer = enemyayer << 16;
                break;
            case CharacterInfo.OwnerType.Pet:
                enemyayer = enemyayer << 13;
                break;
            case CharacterInfo.OwnerType.Object:
                enemyayer = enemyayer << 13;
                break;
            default:
                break;
        }
        return enemyayer;
    }

    public static void Invoke(this MonoBehaviour me, Action theDelegate, float time)
    {
        me.StartCoroutine(ExecuteAfterTime(theDelegate, time));
    }

    private static IEnumerator ExecuteAfterTime(Action theDelegate, float delay)
    {
        yield return new WaitForSeconds(delay);
        theDelegate();
    }
}
