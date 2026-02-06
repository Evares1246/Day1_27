using UnityEngine;

namespace GameDesign.Data
{
    public enum ItemType { Weapon, Shield, Consumable, Relic }

    [CreateAssetMenu(fileName = "NewItem", menuName = "SoulGambler/Inventory/ItemData")]
    public class ItemDataSO : ScriptableObject
    {
        public string itemName;
        public ItemType type;
        public Sprite icon;

        [Header("Inventory Shape")]
        // 定义物品占用的相对格子坐标，例如 (0,0) 和 (1,0) 表示 1x2
        public Vector2Int[] shape = new Vector2Int[] { new Vector2Int(0, 0) };

        [Header("Combat Stats")]
        public int attackDamage;
        public int armorValue;
        [TextArea] public string effectDescription;

        [Header("Visuals")]
        public GameObject itemModelPrefab; // 放在棋子身上的 3D 模型
    }
}