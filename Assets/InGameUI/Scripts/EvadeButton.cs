using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;    //UI 클릭시 터치 이벤트 발생 방지.

public class EvadeButton : MonoBehaviour, IPointerDownHandler
{

    private Character character;

    public void SetPlayer(Character character)
    {
        this.character = character;
    }
    // 터치 했을 때
    public void OnPointerDown(PointerEventData ped)
    {
        if (UIManager.Instance.GetActived())
            return;
        character.Evade();
    }
}
