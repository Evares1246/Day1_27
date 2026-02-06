using UnityEngine;
using GameDesign.Utils;
using GameDesign.Data;
using System.Collections.Generic;

namespace GameDesign.Gameplay.Map
{
    public class BoardManager : Singleton<BoardManager>
    {
        [Header("Generation Settings")]
        public float halfSideLength = 6.55f; // 边长的一半
        public float tileSpacing = 1.31f;    // 地块间距 (必须能被总长整除以保证对齐)
        public TileDataSO defaultTileSO;     // 默认填充地块

        [Header("Runtime Data")]
        public BoardTrack mainTrack = new BoardTrack();

        protected override void Awake()
        {
            base.Awake();
            GenerateBoardData();
        }

        private void GenerateBoardData()
        {
            mainTrack.Tiles.Clear();
            mainTrack.WorldPositions.Clear();

            // 计算单边可以放多少个间隔
            int segmentsPerSide = Mathf.RoundToInt((halfSideLength * 2) / tileSpacing);
            float L = halfSideLength;

            // 按照 正方形 四条边生成：底边 -> 右边 -> 顶边 -> 左边
            // 1. 底边 (Left-Bottom to Right-Bottom)
            for (int i = 0; i < segmentsPerSide; i++)
                AddTile(new Vector3(-L + i * tileSpacing, 0, -L));

            // 2. 右边 (Right-Bottom to Right-Top)
            for (int i = 0; i < segmentsPerSide; i++)
                AddTile(new Vector3(L, 0, -L + i * tileSpacing));

            // 3. 顶边 (Right-Top to Left-Top)
            for (int i = 0; i < segmentsPerSide; i++)
                AddTile(new Vector3(L - i * tileSpacing, 0, L));

            // 4. 左边 (Left-Top to Left-Bottom)
            for (int i = 0; i < segmentsPerSide; i++)
                AddTile(new Vector3(-L, 0, L - i * tileSpacing));

            Debug.Log($"[BoardManager] 自动生成完成，总计 {mainTrack.Count} 个地块。");
        }

        private void AddTile(Vector3 pos)
        {
            mainTrack.WorldPositions.Add(pos);
            mainTrack.Tiles.Add(defaultTileSO); // 实际项目中这里可以按规律放入不同 SO
        }

        // 供 PlayerBrain 获取坐标
        public Vector3 GetPositionByIndex(int index) => mainTrack.GetPosition(index);
        public int GetTotalTileCount() => mainTrack.Count;


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (mainTrack == null || mainTrack.WorldPositions.Count == 0)
            {
                // 预览逻辑：如果没有运行，手动算一遍预览点
                Gizmos.color = Color.yellow;
                float L = halfSideLength;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(L * 2, 0.1f, L * 2));
                return;
            }

            Gizmos.color = Color.cyan;
            for (int i = 0; i < mainTrack.WorldPositions.Count; i++)
            {
                Gizmos.DrawSphere(mainTrack.WorldPositions[i], 0.2f);
                if (i < mainTrack.WorldPositions.Count - 1)
                    Gizmos.DrawLine(mainTrack.WorldPositions[i], mainTrack.WorldPositions[i + 1]);
                else
                    Gizmos.DrawLine(mainTrack.WorldPositions[i], mainTrack.WorldPositions[0]);
            }
        }
#endif
    }
}