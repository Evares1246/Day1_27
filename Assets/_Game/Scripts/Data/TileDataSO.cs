using UnityEngine;

namespace GameDesign.Data
{
    public enum TileType { Start, Empty, Property, Event, Shop }

    [CreateAssetMenu(fileName = "NewTile", menuName = "SoulGambler/Map/TileData")]
    public class TileDataSO : ScriptableObject
    {
        public TileType type;
        public string tileName;
        [TextArea] public string description;
        public GameObject visualPrefab; // 地块的 3D 模型预制体

        [Header("Property Settings")]
        public int baseCost = 100;      // 购买价格
        public int baseTax = 20;        // 基础过路费
    }
}