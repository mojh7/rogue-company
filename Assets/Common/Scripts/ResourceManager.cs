using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviourSingleton<ResourceManager> {

    public Sprite Rock;
    public GameObject ObjectPrefabs;
    public BT.Task task;

    private void Start()
    {
        Debug.Log(task);
    }
}
