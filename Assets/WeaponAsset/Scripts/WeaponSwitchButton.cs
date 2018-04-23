using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class WeaponSwitchButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Vector2 pos;
    // Use this for initialization
    void Start()
    {

    }
    // 터치 했을 때
    public void OnPointerDown(PointerEventData ped)
    {
        pos.x = ped.position.x;
        //Debug.Log("WeaponSwapBtn touch down x : " + ped.position.x);
    }

    // 터치 후 땠을 때
    public void OnPointerUp(PointerEventData ped)
    {
        // 다음 무기로 교체 방향 ->
        if (ped.position.x > pos.x)
        {
            Debug.Log("다음 무기로 교체");
            WeaponManager.Instance.ChangeWeapon(true);
        }
        // 이전 무기로 교체 방향 <-
        else if (ped.position.x < pos.x)
        {
            Debug.Log("이전 무기로 교체");
            WeaponManager.Instance.ChangeWeapon(false);
        }
        // Debug.Log("WeaponSwapBtn touch up x : " + ped.position.x);
    }
}
