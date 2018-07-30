using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class WeaponDataAutoInput : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}


/*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class Test : MonoBehaviour
{
    private void Awake()
    {
        List<Dictionary<string, object>> data = CSVParser.Read("456");

        Debug.Log(data.Count);
        for(var i = 0; i < data.Count; i++)
        {
            //name,weaponType,attackAniType,rate
            Debug.Log(i + " = " + data[i]["name"] + " " + data[i]["weaponType"] + " " + data[i]["attackAniType"] + " " + data[i]["rate"]);
        }
    }
}
 */



