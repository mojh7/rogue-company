using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;

public class WeaponSwitchButton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private Vector2 pos;
    private Character character;
    [SerializeField] private Text ammoViewText;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image PrevImage;
    [SerializeField] private Image NextImage;
    [SerializeField] float shakeAmount;
    [SerializeField] float shakeDuration;

    public void StartShake(float maxX, float maxY, float ShakeTime)
    {
        StartCoroutine(Shake(maxX, maxY, ShakeTime));
    }

    IEnumerator Shake(float maxX, float maxY, float ShakeTime)
    {
        float counter = 0f;
        Vector3 uiPosition = transform.position;
        while (true)
        {
            counter += Time.deltaTime;
            if (counter >= ShakeTime)
            {
                yield break;
            }
            else
            {
                transform.position = uiPosition + new Vector3((ShakeTime - counter) * Random.Range(-maxX, maxX), (ShakeTime - counter) * Random.Range(-maxY, maxY));
            }
            yield return null;
        }
    }

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

    /// <summary> OwnWeaponView UI Right(Next) Own Weapon Sprite View Update </summary>
    public void UpdateNextWeaponSprite(Sprite sprite)
    {
        NextImage.sprite = sprite;
    }

    /// <summary> OwnWeaponView UI Left(Prev) Own Weapon Sprite View Update </summary>
    public void UpdatePrevWeaponSprite(Sprite sprite)
    {
        PrevImage.sprite = sprite;
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

}
