using UnityEngine;

namespace GameDesign.Data
{
    public enum TileType { Start, Empty, Property, Event, Shop }

    [CreateAssetMenu(fileName = "NewTile", menuName = "SoulGambler/TileData")]
    public class TileDataSO : ScriptableObject
    {
        public TileType type;
        public string tileName;
        public GameObject visualPrefab; // 地块模型
        public int baseCost = 100;      // 基础购地/升级费
    }
}