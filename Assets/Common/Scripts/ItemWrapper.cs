using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWrapper : MonoBehaviour {
    Object m_object;/* 임시 */
    public void SetObject(Object _obj)
    {
        GetComponent<SpriteRenderer>().sprite = null;
    }
    public Object GetObject()
    {
        return m_object;
    }
}
