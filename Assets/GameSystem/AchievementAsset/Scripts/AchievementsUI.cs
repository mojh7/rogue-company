using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject achievementsUI;

    // Use this for initialization
    void Awake ()
    {
        achievementsUI.SetActive(false);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
