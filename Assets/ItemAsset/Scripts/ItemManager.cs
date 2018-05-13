using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourSingleton<ItemManager> {
    public GameObject w_obj;
    public GameObject customObject;
    public Sprite sprite;

    public void CallItemBox()
    {
        GameObject obj = Instantiate(customObject, PlayerManager.Instance.GetPlayerPosition(), Quaternion.identity, this.transform);
        obj.AddComponent<ItemBox>();
        obj.GetComponent<ItemBox>().sprite = sprite;
        obj.GetComponent<ItemBox>().Init();
    }

    public void DropItem(Vector3 _position)
    {
        GameObject obj = Instantiate(customObject, _position, Quaternion.identity, this.transform);
        obj.AddComponent<ItemContainer>();
        obj.GetComponent<BoxCollider2D>().enabled = false;
        obj.GetComponent<ItemContainer>().sprite = w_obj.GetComponent<SpriteRenderer>().sprite;
        obj.GetComponent<ItemContainer>().Init();
        StartCoroutine(CoroutineDropping(obj, new Vector2(1, 5)));
    }

    IEnumerator CoroutineDropping(GameObject _object, Vector2 _vector)
    {
        int g = 20;
        float floor = _object.transform.position.y;
        float elapsed_time = 0;
        float sX = _object.transform.position.x;
        float sY = _object.transform.position.y;
        float sZ = _object.transform.position.z;
        float vX = _vector.x;
        float vY = _vector.y;
        while (true)
        {
            elapsed_time += Time.deltaTime;
            float x = sX + vX * elapsed_time;
            float y = sY + vY * elapsed_time - (0.5f * g * elapsed_time * elapsed_time);
            _object.transform.position = new Vector3(x,y,sZ);
            if (_object.transform.position.y <= floor)
                break;
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
        _object.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            CallItemBox();
    }
}
