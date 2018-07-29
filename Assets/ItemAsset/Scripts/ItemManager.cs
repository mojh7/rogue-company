using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourSingleton<ItemManager> {
    public Sprite coinSprite;
    public Sprite sprite;
    Queue<GameObject> objs;
    Queue<ItemContainer> withdraws;
    #region UnityFunc
    private void Start()
    {
        objs = new Queue<GameObject>();
        withdraws = new Queue<ItemContainer>();
    }
    #endregion
    #region Func
    public void DeleteObjs()
    {
        if (objs == null)
            return;
        while (objs.Count > 0)
        {
            GameObject gameObject = objs.Dequeue();
            if (gameObject != null)
            {
                if(gameObject.GetComponent<ItemContainer>() != null)
                    gameObject.GetComponent<ItemContainer>().DestroySelf();
                if (gameObject.GetComponent<ItemBox>() != null)
                    gameObject.GetComponent<ItemBox>().DestroySelf();
            }
        }
    }

    public void CallItemBox(Vector2 _position,Item _item)
    {
        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();

        objs.Enqueue(obj);
        obj.transform.position = _position;
        obj.AddComponent<ItemBox>();
        obj.GetComponent<ItemBox>().sprites = new Sprite[1] { sprite };
        obj.GetComponent<ItemBox>().Init(_item);
        obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y);
    }

    public GameObject CreateItem(Item _item,Vector3 _position)
    {
        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();
        obj.transform.position = _position;
        obj.AddComponent<ItemContainer>().Init(_item);
        objs.Enqueue(obj);
        if (_item.GetType() == typeof(Coin))
        {
            withdraws.Enqueue(obj.GetComponent<ItemContainer>());
        }
        StartCoroutine(CoroutineDropping(obj, new Vector2(Random.Range(-1, 2), 5)));

        return obj;
    }

    public void CollectItem()
    {
        if (withdraws == null)
            return;
        while (withdraws.Count > 0)
        {
            ItemContainer itemContainer = withdraws.Dequeue();
            if(itemContainer != null)
            {
                itemContainer.DettachDestroy();
                itemContainer.SubAcitve();
            }
        }
    }
    #endregion
    #region Coroutine
    IEnumerator CoroutineDropping(GameObject _object, Vector2 _vector)
    {
        int g = 20;
        float lowerLimit = _object.transform.position.y;
        float elapsed_time = 0;
        float sX = _object.transform.position.x;
        float sY = _object.transform.position.y;
        float vX = _vector.x;
        float vY = _vector.y;
        while (true)
        {
            if (_object == null)
                break;
            elapsed_time += Time.deltaTime;
            float x = sX + vX * elapsed_time;
            float y = sY + vY * elapsed_time - (0.5f * g * elapsed_time * elapsed_time);
            _object.transform.position = new Vector2(x, y);
            if (_object.transform.position.y <= lowerLimit)
                break;
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
        if (_object != null)
        {
            _object.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }
    #endregion
}
