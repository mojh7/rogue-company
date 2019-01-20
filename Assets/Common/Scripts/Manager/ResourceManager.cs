using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviourSingleton<ResourceManager> {

    public GameObject DoorArrow;
    public ObjectPool objectPool;
    public GameObject wallPrefab;
    public ObjectPool itemPool;
    public ObjectPool skillPool;
}
