using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour {
    public abstract void Active();
}

public class Coin : Item
{
    bool isActive = false;

    public override void Active()
    {
        if (!isActive)
        {
            Debug.Log("Coin");
            GameDataManager.Instance.SetCoin();
            isActive = !isActive;
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        float distance = Vector2.Distance(transform.position, PlayerManager.Instance.GetPlayerPosition());

        StartCoroutine(CoroutineMoveToTarget(transform, distance / 2));
    }
   
    IEnumerator CoroutineMoveToTarget(Transform _transform, float _duration)
    {
        float elapsed = 0.0f;
        Vector2 start = _transform.localPosition;
        Vector2 target;
        while (elapsed < _duration)
        {
            target = PlayerManager.Instance.GetPlayerPosition();
            elapsed += Time.deltaTime;
            _transform.localPosition = Vector2.Lerp(start, target, elapsed / _duration);

            yield return YieldInstructionCache.WaitForEndOfFrame;
        }

        Destroy(gameObject);
    }
}