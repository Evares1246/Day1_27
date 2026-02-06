using UnityEngine;
using GameDesign.Utils;

namespace Game.Gameplay
{
    /// <summary>
    /// 负责管理游戏场景中的静态/动态物体，如桌子、中心升降台、装饰物等。
    /// 不参与核心玩法逻辑，仅负责视觉和对象生命周期。
    /// </summary>
    public class TableManager : SceneSingleton<TableManager>
    {
        [Header("References")]
        [SerializeField] private Transform _centerElevator;
        [SerializeField] private Transform _tableSurface;
        
        [Header("Seats")]
        [Tooltip("Assign 4 seat transforms here, corresponding to player indices 0-3")]
        [SerializeField] private Transform[] _seats;

        /// <summary>
        /// 获取指定玩家索引的座位位置
        /// </summary>
        public Transform GetSeat(int playerIndex)
        {
            if (_seats == null || _seats.Length == 0)
            {
                Debug.LogError("[TableManager] No seats assigned!");
                return null;
            }

            // 使用取模防止越界，确保总是能返回一个位置
            int safeIndex = playerIndex % _seats.Length;
            return _seats[safeIndex];
        }

        // 可以在这里添加控制升降台动画的方法
        public void SetElevatorHeight(float height)
        {
            if (_centerElevator != null)
            {
                Vector3 pos = _centerElevator.localPosition;
                pos.y = height;
                _centerElevator.localPosition = pos;
            }
        }
    }
}
