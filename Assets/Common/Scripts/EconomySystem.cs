using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomySystem : MonoBehaviourSingleton<EconomySystem> {

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
}
