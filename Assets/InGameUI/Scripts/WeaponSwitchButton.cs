using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class WeaponSwitchButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    private Vector2 pos;
    // Use this for initialization
    private Character character;
    [SerializeField] private Text ammoViewText;
    [SerializeField] private Image weaponImage;

    public void SetPlayer(Character character)
    {
        this.character = character;
    }

    // 터치 했을 때
    public void OnPointerDown(PointerEventData ped)
    {
        pos.x = ped.position.x;
    }

    // 터치 후 땠을 때
    public void OnPointerUp(PointerEventData ped)
    {
        // 다음 무기로 교체 방향 ->
        if (ped.position.x > pos.x)
        {
            character.GetWeaponManager().ChangeWeapon(true);
        }
        // 이전 무기로 교체 방향 <-
        else if (ped.position.x < pos.x)
        {
            character.GetWeaponManager().ChangeWeapon(false);
        }
    }


    /// <summary> WeaponSwitchButton UI Weapon Sprite View Update </summary>
    public void UpdateWeaponSprite(Sprite sprite)
    {
        weaponImage.sprite = sprite;
    }

    /// <summary> WeaponSwitchButton UI Weapon Sprite View Update </summary>
    public void UpdateAmmoView(WeaponInfo info)
    {
        if(info.ammoCapacity < 0)
        {
            //∞
            ammoViewText.text = "oo / oo";
        }
        // string bulider??? 그걸로 합쳐야 빠를라나
        else
        {
            ammoViewText.text = info.ammo + " / " + info.ammoCapacity;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        character.GetWeaponManager().ChangeWeapon(true);
    }
}
