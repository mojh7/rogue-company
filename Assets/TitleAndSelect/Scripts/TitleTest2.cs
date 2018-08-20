using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleTest2 : MonoBehaviour {

    [SerializeField] private Image arrow;
    private bool isHide = true;

    [SerializeField] private Image layer;

    [SerializeField] private GameObject[] panel;
    private bool isSelect = false;

    Vector3 titleDes;
    Vector3 selectDes;

    float touchStart = 0f;
    
    private float swipeSpeed = 0.05f;

    private void Awake()
    {
        titleDes = panel[0].transform.position;
        selectDes = panel[1].transform.position;
    }

    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition.x;
        }
        if (Input.GetMouseButtonUp(0))
        {
            float delta = Input.mousePosition.x - touchStart;
            if (delta < -50f)
            {
                titleDes = new Vector3(panel[0].transform.position.x - 2f,
                    panel[0].transform.position.y, panel[0].transform.position.z);
                selectDes = new Vector3(panel[1].transform.position.x + 2f,
                    panel[1].transform.position.y, panel[1].transform.position.z);
            }
            else if (delta > 50f)
            {
                titleDes = new Vector3(panel[0].transform.position.x + 2f,
                    panel[0].transform.position.y, panel[0].transform.position.z);
                selectDes = new Vector3(panel[1].transform.position.x - 2f,
                    panel[1].transform.position.y, panel[1].transform.position.z);
            }
        }
        if (Vector3.Distance(panel[0].transform.position, titleDes) > 0.2f)
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
}
