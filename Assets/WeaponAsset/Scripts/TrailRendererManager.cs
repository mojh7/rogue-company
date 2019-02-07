using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererManager : MonoBehaviourSingleton<TrailRendererManager>
{

    #region func

    public TrailRenderer ActivateTrailRenderer(TrailRendererInfo info, Transform parent)
    {
        TrailRenderer renderer = ObjectPoolManager.Instance.CreateTrailRenderer();
        if (renderer == null)
            return null;

        renderer.gameObject.transform.SetParent(parent, false);
        renderer.enabled = true;
        renderer.material = info.material;
        renderer.startWidth = info.startWidth;
        renderer.endWidth = info.endWidth;
        renderer.time = info.time;
        renderer.Clear();
        return renderer;
    }

    public void RemoveTrailRenderer(GameObject trailObj, TrailRenderer renderer, float remainTime)
    {
        trailObj.transform.SetParent(ObjectPoolManager.Instance.GetTrailRendererTransform(), true);
        UtilityClass.Invoke(this, () => ObjectPoolManager.Instance.DeleteTrailRenderer(trailObj), remainTime + 0.01f);
    }

    #endregion
}
