using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BlackBoard : MonoBehaviourSingleton<BlackBoard>
    {
        public Hashtable data;

        private void Awake()
        {
            data = new Hashtable();
        }
        public void Init()
        {
            if(!data.ContainsKey("Player"))
                data.Add("Player", PlayerManager.Instance.GetPlayer());
            else
                data["Player"] = PlayerManager.Instance.GetPlayer();
            if (!data.ContainsKey("Enemy"))
                data.Add("Enemy",EnemyManager.Instance.GetEnemyList);
            else
                data["Enemy"] = EnemyManager.Instance.GetEnemyList;
        }
    }
}