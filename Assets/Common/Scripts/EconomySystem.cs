using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct GradePrice
{
    [Header("S,A,B,C,D,E")]
    [SerializeField]
    float[] price;

    public float GetPrice(Grade grade)
    {
        switch (grade)
        {
            default:
            case Grade.S:
                return price[0];
            case Grade.A:
                return price[1];
            case Grade.B:
                return price[2];
            case Grade.C:
                return price[3];
            case Grade.D:
                return price[4];
            case Grade.E:
                return price[5];

        }
    }
}

public class EconomySystem : MonoBehaviourSingleton<EconomySystem> {

    [SerializeField]
    GradePrice gradePrices;
    [SerializeField]
    float floorWeight;
    const int avgCoin = 100;

    int currentRemainCoinNum;
    int monsterRoomGage;
    int eachCoinLow, eachCoinHigh;

	public void InitFloorData(List<Map.Rect> roomList)
    {
        monsterRoomGage = 0;
        int roomListLength = roomList.Count;
        for (int i = 0; i < roomListLength; ++i)
        {
            if (roomList[i].eRoomType == RoomType.MONSTER)
            {
                monsterRoomGage += roomList[i].gage;
            }
        }
        currentRemainCoinNum = avgCoin * (UtilityClass.CoinFlip(50) ? 9 : 11);
        currentRemainCoinNum /= 10;

    }

    public int DropCoin(int gage)
    {
        if (monsterRoomGage <= 0 || currentRemainCoinNum <= 0)
            return 0;
        eachCoinLow = currentRemainCoinNum / monsterRoomGage;
        eachCoinHigh = (currentRemainCoinNum + monsterRoomGage - 1) / monsterRoomGage;

        int ret = Random.Range(eachCoinLow,eachCoinHigh) * gage;

        currentRemainCoinNum -= ret;
        monsterRoomGage -= gage;
        return ret;
    }

    public int GetPrice(Grade grade)
    {
        int ret = 0;
        float weight = Mathf.Pow(floorWeight, InGameManager.Instance.GetFloor());
        ret = (int)(gradePrices.GetPrice(grade) * weight);
        ret = ret * (UtilityClass.CoinFlip(50) ? 8 : 12);
        ret /= 10;
        return ret;
    }
}
    