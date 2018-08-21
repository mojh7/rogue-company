using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleTest2 : MonoBehaviour {

    #region Var
    [SerializeField] private Image arrow;
    private bool isHide = true;

    [SerializeField] private Image layer;

    [SerializeField] private GameObject[] panel;
    private bool isSelect = false;

    Vector3 titleDes;
    Vector3 selectDes;
    Vector3 mousePos;

    float touchStart = 0f;
    
    private float swipeSpeed = 0.05f;
    #endregion

    #region Function
    void GetPanelPosition()
    {
        float delta = Input.mousePosition.x - touchStart;

        if (delta < -50f && !isSelect)
        {
            titleDes = new Vector3(panel[0].transform.position.x - 2f,
                panel[0].transform.position.y, panel[0].transform.position.z);
            selectDes = new Vector3(panel[1].transform.position.x + 2f,
                panel[1].transform.position.y, panel[1].transform.position.z);
            isSelect = !isSelect;
        }
        else if (delta > 50f && isSelect)
        {
            titleDes = new Vector3(panel[0].transform.position.x + 2f,
                panel[0].transform.position.y, panel[0].transform.position.z);
            selectDes = new Vector3(panel[1].transform.position.x - 2f,
                panel[1].transform.position.y, panel[1].transform.position.z);
            isSelect = !isSelect;
        }
    }

    void Swipe()
    {
        if (panel[0].transform.position.x < titleDes.x && panel[1].transform.position.x > selectDes.x)
        {
            panel[0].transform.position = new Vector3(panel[0].transform.position.x + swipeSpeed,
                panel[0].transform.position.y, panel[0].transform.position.z);

            panel[1].transform.position = new Vector3(panel[1].transform.position.x + swipeSpeed,
                   panel[1].transform.position.y, panel[1].transform.position.z);

            // layer setactive false
            layer.gameObject.SetActive(false);
        }
        else
        {
            panel[0].transform.position = new Vector3(panel[0].transform.position.x - swipeSpeed,
                   panel[0].transform.position.y, panel[0].transform.position.z);

            panel[1].transform.position = new Vector3(panel[1].transform.position.x - swipeSpeed,
                   panel[1].transform.position.y, panel[1].transform.position.z);

            // layer setactive true
            layer.gameObject.SetActive(true);
        }
    }

    public void Test()
    {

    }
    #endregion

    #region UnityEngine
    private void Awake()
    {
        titleDes = panel[0].transform.position;
        selectDes = panel[1].transform.position;
    }

    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition.x;
            mousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!panel[2].activeSelf)
            {
                GetPanelPosition();
            }
        }
        if (Vector3.Distance(panel[0].transform.position, titleDes) > 0.2f && !panel[2].activeSelf)
        {
            Swipe();
        }

        if (isHide)
        {
            Color color = arrow.color;
            color.a = color.a - Time.deltaTime;

            if (color.a < 0)
            {
                color.a = 0.0f;
                isHide = false;
            }

            arrow.color = color;
        }
        else
        {
            Color color = arrow.color;
            color.a = color.a + Time.deltaTime;

            if (color.a > 1)
            {
                color.a = 1.0f;
                isHide = true;
            }
            arrow.color = color;
        }
    }
    #endregion
}
