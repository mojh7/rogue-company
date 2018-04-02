using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://ejonghyuck.github.io/blog/2016-12-12/unity-coroutine-optimization/

internal static class YieldInstructionCache
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;
        if (!_timeInterval.TryGetValue(seconds, out wfs))
            _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
        return wfs;
    }
}

/* Usage.
 * yield return YieldInstructionCache.WaitForEndOfFrame;
 * yield return YieldInstructionCache.WaitForFixedUpdate;
 * 
 * yield return YieldInstructionCache.WaitForSeconds(0.1f);
 * yield return YieldInstructionCache.WaitForSeconds(seconds);
 */
