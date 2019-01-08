using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsData : MonoBehaviourSingleton<ItemsData>
{

    #region variables
    [SerializeField]
    private UsableItemInfo[] clothingItemInfos;
    [SerializeField]
    private UsableItemInfo[] etcItemInfos;
    [SerializeField]
    private UsableItemInfo[] foodItemInfos;
    [SerializeField]
    private UsableItemInfo[] medicalItemInfos;
    [SerializeField]
    private UsableItemInfo[] miscItemInfos;
    [SerializeField]
    private UsableItemInfo[] petItemInfos;
    #endregion

    #region getter
    public int GetClothingItemInfosLength()
    {
        return clothingItemInfos.Length;
    }

    public int GetEtcItemInfosLength()
    {
        return etcItemInfos.Length;
    }

    public int GetFoodItemInfosLength()
    {
        return foodItemInfos.Length;
    }

    public int GetMedicalItemInfosLength()
    {
        return medicalItemInfos.Length;
    }

    public int GetMiscItemInfosLength()
    {
        return miscItemInfos.Length;
    }

    public int GetPetItemInfosLength()
    {
        return petItemInfos.Length;
    }

    public UsableItemInfo GetClothingItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, clothingItemInfos.Length);
        return clothingItemInfos[id];
    }
    public UsableItemInfo GetEtcItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, etcItemInfos.Length);
        return etcItemInfos[id];
    }
    public UsableItemInfo GetFoodItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, foodItemInfos.Length);
        return foodItemInfos[id];
    }
    public UsableItemInfo GetMedicalItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, medicalItemInfos.Length);
        return medicalItemInfos[id];
    }
    public UsableItemInfo GetMiscItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, miscItemInfos.Length);
        return miscItemInfos[id];
    }

    public UsableItemInfo GetPetItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, petItemInfos.Length);
        return petItemInfos[id];
    }
    #endregion

    #region unityFunc
    void Awake()
    {
        InitMiscItems();
    }

    #endregion
    #region func
    private void InitMiscItems()
    {
        for (int i = 0; i < miscItemInfos.Length; i++)
            miscItemInfos[i].SetId(i);
    }
    #endregion
}
