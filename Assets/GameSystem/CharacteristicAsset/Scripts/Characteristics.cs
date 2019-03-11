using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacteristicsAsset
{
    [CreateAssetMenu(fileName = "Characteristics", menuName = "CharacteristicsAsset/Characteristics", order = 1)]
    public class Characteristics : ScriptableObject
    {
        public bool unlocked;
        public int id;
        public string characteristicsName;
        [TextArea(3, 100)]
        public string description;
        

        public ItemUseEffect effects;

        [Header("Requirements")]
        public float[] requiredGoldByLevel;
        public int requiredCharacteristicsIds;
    }
}
