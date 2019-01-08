using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum IllustratedBookType { WEAPON, ITEM }

public class IllustratedBookUI : MonoBehaviour
{
    #region variables
    [SerializeField]
    private GameObject bookUI;

    #endregion

    #region unityfunc
    void Awake()
    {
        bookUI.SetActive(false);
    }
    #endregion

    #region func
    public void OpenBook()
    {
        bookUI.SetActive(true);
    }
    public void CloseBook()
    {
        bookUI.SetActive(false);
    }
    #endregion
}
