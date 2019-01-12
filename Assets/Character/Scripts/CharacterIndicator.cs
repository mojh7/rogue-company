using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIndicator : MonoBehaviour
{

    Image image;
    int width, height;
    Character target;
    Coroutine routine;
    Vector3 midPos;
    Vector3 direction;
    Color clear, white;
    float x, y;

    private void Awake()
    {
        image = this.GetComponent<Image>();
        clear = Color.clear;
        white = Color.white;

        width = Screen.width;
        height = Screen.height;

        midPos = new Vector3(width * .5f, height * .5f);
    }

    public void SetTarget(Character target)
    {
        this.target = target;
        routine = StartCoroutine(Routine());
    }
    IEnumerator Routine()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(.025f);

            if (null == target)
            {
                break;
            }
            transform.position = CameraController.Instance.worldToScreen(target.GetPosition());
            if (transform.position.x > 0 && transform.position.x < width &&
                transform.position.y > 0 && transform.position.y < height)
            {
                image.color = clear;
                continue;
            }
            image.color = white;

            x = Mathf.Clamp(transform.position.x, 0, width);
            y = Mathf.Clamp(transform.position.y, 0, height);
         
            transform.position = new Vector3(x, y, 0);
            direction = midPos - transform.position;
            transform.rotation = MathCalculator.GetRotFromVector(direction);

        }
        this.gameObject.SetActive(false);
    }
}
