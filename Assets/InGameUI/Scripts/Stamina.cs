using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스테미나가 줄여들고 불어나는 걸 보여주는 UI
/// </summary>
public class Stamina : MonoBehaviourSingleton<Stamina>
{
    [SerializeField]
    private Text steminaText;
    private PlayerData playerData;
    private Player player;
    private Image staminaImage;
    private int maxStamina;        // 스태미너의 Max치 
    private int playerStamina;       // player 의 스태미너
    private int stamina;
    private float currentTime = 0;
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
        currentTime = 0;
        staminaImage = GetComponent<Image>();
        if (playerData == null)
            playerData = new PlayerData();
    }
    void Update()
    {
        if (Stamina.Instance.IsFullStamina())
        {
            currentTime += Time.deltaTime;
            if (currentTime > 3)
            {
                Stamina.Instance.RecoverStamina();
                currentTime = 0;
            }
        }
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
        steminaText.text = stamina + "/" + maxStamina;
    }

    /// <summary>
    /// 스테미너 회복 : 자연 회복, 몬스터 처치시 회복
    /// </summary>
    public void RecoverStamina(int recoveryAmount = 3)
    {
        ParticleManager.Instance.PlayParticle("Stamina", player.GetPosition());
        recoveryAmount += PlayerBuffManager.Instance.BuffManager.CharacterTargetEffectTotal.gettingStaminaIncrement;
        stamina += (int)(recoveryAmount * PlayerBuffManager.Instance.BuffManager.CharacterTargetEffectTotal.steminaGage);
        staminaImage.fillAmount += recoveryAmount / (float)maxStamina;

        if (stamina >= maxStamina)
        {
            stamina = maxStamina;
            staminaImage.fillAmount = 1;
        }
        playerData.Stamina = stamina;
        steminaText.text = stamina + "/" + maxStamina;
    }

    public void RecoverFullStamina()
    {
        int recoveryAmount = maxStamina;
        stamina += recoveryAmount;
        staminaImage.fillAmount += recoveryAmount / (float)maxStamina;

        if (staminaImage.fillAmount >= maxStamina)
        {
            stamina = maxStamina;
            staminaImage.fillAmount = maxStamina;
        }
        playerData.Stamina = stamina;
        steminaText.text = stamina + "/" + maxStamina;
    }
    public void ConsumeStamina(int staminaConsumption)
    {
        stamina -= staminaConsumption;
        staminaImage.fillAmount -= staminaConsumption / (float)maxStamina;

        if (staminaImage.fillAmount < 0)
        {
            stamina = 0;
            staminaImage.fillAmount = 0;
        }
        playerData.Stamina = stamina;
        steminaText.text = stamina + "/" + maxStamina;
    }

    public bool IsFullStamina()
    {
        if (staminaImage.fillAmount >= 1f)
            return true;
        else
            return false;
    }

    public bool IsConsumableStamina()
    {
        if (0 < staminaImage.fillAmount && staminaImage.fillAmount <= 1)
            return true;
        else
            return false;
    }

    //// 리턴 시키는 함수
    //public int StaminaAmout(float amount, int totalStamina)
    //{
    //    float temp = amount * totalStamina;
    //    playerStamina = (int)temp;
    //    if(player != null)
    //        player.SetStamina(playerStamina);
    //    return playerStamina;
    //}
    #endregion
}
