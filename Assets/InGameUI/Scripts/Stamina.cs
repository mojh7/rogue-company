using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스테미나가 줄여들고 불어나는 걸 보여주는 UI
/// </summary>
public class Stamina : MonoBehaviourSingleton<Stamina>
{
    private PlayerData playerData;
    private Player player;
    private Image staminaImage;
    private int maxStamina;        // 스태미너의 Max치 
    private int playerStamina;       // player 의 스태미너
    private int stamina;

    #region pro
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
        set
        {
            playerData = value;
        }
    }
    #endregion

    void Awake() {
        staminaImage = GetComponent<Image>();
        if (playerData == null)
            playerData = new PlayerData();
    }


    #region function
    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void SetStaminaBar(int _stamina)
    {
        stamina = _stamina;
        maxStamina = _stamina;
        staminaImage.fillAmount = stamina / (float)maxStamina;
    }

    public void StaminaPlus()
    {
        if (StaminaState())
        {
            // 3초마다 채운다
            // 한 번 마다 3씩 충전
            // 몬스터 죽일 때마다도 3씩
            stamina += 3;
            playerData.Stamina = stamina;
            staminaImage.fillAmount += 3 / (float)maxStamina;
        }
        else
        {
            return;
        }
    }

    public void StaminaMinus()
    {
        if (staminaImage.fillAmount > 0 && staminaImage.fillAmount <= 1)
        {
            // 무기 공격시 5 깎인
            stamina -= 5;
            playerData.Stamina = stamina;
            staminaImage.fillAmount -= 5 / (float)maxStamina;
            Debug.Log(playerData.Stamina);
        }
        else if (staminaImage.fillAmount <= 0)
        {
            return;
        }
    }

    public bool StaminaState()
    {
        if (staminaImage.fillAmount >= 0 && staminaImage.fillAmount < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 리턴 시키는 함수
    public int StaminaAmout(float amount, int totalStamina)
    {
        float temp = amount * totalStamina;
        playerStamina = (int)temp;
        if(player != null)
            player.SetStamina(playerStamina);
        return playerStamina;
    }
    #endregion
}
