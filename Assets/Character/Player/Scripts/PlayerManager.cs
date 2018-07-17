using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourSingleton<PlayerManager>
{
    #region variables
    public GameObject playerPrefab;
    public PlayerData[] playerDatas;
    GameObject playerObj;
    Player player;

    #endregion

    #region getter
    // 0513 모장현
    public Player GetPlayer()
    {
        return player;
    }
    public Vector3 GetPlayerPosition()
    {
        if (playerObj == null)
            return Vector3.zero;
        return playerObj.transform.position;
    }
    #endregion

    #region function
    public void DeletePlayer()
    {
        Destroy(playerObj);
    }

    public void SpawnPlayer()
    {
        playerObj = Instantiate(playerPrefab,RoomManager.Instance.RoomStartPoint(), Quaternion.identity);
        player = playerObj.GetComponent<Player>();
        player.Init();
        // 저장된 데이터 없이 새로운 게임을 시작할 때
        if (false == GameStateManager.Instance.GetLoadsGameData())
        {
            player.InitPlayerData(GameDataManager.Instance.GetPlayerData(Player.PlayerType.SOCCER));
        }
        // 저장된 데이터를 로드한 상태일 때
        else
        {
            player.InitPlayerData(GameDataManager.Instance.GetPlayerData());
        }
        GameStateManager.Instance.SetLoadsGameData(false);
        RoomManager.Instance.FindCurrentRoom(); // 플레이어 방찾기.
    }
    #endregion

}
