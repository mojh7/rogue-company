using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearRoomUI : MonoBehaviour {

    [SerializeField] Image[] image;
    bool isHide = true;
    float currentTime = 0;

    /// <summary>
    /// Alpha값 변경
    /// </summary>
    void ChangeAlpha(Image image)
    {
        if (isHide)
        {
            Color iColor = image.color;
            iColor.a = iColor.a - Time.deltaTime * 1.5f;

            if (iColor.a < 0)
            {
                iColor.a = 0.0f;
                isHide = false;
            }

            image.color = iColor;
        }
        else
        {
            Color iColor = image.color;
            iColor.a = iColor.a + Time.deltaTime * 1.5f;

            if (iColor.a > 1)
            {
                iColor.a = 1.0f;
                isHide = true;
            }
            image.color = iColor;
        }
    }

    private void Awake()
    {
        currentTime = 0;
    }
    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime < 3.5f)
        {
            ChangeAlpha(image[0]);
        }
        else
        {
            currentTime = 0;
            UIManager.Instance.ClearRoomUI(false);
        }
    }
}
