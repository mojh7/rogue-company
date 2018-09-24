using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleSwipe : MonoBehaviour
{
    // 윤아는 세젤귀
    #region Var
    [SerializeField] private Image arrow;
    private bool isHide = true;

    [SerializeField] private GameObject[] panel;
    private bool isSelect = false;
    private bool isSwipe = false;

    private Vector2 pos;

    Vector3 desSelect;
    Vector3 desTitle;
    Vector3 mousePos;

    float touchStart = 0f;
    
    private float swipeSpeed = 0f;
    float width = Screen.width;
    #endregion

    #region Function

    void GetPanelPosition()
    {
        float delta = Input.mousePosition.x - touchStart;

        if (delta == 0)
        {
            return;
        }
        if (delta > -100f && !isSelect)
        {
            desSelect = new Vector3(panel[0].transform.position.x + width,
                panel[0].transform.position.y, panel[0].transform.position.z);
            desTitle = new Vector3(panel[1].transform.position.x - width,
                panel[1].transform.position.y, panel[1].transform.position.z);
            isSelect = !isSelect;
        }
        else if (delta < 100f && isSelect)
        {
            desSelect = new Vector3(panel[0].transform.position.x - width,
                panel[0].transform.position.y, panel[0].transform.position.z);
            desTitle = new Vector3(panel[1].transform.position.x + width,
                panel[1].transform.position.y, panel[1].transform.position.z);
            isSelect = !isSelect;
        }
    }

    void Swipe()
    {
        if (panel[0].transform.position.x < desSelect.x && panel[1].transform.position.x > desTitle.x)
        {
            panel[0].transform.position = new Vector3(panel[0].transform.position.x + swipeSpeed,
                panel[0].transform.position.y, panel[0].transform.position.z);

            panel[1].transform.position = new Vector3(panel[1].transform.position.x + swipeSpeed,
                   panel[1].transform.position.y, panel[1].transform.position.z);

            isSwipe = true;
        }
        else
        {
            panel[0].transform.position = new Vector3(panel[0].transform.position.x - swipeSpeed,
                   panel[0].transform.position.y, panel[0].transform.position.z);

            panel[1].transform.position = new Vector3(panel[1].transform.position.x - swipeSpeed,
                   panel[1].transform.position.y, panel[1].transform.position.z);
            isSwipe = true;
        }
    }
    #endregion

    #region UnityEngine
    private void Awake()
    {
        swipeSpeed = width / 30;
        desSelect = panel[0].transform.position;
        desTitle = panel[1].transform.position;
    }
    
    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition.x;
            mousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!panel[2].activeSelf && !isSwipe)
            {
                GetPanelPosition();
            }
        }

        if (Vector3.Distance(panel[0].transform.position, desSelect) > 0.2f)
        {
            panel[2].SetActive(false);
            Swipe();
        }
        else
        {
            isSwipe = false;
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
