using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererManager : MonoBehaviourSingleton<TrailRendererManager>
{

	// Use this for initialization
	void Start () {
		
	}
	

    #region func

    public TrailRenderer ActivateTrailRenderer(TrailRendererInfo info, Transform parent)
    {
        TrailRenderer renderer = ObjectPoolManager.Instance.CreateTrailRenderer();
        if (renderer == null)
            return null;

        renderer.enabled = true;
        //renderer.gameObject.transform.position = Vector3.zero;
        //renderer.gameObject.transform.localScale = Vector3.one;
        renderer.gameObject.transform.SetParent(parent, false);
        renderer.material = info.material;
        renderer.time = info.time;
        renderer.startWidth = info.startWidth;
        renderer.endWidth = info.endWidth;
        return renderer;
    }

    public void RemoveTrailRenderer(GameObject trailObj, TrailRenderer renderer, float remainTime)
    {
        trailObj.transform.SetParent(ObjectPoolManager.Instance.GetTrailRendererTransform(), true);
        UtilityClass.Invoke(this, () => renderer.time = 0, remainTime);
        UtilityClass.Invoke(this, () => ObjectPoolManager.Instance.DeleteTrailRenderer(trailObj), remainTime + 0.1f);
    }

    #endregion
}
