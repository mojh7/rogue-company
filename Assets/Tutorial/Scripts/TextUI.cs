using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviourSingleton<TextUI>
{

    [HideInInspector] public int count = 0;
    [SerializeField] private Text touch;
    private bool isHide = true;

    void Update()
    {
        if (isHide)
        {
            Color color = touch.color;
            color.a = color.a - Time.deltaTime;

            if (color.a < 0)
            {
                color.a = 0.0f;
                isHide = false;
            }

            touch.color = color;
        }
        else
        {
            Color color = touch.color;
            color.a = color.a + Time.deltaTime;

            if (color.a > 1)
            {
                color.a = 1.0f;
                isHide = true;
            }
            touch.color = color;
        }
    }
}
