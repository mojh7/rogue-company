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
    private Image stamina;
    [SerializeField]
    private int maxStamina;        // 스태미너의 Max치 
    private int playerStamina;       // player 의 스태미너

    #region getter setter
    public int getPlayerStamina() { return playerStamina; }
    public int getTotalStamina() { return maxStamina; }
    public void setPlayerStamina(int playerStamina) { this.playerStamina = playerStamina; }
    public void setTotalStamina(int totalStamina) { this.maxStamina = totalStamina; }
    #endregion

    void Awake() {
        stamina = GetComponent<Image>();
        StaminaAmout(stamina.fillAmount, maxStamina);
    }


    #region function
    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void StaminaPlus()
    {
        if (StaminaState())
        {
            // 3초마다 한번씩 1씩 추가
            // 몬스터 죽을때마다 1씩 추가
            stamina.fillAmount += 3 / (float)maxStamina;
            StaminaAmout(stamina.fillAmount, maxStamina);
        }
        else
        {
            return;
        }
    }

    public void StaminaMinus()
    {
        if (stamina.fillAmount > 0 && stamina.fillAmount <= 1)
        {
            stamina.fillAmount -= 5 / (float)maxStamina;
            StaminaAmout(stamina.fillAmount, maxStamina);
        }
        else if (stamina.fillAmount <= 0)
        {
            //playerData.Stamina = 0;
            Debug.Log("스태미너 0이에요 공격 ㄴㄴ");
        }
    }

    public bool StaminaState()
    {
        if (stamina.fillAmount >= 0 && stamina.fillAmount < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isAttack()
    {
        if (stamina.fillAmount >= 5 && stamina.fillAmount <= 1)
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
