using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourSingleton<ItemManager> {
    public GameObject customObject;
    public Sprite sprite;

    // 0513 모장현
    [SerializeField]
    private GameObject wepaonPrefab;

    public void CallItemBox(Vector3 _position)
    {
        GameObject obj = Instantiate(customObject, _position, Quaternion.identity, this.transform);
        obj.AddComponent<ItemBox>();
        obj.GetComponent<ItemBox>().sprite = sprite;
        obj.GetComponent<ItemBox>().Init();
    }

    public void DropItem(Vector3 _position)
    {
        GameObject obj = Instantiate(customObject, _position, Quaternion.identity, this.transform);

        // 모장현, id에 따른 무기 생성 초기화 및 parent item container로 지정
        GameObject Item = ObjectPoolManager.Instance.CreateWeapon(Random.Range(0, 10), _position, obj.transform);

        obj.AddComponent<ItemContainer>();
        obj.GetComponent<ItemContainer>().Init(Item.GetComponent<Item>());
        StartCoroutine(CoroutineDropping(obj, new Vector2(Random.Range(-1,2), 5)));
    }
    /// <summary>
    /// 아이템 포물선 방향으로 던짐
    /// </summary>
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
        _object.GetComponent<PolygonCollider2D>().enabled = true;
    }
}
