using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviourSingleton<SpawnManager> {

    [SerializeField]
    GameObject prefabs;

    Spawner spawner;

    private void Awake()
    {
        GameObject gameObject = GameObject.Instantiate(prefabs);
        gameObject.SetActive(true);
        spawner = gameObject.AddComponent<Spawner>();
        spawner.LoadAwake();
        spawner.sprites = null;
        spawner.Init();
    }

    public void Spawn()
    {
        spawner.Active();
    }
    public void ResetProcess()
    {
        spawner.Init();        
    }
}
