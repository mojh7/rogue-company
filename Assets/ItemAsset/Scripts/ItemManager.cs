using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourSingleton<ItemManager> {
    public GameObject customObject;
    public Sprite coinSprite;
    public Sprite sprite;
    Queue<GameObject> objs;
    private void Start()
    {
        objs = new Queue<GameObject>();
    }
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
        GameObject obj = Instantiate(customObject, _position, Quaternion.identity);

        objs.Enqueue(obj);
        obj.AddComponent<ItemBox>();
        obj.GetComponent<ItemBox>().sprite = sprite;
        obj.GetComponent<ItemBox>().Init(_item);
        obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y);
    }

    public GameObject CreateItem(Item _item,Vector3 _position)
    {
        GameObject obj = Instantiate(customObject, _position, Quaternion.identity,transform);
        obj.AddComponent<ItemContainer>().Init(_item);
        objs.Enqueue(obj);
        StartCoroutine(CoroutineDropping(obj, new Vector2(Random.Range(-1, 2), 5)));

        return obj;
    }

    public Item CreateVendingItem()
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<Coin>();

        return gameObject.GetComponent<Coin>();
    }
    #endregion
  

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
}
