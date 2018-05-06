using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

//
// http://tenlie10.tistory.com/115
//

// 조이스틱 입력에 따른 ui조절, 벡터 값 생성 class
public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    #region variables
    public Image backgroundImage;           // 배경 image, 큰 원
    public Image joystickImage;             // 움직일 image, 작은 원
    private Vector3 inputVector;            // 입력 중일 때의 vector 값
    private Vector3 recenteInputVector;     // 터치 다운, 업에 상관없이 가장 최근 입력 벡터 
    #endregion

    #region getter
    public float GetHorizontalValue()
    {
        return inputVector.x;
    }
    public float GetVerticalValue()
    {
        return inputVector.y;
    }
    public Vector3 GetRecenteInputVector()
    {
        return recenteInputVector;
    }
    #endregion

    void Awake()
    {
        recenteInputVector = Vector3.right;
    }

    #region interface implement
    /*
     * 터치된 로컬 좌표값을 pos에 할당하고 bgImg 직사각형의 sizeDelta값으로 나누어
     * pos.x는 0~-1, pos.y는 0~1사이의 값으로 만듭니다. joystickImg를 기준으로
     * 좌우로 움직였을 때 pos.x는 -1~1 사이의 값으로, 상하로 움직였을 때 pos.y는 -1~1의 값으로 변환하기 위해 
     * pos.x*2 +1, pos.y*2-1 처리를 합니다. 이 값을 inputVector에 대입하고 단위벡터로 만듭니다.
     * 마지막으로 joystickImg를 터치한 좌표값으로 이동시킵니다.
     * 내껀 중심을 중앙으로 잡아서 아마도 -0.5~0.5 나올 것 같음
     * 그래서 *2 만 함. -1 ~ +1 나오게 <= 틀림 잘 안됨
     * 위에 방법이 안되는 줄 알았지만 중앙으로 중심 잡고 pos.x * 2 + 1, pos.y * 2 - 1를 아래처럼 바꾸니 또 잘됨
     */
    // 드래그 중
    public void OnDrag(PointerEventData ped)
    {

        Vector2 pos;
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundImage.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / backgroundImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / backgroundImage.rectTransform.sizeDelta.y);
            //Debug.Log("2 : " + pos.x + ", " + pos.y);

            inputVector = new Vector3(pos.x * 2, pos.y * 2, 0);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            // 가장 마지막 입력 vector 값
            recenteInputVector = inputVector;

            // Move Joystick IMG
            joystickImage.rectTransform.anchoredPosition = new Vector3(inputVector.x * (backgroundImage.rectTransform.sizeDelta.x / 3)
                , inputVector.y * (backgroundImage.rectTransform.sizeDelta.y / 3));
        }
    }

    // 터치 했을 때
    public void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    // 땠을 때
    public void OnPointerUp(PointerEventData ped)
    {
        
        inputVector = Vector3.zero;
        joystickImage.rectTransform.anchoredPosition = Vector3.zero;
    }
    #endregion

}
