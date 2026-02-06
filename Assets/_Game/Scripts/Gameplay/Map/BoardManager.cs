using UnityEngine;
using System.Collections.Generic;
using GameDesign.Utils;
using GameDesign.Data;

namespace GameDesign.Gameplay.Map
{
    /// <summary>
    /// 负责数学计算并生成正方形轨道路径点。
    /// </summary>
    public class BoardManager : Singleton<BoardManager>
    {
        [Header("Generation Params")]
        public float halfSideLength = 6.55f; // 正方形中心到边的距离
        public float tileSpacing = 1.31f;    // 地块中心间距
        public TileDataSO defaultTileData;

        [Header("Generated Data")]
        public List<Vector3> TilePositions = new List<Vector3>();
        public List<TileDataSO> TileLogicData = new List<TileDataSO>();

        public int TotalTiles => TilePositions.Count;

        protected override void Awake()
        {
            base.Awake();
            GenerateTrack();
        }

        private void GenerateTrack()
        {
            TilePositions.Clear();
            TileLogicData.Clear();

            float L = halfSideLength;
            // 每边步数 = 总长度 / 间距 (13.1 / 1.31 = 10)
            int steps = Mathf.RoundToInt((halfSideLength * 2) / tileSpacing);

            // 按顺序生成正方形的四个边点：
            // 底边 (Left-Bottom to Right-Bottom)
            for (int i = 0; i < steps; i++) AddPoint(new Vector3(-L + i * tileSpacing, 0, -L));
            // 右边 (Right-Bottom to Right-Top)
            for (int i = 0; i < steps; i++) AddPoint(new Vector3(L, 0, -L + i * tileSpacing));
            // 顶边 (Right-Top to Left-Top)
            for (int i = 0; i < steps; i++) AddPoint(new Vector3(L - i * tileSpacing, 0, L));
            // 左边 (Left-Top to Left-Bottom)
            for (int i = 0; i < steps; i++) AddPoint(new Vector3(-L, 0, L - i * tileSpacing));

            Debug.Log($"[BoardManager] Track fully generated with {TotalTiles} tiles.");
        }

        private void AddPoint(Vector3 pos)
        {
            TilePositions.Add(pos);
            TileLogicData.Add(defaultTileData);
        }

        public Vector3 GetPosition(int index) => TilePositions[index % TotalTiles];
        public TileDataSO GetTileData(int index) => TileLogicData[index % TotalTiles];

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (TilePositions.Count == 0) return;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < TilePositions.Count; i++)
            {
                Gizmos.DrawWireCube(TilePositions[i], new Vector3(1, 0.1f, 1));
                Gizmos.DrawLine(TilePositions[i], TilePositions[(i + 1) % TilePositions.Count]);
            }
        }
#endif
    }
}